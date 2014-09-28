namespace Chat.Server
{
    using Networking;
    using System;
    using System.Windows.Forms;

    public partial class ServerGUI : Form
    {
        ChatServer server;
        public ServerGUI()
        {
            InitializeComponent();
            server = new ChatServer();
            server.StatusChanged += server_StatusChanged;
            server.ClientConnected += server_ClientConnected;
        }

        void server_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            MessageBox.Show(e.IP);
        }

        void server_StatusChanged(object sender, EventArgs e)
        {
            logBox.AppendText(server.SystemMessage);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.Start(int.Parse(textBox1.Text));
        }
    }
}
