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
    public class SubTypes
    {
        public class SubType
        {
            public SubType(int ID, string acTypeID, int anCountryID, string acTitle, DateTime adModificationDate, int anUserMod, int anErpID)
            {
                this.ID = ID;
                this.acTypeID = acTypeID;
                this.anCountryID = anCountryID;
                this.acTitle = acTitle;
                this.adModificationDate = adModificationDate;
                this.anUserMod = anUserMod;
                this.anErpID = anErpID;
            }
            public int ID;
            public string acTypeID;
            public int anCountryID;
            public string acTitle;
            public DateTime adModificationDate;
            public int anUserMod;
            public int anErpID;
        }

        public static DataTable Get_d()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM SubTypes ");
            sSQL.AppendLine("ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;

        }
        public static DataTable Get_d(int anCountryID, string acTypeID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from SubTypes where acTypeID=@id and anCountryID = @anCountryID ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@id", acTypeID),
                new SqlParameter("@anCountryID", anCountryID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_d(int anCountryID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from SubTypes where anCountryID = @anCountryID ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@anCountryID", anCountryID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_d(string acTypeID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from SubTypes where acTypeID = @acTypeID ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@acTypeID", acTypeID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_w()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT S.*, Cy.acTitle as CountryTitle FROM SubTypes S ");
            sSQL.AppendLine("LEFT JOIN Countrys Cy ON Cy.anCountryID = S.anCountryID ");
            sSQL.AppendLine("ORDER BY S.acTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }

        public static List<SubType> Get(int anCountryID, string acTypeID)
        {
            DataTable dt = new DataTable();
            List<SubType> lResp = new List<SubType>();

            if (!String.IsNullOrEmpty(acTypeID) && (anCountryID > 0)) { dt = Get_d(anCountryID, acTypeID); }
            else if (anCountryID > 0) { dt = Get_d(anCountryID); }
            else if (!String.IsNullOrEmpty(acTypeID)) { dt = Get_d(acTypeID); }
            else { dt = Get_d(); }

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new SubType(Convert.ToInt32(r["ID"]),
                                            Convert.ToString(r["acTypeID"]),
                                            Convert.ToInt32(r["anCountryID"]),
                                            Convert.ToString(r["acTitle"]),
                                            Convert.ToDateTime(r["adModificationDate"]),
                                            Convert.ToInt32(r["anUserMod"]),
                                            Convert.ToInt32(r["anErpID"])
                                            ));
            }

            return lResp;
        }
        public static List<SubType> Set(SubType pData)
        {

            if (pData.ID > 0) { Update(pData); } else { Insert(pData); }

            return Get(pData.anCountryID, pData.acTypeID);
        }

        public static void Delete(SubType pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM SubTypes  ");
            sSQL.AppendLine("WHERE (ID = @ID) ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ID", pData.ID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Update(SubType pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("update SubTypes SET acTitle = @acTitle, adModificationDate = @adModificationDate, anUserMod = @anUserMod, anErpID = @anErpID ");
            sSQL.AppendLine("  ");
            sSQL.AppendLine("where id=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@id", pData.ID),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anErpID", pData.anErpID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Insert(SubType pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("INSERT INTO SubTypes (acTypeID, anCountryID, acTitle, anUserMod, anErpID)  ");
            sSQL.AppendLine(" VALUES (@acTypeID, @anCountryID, @acTitle, @anUserMod, @anErpID) ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@acTypeID", pData.acTypeID),
                        new SqlParameter("@anCountryID", pData.anCountryID),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anErpID", pData.anErpID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }

    }

}
