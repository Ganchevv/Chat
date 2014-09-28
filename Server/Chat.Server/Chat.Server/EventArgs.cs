using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking
{       
    public class ClientConnectedEventArgs : EventArgs
    {
        public string IP { get; set; }

        public ClientConnectedEventArgs(string ip)
        {
            this.IP = ip;
        }
    }  
}
