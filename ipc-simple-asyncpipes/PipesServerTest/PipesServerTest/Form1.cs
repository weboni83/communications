using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace PipesServerTest
{
    public partial class Form1 : Form
    {
        public delegate void NewMessageDelegate(string NewMessage);
        private PipeServer _pipeServer;

        public Form1()
        {
            InitializeComponent();
            _pipeServer = new PipeServer();
            _pipeServer.PipeMessage += new DelegateMessage(PipesMessageHandler);
        }

        private void cmdListen_Click(object sender, EventArgs e)
        {
            try
            {
                _pipeServer.Listen("TestPipe");
                txtMessage.Text = "Listening - OK";
                cmdListen.Enabled = false;
            }
            catch (Exception)
            {
                txtMessage.Text = "Error Listening";
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PipesMessageHandler(string message)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new NewMessageDelegate(PipesMessageHandler), message);
                }
                else
                {
                    txtMessage.Text = message;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _pipeServer.PipeMessage -= new DelegateMessage(PipesMessageHandler);
            _pipeServer = null;

        }
    }
}
