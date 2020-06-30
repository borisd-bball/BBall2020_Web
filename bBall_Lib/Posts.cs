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
    public class Posts
    {
        public class Post
        {
            public Post(int ID, int anPostID, int anCountryID, string acTitle, string acISOCode, string acCode, DateTime adModificationDate, int anUserMod, int anErpID)
            {
                this.ID = ID;
                this.anPostID = anPostID;
                this.anCountryID = anCountryID;
                this.acTitle = acTitle;
                this.acISOCode = acISOCode;
                this.acCode = acCode;
                this.adModificationDate = adModificationDate;
                this.anUserMod = anUserMod;
                this.anErpID = anErpID;
            }
            public int ID;
            public int anPostID;
            public int anCountryID;
            public string acTitle;
            public string acISOCode;
            public string acCode;
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
            sSQL.AppendLine("SELECT  * FROM Posts ");
            sSQL.AppendLine("ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_d(int anCountryID, int anPostID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from Posts where anCountryID=@id and anPostID=@anPostID  ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@id", anCountryID),
                new SqlParameter("@anPostID", anPostID)
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
            sSQL.AppendLine("select * from Posts where anCountryID=@id ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
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
        public static DataTable Get_d_postid(int anPostID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from Posts where anPostID=@id ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@id", anPostID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;

        }
        public static DataTable Get_d(string acTitle, string acCode)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select * from Posts where acTitle LIKE @acTitle and acCode LIKE @acCode  ORDER BY acTitle asc ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@acTitle", acTitle),
                new SqlParameter("@acCode", acCode)
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
            sSQL.AppendLine("SELECT P.*, Cy.acTitle as CountryTitle FROM Posts P ");
            sSQL.AppendLine("LEFT JOIN Countrys Cy ON Cy.anCountryID = P.anCountryID ");
            sSQL.AppendLine("ORDER BY P.acTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }


        public static List<Post> Get(int anCountryID, int anPostID)
        {
            DataTable dt = new DataTable();
            List<Post> lResp = new List<Post>();

            if (anCountryID > 0 && anPostID > 0) { dt = Get_d(anCountryID, anPostID); }
            else if (anCountryID > 0) { dt = Get_d(anCountryID); }
            else if (anPostID > 0) { dt = Get_d_postid(anPostID); }
            else { dt = Get_d(); }

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new Post(Convert.ToInt32(r["ID"]),
                                            Convert.ToInt32(r["anPostID"]),
                                            Convert.ToInt32(r["anCountryID"]),
                                            Convert.ToString(r["acTitle"]),
                                            Convert.ToString(r["acISOCode"]),
                                            Convert.ToString(r["acCode"]),
                                            Convert.ToDateTime(r["adModificationDate"]),
                                            Convert.ToInt32(r["anUserMod"]),
                                            Convert.ToInt32(r["anErpID"])
                                            ));
            }

            return lResp;
        }
        public static List<Post> fGet(string acTitle, string acCode)
        {
            DataTable dt = new DataTable();
            List<Post> lResp = new List<Post>();

            if (String.IsNullOrEmpty(acTitle)) { acTitle = "%"; }
            if (String.IsNullOrEmpty(acCode)) { acCode = "%"; }

            dt = Get_d(acTitle, acCode);

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new Post(Convert.ToInt32(r["ID"]),
                                            Convert.ToInt32(r["anPostID"]),
                                            Convert.ToInt32(r["anCountryID"]),
                                            Convert.ToString(r["acTitle"]),
                                            Convert.ToString(r["acISOCode"]),
                                            Convert.ToString(r["acCode"]),
                                            Convert.ToDateTime(r["adModificationDate"]),
                                            Convert.ToInt32(r["anUserMod"]),
                                            Convert.ToInt32(r["anErpID"])
                                            ));
            }

            return lResp;
        }

        public static int Get_MaxID()
        {
            int lResp = 0;
            string lErr;

            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            lErr = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select MAX(anPostID) as maxValue from Posts ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            if (lTmpDT.Rows.Count > 0) { lResp = (DBNull.Value.Equals(lTmpDT.Rows[0]["maxValue"])) ? 0 : Convert.ToInt32(lTmpDT.Rows[0]["maxValue"]); }


            return lResp;
        }

        public static void Delete(Post pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM Posts  ");
            sSQL.AppendLine("WHERE (anPostID = @ID) ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ID", pData.anPostID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Update(Post pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("update Posts SET acTitle = @acTitle, acISOCode = @acISOCode, acCode = @acCode,  ");
            sSQL.AppendLine(" adModificationDate = @adModificationDate, anUserMod = @anUserMod, anErpID = @anErpID ");
            sSQL.AppendLine("where id=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@id", pData.ID),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@acISOCode", pData.acISOCode),
                        new SqlParameter("@acCode", pData.acCode),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anErpID", pData.anErpID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void Insert(Post pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("INSERT INTO Posts (anPostID, anCountryID, acTitle, acISOCode, acCode, adModificationDate, anUserMod, anErpID)  ");
            sSQL.AppendLine(" VALUES (@anPostID, @anCountryID, @acTitle, @acISOCode, @acCode, @adModificationDate, @anUserMod, @anErpID) ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@anPostID", pData.anPostID),
                        new SqlParameter("@anCountryID", pData.anCountryID),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@acISOCode", pData.acISOCode),
                        new SqlParameter("@acCode", pData.acCode),
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
