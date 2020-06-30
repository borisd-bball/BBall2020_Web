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
    public class Countrys
    {
        public class Country
        {
            public Country(int ID, int anCountryID, string acTitle, string acISOCode, string acCurrency, string acVATCodePrefix, int anIsEU, DateTime adModificationDate, int anUserMod, int anErpID)
            {
                this.ID = ID;
                this.anCountryID = anCountryID;
                this.acTitle = acTitle;
                this.acISOCode = acISOCode;
                this.acCurrency = acCurrency;
                this.acVATCodePrefix = acVATCodePrefix;
                this.anIsEU = anIsEU;
                this.adModificationDate = adModificationDate;
                this.anUserMod = anUserMod;
                this.anErpID = anErpID;
            }
            public int ID;
            public int anCountryID;
            public string acTitle;
            public string acISOCode;
            public string acCurrency;
            public string acVATCodePrefix;
            public int anIsEU;
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
            sSQL.AppendLine("SELECT  * FROM Countrys ");
            sSQL.AppendLine("ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
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
            sSQL.AppendLine("SELECT * FROM Countrys ");
            sSQL.AppendLine("WHERE anCountryID=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@id", anCountryID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable fGet_d(string acISOCode)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT * FROM Countrys ");
            sSQL.AppendLine("WHERE acISOCode=@acISOCode ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@acISOCode", acISOCode)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }

        public static int Get_MaxID()
        {
            int lResp = 0;

            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lR = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lR.Length > 0) { throw new Exception(lR); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select MAX(anCountryID) as maxValue from Countrys ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            if (lTmpDT.Rows.Count > 0) { lResp = (DBNull.Value.Equals(lTmpDT.Rows[0]["maxValue"])) ? 0 : Convert.ToInt32(lTmpDT.Rows[0]["maxValue"]); }


            return lResp;
        }

        public static List<Country> Get(int anCountryID)
        {
            DataTable dt = new DataTable();
            List<Country> lResp = new List<Country>();

            if (anCountryID > 0) { dt = Get_d(anCountryID); }
            else { dt = Get_d(); }

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new Country(Convert.ToInt32(r["ID"]),
                                            Convert.ToInt32(r["anCountryID"]),
                                            Convert.ToString(r["acTitle"]),
                                            Convert.ToString(r["acISOCode"]),
                                            Convert.ToString(r["acCurrency"]),
                                            Convert.ToString(r["acVATCodePrefix"]),
                                            Convert.ToInt32(r["anIsEU"]),
                                            Convert.ToDateTime(r["adModificationDate"]),
                                            Convert.ToInt32(r["anUserMod"]),
                                            Convert.ToInt32(r["anErpID"])
                                            ));
            }

            return lResp;
        }
        public static List<Country> Set(Country pData)
        {
            if (pData.ID > 0) { Update(pData); } else { Insert(pData); }

            return Get(pData.anCountryID);
        }
        public static Country fGet(string acISOCode)
        {
            DataTable dt = new DataTable();
            Country lResp = new Country(0,0,"","","","",0,DateTime.Now,0,0);

            dt = fGet_d(acISOCode);

            if (dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                lResp = (new Country(Convert.ToInt32(r["ID"]),
                                            Convert.ToInt32(r["anCountryID"]),
                                            Convert.ToString(r["acTitle"]),
                                            Convert.ToString(r["acISOCode"]),
                                            Convert.ToString(r["acCurrency"]),
                                            Convert.ToString(r["acVATCodePrefix"]),
                                            Convert.ToInt32(r["anIsEU"]),
                                            Convert.ToDateTime(r["adModificationDate"]),
                                            Convert.ToInt32(r["anUserMod"]),
                                            Convert.ToInt32(r["anErpID"])
                                            ));
            }

            return lResp;
        }

        public static void Delete(Country pData)
        {

            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM Countrys  ");
            sSQL.AppendLine("WHERE (anCountryID = @anCountryID) ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@anCountryID", pData.anCountryID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Update(Country pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("update Countrys SET acTitle = @acTitle, acISOCode = @acISOCode, acCurrency = @acCurrency, acVATCodePrefix = @acVATCodePrefix, anIsEU = @anIsEU, adModificationDate = @adModificationDate, anUserMod = @anUserMod, anErpID = @anErpID  ");
            sSQL.AppendLine("  ");
            sSQL.AppendLine("where anCountryID=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@id", pData.anCountryID),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@acISOCode", pData.acISOCode),
                        new SqlParameter("@acCurrency", pData.acCurrency),
                        new SqlParameter("@acVATCodePrefix", pData.acVATCodePrefix),
                        new SqlParameter("@anIsEU", pData.anIsEU),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anErpID", pData.anErpID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Insert(Country pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("INSERT INTO Countrys (anCountryID, acTitle, acISOCode, acCurrency, acVATCodePrefix, anIsEU, anUserMod, anErpID) ");
            sSQL.AppendLine(" VALUES (@anCountryID, @acTitle, @acISOCode, @acCurrency, @acVATCodePrefix, @anIsEU, @anUserMod, @anErpID) ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@anCountryID", pData.anCountryID),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@acISOCode", pData.acISOCode),
                        new SqlParameter("@acCurrency", pData.acCurrency),
                        new SqlParameter("@acVATCodePrefix", pData.acVATCodePrefix),
                        new SqlParameter("@anIsEU", pData.anIsEU),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anErpID", pData.anErpID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }

    }

}
