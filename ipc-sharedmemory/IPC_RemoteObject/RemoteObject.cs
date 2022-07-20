using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IPC_RemoteObject
{
    public class RemoteObject : MarshalByRefObject
    {
        /// ---------------------------------------------------------------------
        /// 구조체
        /// ---------------------------------------------------------------------
        public static System.Data.DataTable dt_r = null;
        public struct RemoteMessage
        {
            static public decimal ID = 0;
            static public string PRG = "";
            static public string MSG = "";
            static public string DIV = "";
            static public string FLAG = "";
        }

        public RemoteObject()
        {
            try {
                lfn_dt_Create(ref dt_r);
            }
            catch (Exception ex) { throw ex; }
        }

        private void lfn_dt_Create(ref System.Data.DataTable pdt)
        {
            try
            {
                if (pdt != null) { return; }
                pdt = new DataTable();

                pdt.Columns.Add("ID"); pdt.Columns["ID"].DataType = System.Type.GetType("System.Decimal");

                pdt.Columns.Add("PRG"); //작성_프로그램명
                pdt.Columns.Add("MSG"); //주요메세지
                pdt.Columns.Add("DIV"); //구분
                pdt.Columns.Add("FLAG");//상태값

                //기본키처리
                DataColumn[] pk = new DataColumn[1];
                pk[0] = pdt.Columns["ID"];
                pdt.PrimaryKey = pk;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        #region "set"
        /// <summary>
        /// 메세지값입력
        /// </summary>
        /// <param name="PRG">프로그램명</param>
        /// <param name="MSG">메세지</param>
        /// <param name="DIV">구분값</param>
        /// <param name="FLAG">상태값</param>
        public void Set_MSG_INS(string PRG, string MSG, string DIV, string FLAG)
        {
            try
            {
                RemoteMessage.ID    = RemoteMessage.ID + 1;
                RemoteMessage.PRG   = PRG;
                RemoteMessage.MSG   = MSG;
                RemoteMessage.FLAG  = FLAG;

                DataRow dr_r = dt_r.NewRow();
                dr_r["ID"] = RemoteMessage.ID;
                dr_r["MSG"] = RemoteMessage.MSG;
                dr_r["DIV"] = RemoteMessage.DIV;
                dr_r["FLAG"] = RemoteMessage.FLAG;
                dt_r.Rows.Add(dr_r);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Flag값변경처리
        public void Set_FLAG_EDT(decimal pID, string pFLAG)
        {
            try
            {
                DataRow dr_r = dt_r.Rows.Find(pID);
                if (dr_r==null) { return; }
                dr_r["FLAG"] = pFLAG;
                
                //int selRow = dt_r.Rows.IndexOf(dr_r);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion //set




        #region "get"
        public DataTable getDATA()
        {
            try
            { 
                return dt_r;
            }
            catch (Exception ex)
            {
                
                throw ex;
                return null;
            }
        }

        public decimal get_ID() { return RemoteMessage.ID; }

        public void get_LAST_MSG(ref decimal pID, ref string pPRG,ref string pMSG,ref string pDIV, ref string pFLAG )
        {
            pID = RemoteMessage.ID;
            pPRG = RemoteMessage.PRG;
            pMSG = RemoteMessage.MSG;
            pDIV = RemoteMessage.DIV;
            pFLAG = RemoteMessage.FLAG;
        }
        #endregion //get

    }
}
