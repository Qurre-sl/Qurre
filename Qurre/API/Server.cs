using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API
{
    static public class Server
    {
        private static Player host;

        static public ushort Port => ServerStatic.ServerPort;
        static public Player Host
        {
            get
            {
                if (host is null || host.ReferenceHub is null) host = new Player(ReferenceHub.HostHub);
                return host;
            }
        }
    }
}