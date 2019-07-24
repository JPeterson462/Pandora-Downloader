using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Pandora
{
    public class RESTfulService
    {
        internal HttpClient _httpClient;
        public HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                return _httpClient;
            }
        }
        public async Task<HttpResponseMessage> DoPost(string url, Dictionary<string, object> data, CryptoConfig config)
        {
            string content = JsonConvert.SerializeObject(data);
            if (config != null)
            {
                Crypto crypto = new Crypto();
                content = crypto.Encrypt(config, content);
            }
            return await HttpClient.PostAsync(url, new StringContent(content, Encoding.UTF8));
        }
    }
}
