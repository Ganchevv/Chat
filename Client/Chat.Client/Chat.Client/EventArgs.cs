using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking
{
    public class ReceivedTextEventArgs : EventArgs
    {
        public string Text {get; set;}       

        public ReceivedTextEventArgs(string text)
        {
            this.Text = text;
        }
    }
    public class ConnectedEventArgs : EventArgs
    {
        public bool Connected { get; set; }

        public ConnectedEventArgs(bool cnn)
        {
            this.Connected = cnn;
        }
    }
}
