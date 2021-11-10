using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VoxelEngine.Network
{
    public partial class FormClient : Form
    {
        protected SocketClient client;

        public FormClient()
        {
            InitializeComponent();
        }

        protected void Log(string str, params object[] args)
        {
            textBoxLog.Text = string.Format("{2:HH:mm.ss} {0}\r\n{1}",
                string.Format(str, args), textBoxLog.Text, DateTime.Now);
            textBoxLog.SelectionStart = 0;
            textBoxLog.ScrollToCaret();
        }

        public void SetNumber(string name)
        {
            Text = "FormClient #" + name.ToString();
        }

        private void buttonConnect_Click(object sender, EventArgs e) => Connect();
        private void buttonDisconnect_Click(object sender, EventArgs e) => Disconnect();
        private void buttonSend_Click(object sender, EventArgs e) => Send();
        private void FormClient_Load(object sender, EventArgs e) => Connect();
        private void FormClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Disconnect();
        }


        protected void Connect()
        {
            if (client == null || !client.IsConnected)
            {
                Log("ClientConnection...");
                client = new SocketClient(System.Net.IPAddress.Parse("127.0.0.1"), 32021);
                client.Receive += Client_Receive;
                client.Error += Client_Error;
                client.Connect();
            }
        }

        private void Client_Receive(object sender, ServerPacketEventArgs e)
        {
            if (InvokeRequired) Invoke(new ServerPacketEventHandler(Client_Receive), sender, e);
            else
            {
                if (e.Packet.Status == StatusNet.Receive)
                {
                    Log("Receive {0}", Encoding.UTF8.GetString(e.Packet.Bytes));
                }
                else if (e.Packet.Status == StatusNet.Loading)
                {
                    Log("Loading {1}/{0}", e.Leght, e.Received);
                }
                else
                {
                    Log("Status {0}", e.Packet.Status);
                    if (e.Packet.Status == StatusNet.Connect)
                    {
                        SetNumber(e.Packet.WorkSocket.LocalEndPoint.ToString());
                    }
                    else if (e.Packet.Status == StatusNet.Disconnect)
                    {
                        SetNumber("-");
                    }
                }
            }
        }

        private void Client_Error(object sender, ErrorEventArgs e)
        {
            try
            {
                if (InvokeRequired) Invoke(new ErrorEventHandler(Client_Error), sender, e);
                else Log("ERROR: {0}", e.GetException().Message);
            } catch { }
        }

        protected void Disconnect()
        {
            if (client != null)
            {
                client.Disconnect();
            }
            Log("ClientDisconnect");
        }

        protected void Send()
        {
            if (client != null)
            {
                client.Sender(Encoding.UTF8.GetBytes(textBoxSend.Text));
                Log("ClientSend");
            }
        }
    }
}
