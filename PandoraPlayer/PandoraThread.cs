using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandoraPlayer
{
    public class PandoraThread
    {
        public static void Run(PlayerContext context)
        {
            while (true)
            {
                context.PlayNext();
            }
        }
    }
}
