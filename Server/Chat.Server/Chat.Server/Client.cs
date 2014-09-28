using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Networking
{
    public class Client
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public Socket Socket { get; set; }

        public Client(Socket socket)
        {
            this.Socket = socket;
        }
    }
}
