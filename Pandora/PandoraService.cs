using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web;

namespace Pandora
{
    public class PandoraService : IPandoraService
    {
        public async Task<PandoraState> ConnectAsync(RESTfulService service)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "username", "android" },
                { "password", PandoraConstants.ANDROID_PARTNER_PASSWORD },
                { "deviceModel", "android-generic" },
                { "version", "5" },
                { "includeUrls", true }
            };

            CryptoConfig cryptoConfig = PandoraConstants.GetCryptoConfig();
            Crypto crypto = new Crypto();

            HttpResponseMessage response = await service.DoPost(PandoraConstants.BuildUrl("method=auth.partnerLogin"), data, null);
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            PartnerLoginDataDto loginData = JsonConvert.DeserializeObject<PartnerLoginDataDto>(await response.Content.ReadAsStringAsync(), serializerSettings);
            string partnerAuthToken = loginData.Result.PartnerAuthToken;
            string decryptedSyncTime = crypto.Decrypt(cryptoConfig, loginData.Result.SyncTime);
            long syncTime = long.Parse(System.Text.RegularExpressions.Regex.Match(decryptedSyncTime.Substring(4), "^[0-9]*").Groups[0].Value);
            int partnerId = loginData.Result.PartnerId;

            return await Task.FromResult(new PandoraState {
                PartnerAuthToken = partnerAuthToken,
                SyncTime = syncTime,
                PartnerId = partnerId
            });
        }
        public async Task<bool> LoginAsync(PandoraConfig config, RESTfulService service, PandoraState pandoraState)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "loginType", "user" },
                { "username", config.Username },
                { "password", config.Password },
                { "partnerAuthToken", pandoraState.PartnerAuthToken },
                { "syncTime", pandoraState.SyncTime }
            };

            CryptoConfig cryptoConfig = PandoraConstants.GetCryptoConfig();
            Crypto crypto = new Crypto();

            HttpResponseMessage response = await service.DoPost(PandoraConstants.BuildUrl($"method=auth.userLogin&auth_token={HttpUtility.UrlEncode(pandoraState.PartnerAuthToken)}&partner_id={pandoraState.PartnerId}"), data, cryptoConfig);
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            PartnerLoginStatusDto loginData = JsonConvert.DeserializeObject<PartnerLoginStatusDto>(await response.Content.ReadAsStringAsync(), serializerSettings);
            if (loginData.Stat == "ok")
            {
                pandoraState.UserAuthToken = loginData.Result.UserAuthToken;
                pandoraState.UserId = loginData.Result.UserId;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
        public async Task<HttpResponseMessage> DoStandardCallAsync(RESTfulService service, string method, Dictionary<string, object> data, bool useSsl, PandoraState pandoraState, CryptoConfig config)
        {
            string url = (useSsl ? PandoraConstants.BASE_URL : PandoraConstants.BASE_NON_TLS_URL) + $"method={method}&auth_token={HttpUtility.UrlEncode(pandoraState.UserAuthToken)}&partner_id={pandoraState.PartnerId}&user_id={pandoraState.UserId}";
            data["userAuthToken"] = pandoraState.UserAuthToken;
            data["syncTime"] = pandoraState.SyncTime;
            return await service.DoPost(url, data, config);
        }
        public async Task<List<PandoraStation>> GetPandoraStationsAsync(PandoraConfig config, RESTfulService service, PandoraState pandoraState)
        {
            CryptoConfig cryptoConfig = PandoraConstants.GetCryptoConfig();

            HttpResponseMessage response = await DoStandardCallAsync(service, "user.getStationList", new Dictionary<string, object>(), false, pandoraState, cryptoConfig);
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            PandoraStationListDto stationList = JsonConvert.DeserializeObject<PandoraStationListDto>(await response.Content.ReadAsStringAsync());
            List<PandoraStation> stations = new List<PandoraStation>();
            foreach (PandoraStationDto station in stationList.Result.Stations)
            {
                stations.Add(new PandoraStation
                {
                    Id = station.StationId,
                    Token = station.StationToken,
                    IsQuickMix = station.IsQuickMix,
                    Name = station.StationName
                });
            }
            stations.Sort((station1, station2) => string.Compare(station1.Name, station2.Name));
            return await Task.FromResult(stations);
        }
        public async Task<bool> RatePandoraSongAsync(RESTfulService service, PandoraState pandoraState, PandoraStation station, PandoraSong song, bool rating)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "trackToken", song.TrackToken },
                { "isPositive", rating }
            };

            CryptoConfig cryptoConfig = PandoraConstants.GetCryptoConfig();

            await DoStandardCallAsync(service, "station.addFeedback", data, false, pandoraState, cryptoConfig);
            return await Task.FromResult(true);
        }
        public async Task<List<PandoraSong>> GetPandoraPlaylistAsync(RESTfulService service, PandoraState pandoraState, PandoraStation station)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "stationToken", station.Token },
                { "additionalAudioUrl", "HTTP_192_MP3,HTTP_128_MP3" }
            };

            CryptoConfig cryptoConfig = PandoraConstants.GetCryptoConfig();

            HttpResponseMessage response = await DoStandardCallAsync(service, "station.getPlaylist", data, false, pandoraState, cryptoConfig);
            PandoraSongListDto songList = JsonConvert.DeserializeObject<PandoraSongListDto>(await response.Content.ReadAsStringAsync());
            List<PandoraSong> playlist = new List<PandoraSong>();
            foreach (PandoraSongDto song in songList.Result.Items)
            {
                if (song.AdToken != null)
                {
                    continue; // Ignore advertisements
                }
                playlist.Add(new PandoraSong
                {
                    AlbumName = song.AlbumName,
                    ArtistName = song.ArtistName,
                    AudioUrl = song.AudioUrlMap.HighQuality.AudioUrl ?? song.AdditionalAudioUrl,
                    Title = song.SongName,
                    AlbumArtUrl = song.AlbumArtUrl,
                    TrackToken = song.TrackToken,
                    Rating = song.SongRating
                });
            }
            return await Task.FromResult(playlist);
        }
    }
}
