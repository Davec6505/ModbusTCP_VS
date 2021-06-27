using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SocketAsync;

namespace ModbusTCP_VS
{
    public partial class Form1 : Form
    {
        SocketServer mServer;
        public Form1()
        {
            InitializeComponent();
            mServer = new SocketServer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            mServer.StartListeningForIncomingConnection();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            mServer.StopServer();
        }

        private void btnConvertStrA_Click(object sender, EventArgs e)
        {
            mServer.SendToAll(tbStringA.Text.Trim());
        }

    }
}
