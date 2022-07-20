using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.Remoting;
using IPC_RemoteObject;

namespace pipeClient
{
    public partial class Form1 : Form
    {
        System.Runtime.Remoting.Channels.Ipc.IpcClientChannel client;
        IPC_RemoteObject.RemoteObject RObj;
        decimal ID_OLD =-1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            lfn_Con();
        }


        private void lfn_Con()
        {
            try
            {
                client = new System.Runtime.Remoting.Channels.Ipc.IpcClientChannel();
                System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(client, false);

                System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownClientType(typeof(IPC_RemoteObject.RemoteObject), "ipc://SERVER_REMOTE_001/rodata");

                RObj = new RemoteObject();
                //ID_OLD = RObj.get_ID();

                timer1.Enabled = true;
            }
            catch (Exception ex)
            {
                lfn_txt(ex.Message.ToString());
                lfn_Close();
            }
        }

        private void lfn_txt(string str)
        {
            textBox1.AppendText(str + Environment.NewLine);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (RObj == null) { timer1.Enabled = false; lfn_Close();  return; }

                decimal ID_NEW = -1;
                ID_NEW = RObj.get_ID();

                if (ID_NEW.Equals(ID_OLD)) { return; }

                System.Data.DataTable dt = RObj.getDATA();
                if (dt==null) { return; }


                for (decimal i = ID_OLD+1; i <= ID_NEW; i++)
                {
                    RObj.Set_FLAG_EDT(i, "Y");
                }

                DataRow[] drs = dt.Select("ID>" + ID_OLD + " AND ID<=" + ID_NEW);
                lfn_txt("ID_OLD: "+ ID_OLD +"  | ID_NEW: " + ID_NEW + " | Count:" + drs.Length);


                foreach (DataRow dr in drs)
                {
                    lfn_txt(dr["MSG"].ToString()); //출력
                }

                ID_OLD = ID_NEW;
            }
            catch (Exception ex)
            {
                lfn_txt(ex.Message.ToString());
                timer1.Enabled = false;
                lfn_Close();
            }
        }

        private void lfn_Close()
        {
            try
            {
                System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(client);
                client = null;
                RObj = null;
                //System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownClientType(null, null);
                ID_OLD = -1;
            }
            catch (Exception ex)
            {
                lfn_txt(ex.Message.ToString());
            }
        }


    }
}
