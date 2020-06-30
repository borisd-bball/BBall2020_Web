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
    public class DevicesData
    {
        public class DeviceData
        {
            public int Id { get; set; }
            public string acDevID { get; set; }
            public decimal anBatteryVoltage { get; set; }
            public decimal anSensor1 { get; set; }
            public decimal anSensor2 { get; set; }
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
            sSQL.AppendLine("SELECT TOP 500 * FROM DevicesData ");
            sSQL.AppendLine("ORDER BY ID desc ");
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
            sSQL.AppendLine("select TOP 500 * from DevicesData where acDevID=@id ORDER BY ID desc ");
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
            sSQL.AppendLine("select TOP 500 * from DevicesData where ID = @ID ORDER BY ID desc ");
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

        public static List<DeviceData> Get(int ID, string acDevID)
        {
            DataTable dt = new DataTable();
            List<DeviceData> lResp = new List<DeviceData>();

            if (ID > 0) { dt = Get_d(ID); }
            else if (!String.IsNullOrEmpty(acDevID)) { dt = Get_d(acDevID); }
            else { dt = Get_d(); }

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new DeviceData
                   { Id = Convert.ToInt32(r["ID"]),
                    acDevID = Convert.ToString(r["acDevID"]),
                    anBatteryVoltage = Convert.ToDecimal(r["anBatteryVoltage"]),
                    anSensor1 = Convert.ToDecimal(r["anSensor1"]),
                    anSensor2 = Convert.ToDecimal(r["anSensor2"]),
                    adModificationDate = Convert.ToDateTime(r["adModificationDate"]),
                    anUserMod = Convert.ToInt32(r["anUserMod"])
                });
            }

            return lResp;
        }
        public static List<DeviceData> Set(DeviceData pData)
        {

            if (pData.Id > 0) { Update(pData); } else { Insert(pData); }

            return Get(pData.Id, pData.acDevID);
        }
        public static void Upload(DeviceData pData)
        {
            Insert(pData); 
        }


        public static void Delete(DeviceData pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM DevicesData  ");
            sSQL.AppendLine("WHERE (ID = @ID) ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ID", pData.Id)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Update(DeviceData pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("update DevicesData SET adModificationDate = @adModificationDate, anUserMod = @anUserMod, anBatteryVoltage = @anBatteryVoltage, anSensor1 = @anSensor1, ");
            sSQL.AppendLine(" anSensor2 = @anSensor2 ");
            sSQL.AppendLine("where id=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@id", pData.Id),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anBatteryVoltage", pData.anBatteryVoltage),
                        new SqlParameter("@anSensor1", pData.anSensor1),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anSensor2", pData.anSensor2)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Insert(DeviceData pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("INSERT INTO DevicesData (acDevID, anBatteryVoltage, anSensor1, anUserMod, anSensor2)  ");
            sSQL.AppendLine(" VALUES (@acDevID, @anBatteryVoltage, @anSensor1, @anUserMod, @anSensor2) ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@acDevID", pData.acDevID),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anBatteryVoltage", pData.anBatteryVoltage),
                        new SqlParameter("@anSensor1", pData.anSensor1),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anSensor2", pData.anSensor2)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }

    }

}
