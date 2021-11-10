using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace VoxelEngine.Network
{
    public partial class FormServer : Form
    {
        protected SocketServer server;

        protected bool isClose = false;

        public FormServer()
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

        /// <summary>
        /// Получить истину запущен ли сервер
        /// </summary>
        public bool IsRun() => server != null && server.IsConnected;

        private void buttonRun_Click(object sender, EventArgs e) => Run();
        private void buttonStop_Click(object sender, EventArgs e) => Stop(false);
        private void buttonSend_Click(object sender, EventArgs e) => Send();
        private void FormServer_Load(object sender, EventArgs e) => Run();
        private void FormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsRun())
            {
                e.Cancel = true;
                Stop(true);
            }
        }

        protected void Run()
        {
            if (!IsRun())
            {
                server = new SocketServer(32021);
                server.Error += Server_Error;
                server.Stopping += Server_Stopping;
                server.Stopped += Server_Stopped;
                server.Runned += Server_Runned;
                server.Receive += Server_Receive;
                server.Run();
                Log("Click Run");
            } else
            {
                Log("Click Run - Hmmm");
            }
        }

        private void Server_Runned(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(Server_Stopped), sender, e);
            else Log("Runned");
        }

        private void Server_Receive(object sender, ServerPacketEventArgs e)
        {
            if (InvokeRequired) Invoke(new ServerPacketEventHandler(Server_Receive), sender, e);
            else
            {
                if (e.Packet.Status == StatusNet.Connect || e.Packet.Status == StatusNet.Disconnect)
                {
                    Log("Status {1} {0}", e.Packet.Status, e.Packet.WorkSocket.RemoteEndPoint);
                    listBoxClients.Items.Clear();
                    foreach(Socket socket in server.Clients())
                    {
                        listBoxClients.Items.Add(new SocketHeir(socket));
                    }
                } else if (e.Packet.Status == StatusNet.Receive)
                {
                    Log("Receive {1} {0}", Encoding.UTF8.GetString(e.Packet.Bytes), e.Packet.WorkSocket.RemoteEndPoint);
                }
                else if (e.Packet.Status == StatusNet.Loading)
                {
                    Log("Loading {2} {1}/{0}", e.Leght, e.Received, e.Packet.WorkSocket.RemoteEndPoint);
                }
            }
        }

        private void Server_Error(object sender, ErrorEventArgs e)
        {
            try { 
                if (InvokeRequired) Invoke(new ErrorEventHandler(Server_Error), sender, e);
                else Log("ERROR: {0}", e.GetException().Message);
            } catch { }
        }

        private void Server_Stopping(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(Server_Stopped), sender, e);
            else Log("Stopping");
        }

        private void Server_Stopped(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(Server_Stopped), sender, e);
            else
            {
                Log("Stopped");
                if (isClose) Close();
            }
        }

        protected void Stop(bool isClose)
        {
            Log("Click Stop");
            this.isClose = isClose;
            server.Stop();
        }

        protected void Send()
        {
            if (listBoxClients.SelectedItem != null && listBoxClients.SelectedItem.GetType() == typeof(SocketHeir))
            {
                SocketHeir s = listBoxClients.SelectedItem as SocketHeir;
                server.Sender(s.WorkSocket, Encoding.UTF8.GetBytes(textBoxSend.Text));
                Log("ServerSend");
            }
        }
    }
}
