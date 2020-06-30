using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace mr.bBall_Lib
{
    public class Devices
    {
        public class Device
        {
            public int Id { get; set; }
            public string acDevID { get; set; }
            public string acTitle { get; set; }
            public string acBT_Name { get; set; }
            public string acEmail { get; set; }
            public DateTime adInsetDate { get; set; }
            public int anUserIns { get; set; }
            public DateTime adModificationDate { get; set; }
            public int anUserMod { get; set; }
        }


        public static DataTable Get_d()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM Devices ");
            sSQL.AppendLine("ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;

        }
        public static DataTable Get_d(string acDevID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from Devices where acDevID=@id ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@id", acDevID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_d(int ID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from Devices where ID = @ID ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@ID", ID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }

        public static List<Device> Get(int ID, string acDevID)
        {
            DataTable dt = new DataTable();
            List<Device> lResp = new List<Device>();

            if (ID > 0) { dt = Get_d(ID); }
            else if (!String.IsNullOrEmpty(acDevID)) { dt = Get_d(acDevID); }
            else { dt = Get_d(); }

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new Device { Id = Convert.ToInt32(r["ID"]),
                                       acDevID = Convert.ToString(r["acDevID"]),
                                       acTitle = Convert.ToString(r["acTitle"]),
                                       acBT_Name = Convert.ToString(r["acBT_Name"]),
                                       acEmail = Convert.ToString(r["acEmail"]),
                                       adInsetDate = Convert.ToDateTime(r["adInsetDate"]),
                                       anUserIns = Convert.ToInt32(r["anUserMod"]),
                                       adModificationDate = Convert.ToDateTime(r["adModificationDate"]),
                                       anUserMod = Convert.ToInt32(r["anUserMod"])
                });
            }

            return lResp;
        }
        public static List<Device> Set(Device pData)
        {

            if (pData.Id > 0) { Update(pData); } else { Insert(pData); }

            return Get(pData.Id, pData.acDevID);
        }
        public static void Upload(Device pData)
        {
            List<Device> ld = Get(0, pData.acDevID);
            if (ld.Count > 0) { pData.Id = ld[0].Id; }

            if (pData.Id > 0) { Update(pData); } else { Insert(pData); }

        }


        public static void Delete(Device pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM Devices  ");
            sSQL.AppendLine("WHERE (ID = @ID) ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ID", pData.Id)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Update(Device pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("update Devices SET acTitle = @acTitle, adModificationDate = @adModificationDate, anUserMod = @anUserMod, acBT_Name = @acBT_Name, acEmail = @acEmail ");
            sSQL.AppendLine("  ");
            sSQL.AppendLine("where id=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@id", pData.Id),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@acBT_Name", pData.acBT_Name),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@acEmail", pData.acEmail)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Insert(Device pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("INSERT INTO Devices (acDevID, acBT_Name, acTitle, anUserMod, adInsetDate, acEmail, anUserIns)  ");
            sSQL.AppendLine(" VALUES (@acDevID, @acBT_Name, @acTitle, @anUserMod, @adInsetDate, @acEmail, @anUserIns) ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@acDevID", pData.acDevID),
                        new SqlParameter("@acBT_Name", pData.acBT_Name),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@adInsetDate", pData.adInsetDate),
                        new SqlParameter("@acEmail", pData.acEmail),
                        new SqlParameter("@anUserIns", pData.anUserIns)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }

    }

}
