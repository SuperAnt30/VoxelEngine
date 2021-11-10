using System;
using System.Windows.Forms;

namespace VoxelEngine.Network
{
    public partial class FormNetwork : Form
    {
        private FormServer formServer;
        private int numberClient = 0;

        public FormNetwork()
        {
            InitializeComponent();
        }

        private void buttonServer_Click(object sender, EventArgs e)
        {
            if (formServer == null || formServer.IsDisposed)
            {
                formServer = new FormServer();
                formServer.Show();
            }
            else
            {
                formServer.Focus();
            }
        }

        private void buttonClient_Click(object sender, EventArgs e)
        {
            numberClient++;
            FormClient formClient = new FormClient();
            formClient.SetNumber("-");
            formClient.Show();
        }

        private void FormNetwork_Load(object sender, EventArgs e)
        {
            buttonServer_Click(sender, e);
            buttonClient_Click(sender, e);
            buttonClient_Click(sender, e);
        }
    }
}
