namespace Networking
{
    using Chat.Client;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;

    public class ChatClient
    {
        Socket client;
        public event EventHandler<ReceivedTextEventArgs> TextReceive;
        public event EventHandler<ConnectedEventArgs> ConnectedToServer;
        byte[] buffer = new byte[10];       
        private string textToReceive;
        byte[] receiveBuffer;
        private IPAddress ipaddress;        

        public string Text
        {
            get
            {
                return textToReceive;
            }
            set
            {
                textToReceive = value;
                OnReceiveText(textToReceive);
            }

        }
        
        //IPEndPoint endPoint;

        public ChatClient()
        {
            ipaddress = IPAddress.Parse("127.0.0.1");
            this.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public ChatClient(Socket s)
        {
            ipaddress = IPAddress.Parse("127.0.0.1");
            client = s;
        }

        public string Name
        {
            get;
            set;
        }          

        public void Connect(string Name)
        {
            this.Name = Name;
            try
            {
                client.BeginConnect(ipaddress, 5001, new AsyncCallback(stopConn), null);
            }
            catch (SocketException ex)
            {
                throw ex;
            }
        }

        private void stopConn(IAsyncResult result)
        {
            try
            {
                client.EndConnect(result);
                SendName();                
                OnConnectedToServer();
                // open the broadcast receiver 
                receiveBuffer = new byte[1024];
                client.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveMessage), client);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }
       
        private void SendName()
        {
            byte[] name = Encoding.ASCII.GetBytes(this.Name);
            client.BeginSend(name, 0, name.Length, SocketFlags.None,
                new AsyncCallback(EndSendName), client);
        }

        private void EndSendName(IAsyncResult result)
        {
            var s = (Socket)result.AsyncState;
            s.EndSend(result);
        }
        
        private void ReceiveMessage(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            int size = socket.EndReceive(result);
            byte[] data = new byte[size];
            Array.Copy(receiveBuffer, data, size);
            string text = Encoding.ASCII.GetString(data);
            this.Text = text;
            receiveBuffer = new byte[1024];
            client.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None,
                new AsyncCallback(ReceiveMessage), client);
        }

        public void SendMessage(string s)
        {
            try
            {
                buffer = Encoding.ASCII.GetBytes(s);
                client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                    new AsyncCallback(EndSend), client);
            }
            catch (Exception e)
            {
                throw new Exception("sorry: " + e);
            }
        }

        private void EndSend(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            socket.EndSend(result);

        }

        protected virtual void OnReceiveText(string text)
        {
            if (TextReceive != null)
            {
                TextReceive(this, new ReceivedTextEventArgs(text));
            }
        }

        protected void OnConnectedToServer()
        {
            if (ConnectedToServer != null)
            {
                ConnectedToServer(this, new ConnectedEventArgs(true));
            }
        }

        public bool isConnected()
        {
            if (client.Connected)
            {
                return true;
            }
            else return false;
        }

    }
}
