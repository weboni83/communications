using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

using System.Runtime.Remoting;
using IPC_RemoteObject;

namespace pipedirection1
{
    public partial class Form1 : Form
    {
        System.Runtime.Remoting.Channels.Ipc.IpcServerChannel svr;
        IPC_RemoteObject.RemoteObject RObj;
        decimal ID_OLD = -1;

        public Form1(string[] args)
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //서버시작
        private void button1_Click(object sender, EventArgs e)
        {
            lfn_SERVER();
        }

        //메세지전송
        private void BTN_SEND_MSG_Click(object sender, EventArgs e)
        {
            try
            {
                if (RObj == null) { lfn_txt("서버시작 전1"); return; }

                string str = txtMSG.Text.ToString();
                if (str.Equals("")) { lfn_txt("메세지가 비었습니다."); return; }
                RObj.Set_MSG_INS("SVR",str,"msg", "");
                txtMSG.Text = "";
            }
            catch (Exception ex)
            {
                lfn_txt("[ BTN_SEND_MSG_Click ERR ] " + ex.Message.ToString());
            }
        }

        //서버시작
        private void lfn_SERVER()
        {
            try
            {

                svr = new System.Runtime.Remoting.Channels.Ipc.IpcServerChannel("SERVER_REMOTE_001");
                System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(svr, false);

                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IPC_RemoteObject.RemoteObject), "rodata", WellKnownObjectMode.Singleton);
                lfn_txt(" Listening on → " + svr.GetChannelUri().ToString());

                RObj = new IPC_RemoteObject.RemoteObject();
            }
            catch (Exception ex)
            {
                lfn_txt("[ lfn_SERVER ERR ] " + ex.Message.ToString());
            }
        }

        //서버닫기
        private void lfn_Close()
        {
            try { 
                    System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(svr);
                    svr = null;
                    RObj = null;
                    ID_OLD = -1;
            }
            catch (Exception ex)
            {
                lfn_txt("[ lfn_SERVER ERR ] " + ex.Message.ToString());
            }
        }

        private void lfn_txt(string str)
        {
            textBox1.AppendText(str + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string strData = "";

                if (RObj == null) { lfn_txt("서버시작 전1"); return; }

                DataTable dt = RObj.getDATA();
                if (dt==null) { lfn_txt("서버시작 전2"); return; }

                foreach (DataRow dr in dt.Rows)
                {
                    string strRow = "";
                    for (int y = 0; y < dt.Columns.Count; y++)
                    {
                        if (!strRow.Equals("")) { strRow += ","; }
                        strRow = string.Format("{0} \"{1}\":\"{2}\"", strRow, dt.Columns[y].Caption.ToString() , dr[y].ToString());
                    }
                    strRow = "[" + strRow + "]" + Environment.NewLine;
                    strData += strRow;
                }

                if (strData.Equals("")) { strData = "내용없음"; }
                lfn_txt(strData); //내용출력

            }
            catch (Exception ex)
            {
                lfn_txt("[ button2_Click ERR ] " + ex.Message.ToString());
            }
        }




        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (RObj == null) { timer1.Enabled = false; return; }

                decimal ID_NEW = -1;
                ID_NEW = RObj.get_ID();

                if (ID_NEW.Equals(ID_OLD)) { return; }

                System.Data.DataTable dt = RObj.getDATA();
                if (dt == null) { return; }


                DataRow[] drs = dt.Select("ID>" + ID_OLD);
                lfn_txt("ID_OLD: " + ID_OLD + "  | ID_NEW: " + ID_NEW + " | Count:" + drs.Length);


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

 



    }
}
