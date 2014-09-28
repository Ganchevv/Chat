namespace Chat.Client
{
    using Networking;
    using System;
    using System.Net.Sockets;
    using System.Windows.Forms;

    public partial class ClientGUI : Form
    {
        private ChatClient client;       

        public ClientGUI()
        {
            InitializeComponent();
            client = new ChatClient();
            client.TextReceive += client_TextReceive;
            client.ConnectedToServer += client_ConnectedToServer;
        }

        void client_ConnectedToServer(object sender, ConnectedEventArgs e)
        {
            label1.Text = "Connection established";
        }

        void client_TextReceive(object sender, ReceivedTextEventArgs e)
        {
            textBox2.AppendText(e.Text + System.Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.Connect(textBox3.Text);            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.SendMessage(textBox1.Text);
            textBox1.Text = "";
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                client.SendMessage(textBox1.Text);
                textBox1.Text = "";

            }
        }
        
    }
}
