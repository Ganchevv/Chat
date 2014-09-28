namespace Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;


    public class ChatServer
    {
        Socket _server;
        List<Client> listOfConnections;
        Client client;        
        
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;           
        public event EventHandler StatusChanged;

        private string _backMessage;
        byte[] _buffer, name;

        public string SystemMessage
        {
            get
            {
                return _backMessage;
            }
            set
            {
                _backMessage = value;
                OnStatusChanged();
            }

        }
        public ChatServer()
        {
            listOfConnections = new List<Client>();
        }

        public List<Client> GetCurrentConnections()
        {
            return listOfConnections;

        }
        public void Start(int Port)
        {            
            try
            {
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _server.Bind(new IPEndPoint(IPAddress.Any, Port));
                _server.Listen(3);
                SystemMessage = "Server listening on port " + Port + "...." + Environment.NewLine;
                _server.BeginAccept(new AsyncCallback(EndAccept), null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EndAccept(IAsyncResult result)
        {
            try
            {
                var socket = _server.EndAccept(result);
                client = new Client(socket);
                listOfConnections.Add(client);
                var endPoint = (IPEndPoint)socket.RemoteEndPoint;
                OnClientConnected(endPoint.Address.ToString());
                name = new byte[1024];
                socket.BeginReceive(name, 0, name.Length, SocketFlags.None, 
                    NameReceive, socket);               

                _server.BeginAccept(EndAccept, null);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private void NameReceive(IAsyncResult result)
        {
            var s = (Socket)result.AsyncState;
            int received = s.EndReceive(result);
            byte[] receiveDname = new byte[received];
            Array.Copy(name, receiveDname, received);
            client.Name = Encoding.ASCII.GetString(receiveDname);
            SystemMessage = "We have a client connected! (" + client.Name + ")" + Environment.NewLine;
           
            _buffer = new byte[1024];
            s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None,
                ReceiveCallBack, s);
        }
        //private void IPAddReceive(IAsyncResult result)
        //{
        //    var s = (Socket)result.AsyncState;
        //    int received = s.EndReceive(result);
        //    byte[] receiveDip = new byte[received];
        //    Array.Copy(ipadd, receiveDip, received);
        //    string IP = Encoding.ASCII.GetString(receiveDip);

        //    client.IpAddress = IP;

        //    listOfConnections.Add(client);
        //    //OnClientConnected();

        //    _buffer = new byte[1024];
        //    s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None,
        //        new AsyncCallback(ReceiveCallBack), s);

        //}
        private void ReceiveCallBack(IAsyncResult result)
        {
            var s = (Socket)result.AsyncState;
            int receive = s.EndReceive(result);
            byte[] data = new byte[receive];
            Array.Copy(_buffer, data, receive);
            string text = Encoding.ASCII.GetString(data);
            _buffer = new byte[1024];

            //===== broadcast to all connected guys ======            
            BroadcastToClients(client.Name + ": " + text);

            //===== receive again from connected guys ======
            s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), s);

        }
        private void BroadcastToClients(string message)
        {
            foreach (Client cl in listOfConnections)
            {
                var s = cl.Socket;
                byte[] data = Encoding.ASCII.GetBytes(message);
                s.BeginSend(data, 0, data.Length, SocketFlags.None, MessageBroadcastCallBack, s);
            }
        }
        protected virtual void OnClientConnected(string ip)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, new ClientConnectedEventArgs(ip));
            }
        }
        protected virtual void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }
        private void MessageBroadcastCallBack(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;
            socket.EndSend(result);
            SystemMessage = "Message Received and sent to all users :)" + Environment.NewLine;

        }
        public void SendListOfCurrentConnections()
        {
            //using (Stream strem = File.Open("list.bin", FileMode.Create))
            //{
            //    var bin = new BinaryFormatter();
            //    bin.Serialize(strem, listOfConnections);
            //}
        }
    }
    }
