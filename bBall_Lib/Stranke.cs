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
    public class Stranke
    {
        public static Dictionary<string, string> Poste = new Dictionary<string, string>();
        public class Customer
        {
            public Customer(int ID, int anCustomerID, int anPostID, int anCountryID, string acTitle, string acShortTitle, string acAddress, string acPostTitle, string acVATPrefix, string acVATNumber, string acVATTypeID, string acTelephone, string acNote, string aceMaile, string acContactName, int anActive, DateTime adModificationDate, int anUserMod, int anErpID)
            {
                this.ID = ID;
                this.anCustomerID = anCustomerID;
                this.anPostID = anPostID;
                this.anCountryID = anCountryID;
                this.acTitle = acTitle;
                this.acShortTitle = acShortTitle;
                this.acAddress = acAddress;
                this.acPostTitle = acPostTitle;
                this.acVATPrefix = acVATPrefix;
                this.acVATNumber = acVATNumber;
                this.acVATTypeID = acVATTypeID;
                this.acTelephone = acTelephone;
                this.acNote = acNote;
                this.aceMaile = aceMaile;
                this.acContactName = acContactName;
                this.anActive = anActive;
                this.adModificationDate = adModificationDate;
                this.anUserMod = anUserMod;
                this.anErpID = anErpID;
            }
            public int ID;
            public int anCustomerID;
            public int anPostID;
            public int anCountryID;
            public string acShortTitle;
            public string acTitle;
            public string acAddress;
            public string acPostTitle;
            public string acVATPrefix;
            public string acVATNumber;
            public string acVATTypeID;
            public string acTelephone;
            public string acNote;
            public string aceMaile;
            public string acContactName;
            public int anActive;
            public DateTime adModificationDate;
            public int anUserMod;
            public int anErpID;
        }

        public static List<Customer> Seznam = new List<Customer>();
        public static void Nalozi()
        {
            foreach (Customer lc in Get(0))
            {
                osvezi_stranko(lc);
            }
        }
        private static void osvezi_stranko(Customer pData)
        {
            try
            {
                if (Seznam.Any(l => l.anCustomerID == pData.anCustomerID)) Seznam.Remove(Seznam.First(l => l.anCustomerID == pData.anCustomerID));
                Seznam.Add(new Customer(pData.ID, pData.anCustomerID, pData.anPostID, pData.anCountryID, pData.acTitle, pData.acShortTitle, pData.acAddress, pData.acPostTitle, pData.acVATPrefix, pData.acVATNumber,
                    pData.acVATTypeID, pData.acTelephone, pData.acNote, pData.aceMaile, pData.acContactName, pData.anActive, pData.adModificationDate, pData.anUserMod, pData.anErpID));
            }
            catch { }
        }
        public static DataTable Get_d()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM Customers ");
            sSQL.AppendLine("ORDER BY acShortTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_d(int id)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT * FROM Customers ");
            sSQL.AppendLine("WHERE anCustomerID=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@id", id)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_d(string acShortTitle, string acVATNumber)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT * FROM Customers ");
            sSQL.AppendLine("WHERE acShortTitle LIKE @acShortTitle and acVATNumber LIKE @acVATNumber ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@acShortTitle", acShortTitle),
                new SqlParameter("@acVATNumber", acVATNumber)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }
        public static string Get(string acShortTitle, string acVATNumber, int pRespType, string pOption)
        {
            string lResponse = "";

            DataTable dt = Get_d(acShortTitle, acVATNumber);
            if (pRespType == 0)
            {
                lResponse = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.SerializeDataTable_xml(dt, true);
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }

            return lResponse;
        }

        public static DataTable Get_w()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT C.*, P.acTitle as PostTitle, P.acCode as PostCode, Cy.acTitle as CountryTitle FROM Customers C ");
            sSQL.AppendLine("LEFT JOIN Posts P ON P.anPostID = C.anPostID ");
            sSQL.AppendLine("LEFT JOIN Countrys Cy ON Cy.anCountryID = C.anCountryID ");
            sSQL.AppendLine("ORDER BY C.acShortTitle asc ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            return lTmpDT;
        }
        public static DataTable Get_w(int id)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT C.*, P.acTitle as PostTitle, P.acCode as PostCode, Cy.acTitle as CountryTitle FROM Customers C ");
            sSQL.AppendLine("LEFT JOIN Posts P ON P.anPostID = C.anPostID ");
            sSQL.AppendLine("LEFT JOIN Countrys Cy ON Cy.anCountryID = C.anCountryID ");
            sSQL.AppendLine("WHERE C.anCustomerID=@id ");
            sSQL.AppendLine("ORDER BY C.acShortTitle asc ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@id", id)
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
            sSQL.AppendLine("select MAX(anCustomerID) as maxValue from Customers ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lSql.DisconnectSQL();

            if (lTmpDT.Rows.Count > 0) { lResp = (DBNull.Value.Equals(lTmpDT.Rows[0]["maxValue"])) ? 0 : Convert.ToInt32(lTmpDT.Rows[0]["maxValue"]); }


            return lResp;
        }

        public static List<Customer> Get(int anCustomerID)
        {
            DataTable dt = new DataTable();
            List<Customer> lResp = new List<Customer>();

            if (anCustomerID > 0) { dt = Get_d(anCustomerID); }
            else { dt = Get_d(); }

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new Customer(Convert.ToInt32(r["ID"]),
                                            Convert.ToInt32(r["anCustomerID"]),
                                            Convert.ToInt32(r["anPostID"]),
                                            Convert.ToInt32(r["anCountryID"]),
                                            Convert.ToString(r["acTitle"]),
                                            Convert.ToString(r["acShortTitle"]),
                                            Convert.ToString(r["acAddress"]),
                                            Convert.ToString(r["acPostTitle"]),
                                            Convert.ToString(r["acVATPrefix"]),
                                            Convert.ToString(r["acVATNumber"]),
                                             Convert.ToString(r["acVATTypeID"]),
                                            Convert.ToString(r["acTelephone"]),
                                            Convert.ToString(r["acNote"]),
                                            Convert.ToString(r["aceMaile"]),
                                            Convert.ToString(r["acContactName"]),
                                            Convert.ToInt32(r["anActive"]),
                                            Convert.ToDateTime(r["adModificationDate"]),
                                            Convert.ToInt32(r["anUserMod"]),
                                            Convert.ToInt32(r["anErpID"])
                                            ));
            }

            return lResp;
        }
        public static List<Customer> fGet(string acShortTitle, string acVATNumber)
        {
            DataTable dt = new DataTable();
            List<Customer> lResp = new List<Customer>();

            if (String.IsNullOrEmpty(acShortTitle)) { acShortTitle = "%"; }
            if (String.IsNullOrEmpty(acVATNumber)) { acVATNumber = "%"; }

            dt = Get_d(acShortTitle, acVATNumber);

            foreach (DataRow r in dt.Rows)
            {
                lResp.Add(new Customer(Convert.ToInt32(r["ID"]),
                                            Convert.ToInt32(r["anCustomerID"]),
                                            Convert.ToInt32(r["anPostID"]),
                                            Convert.ToInt32(r["anCountryID"]),
                                            Convert.ToString(r["acTitle"]),
                                            Convert.ToString(r["acShortTitle"]),
                                            Convert.ToString(r["acAddress"]),
                                            Convert.ToString(r["acPostTitle"]),
                                            Convert.ToString(r["acVATPrefix"]),
                                            Convert.ToString(r["acVATNumber"]),
                                            Convert.ToString(r["acVATTypeID"]),
                                            Convert.ToString(r["acTelephone"]),
                                            Convert.ToString(r["acNote"]),
                                            Convert.ToString(r["aceMaile"]),
                                            Convert.ToString(r["acContactName"]),
                                            Convert.ToInt32(r["anActive"]),
                                            Convert.ToDateTime(r["adModificationDate"]),
                                            Convert.ToInt32(r["anUserMod"]),
                                            Convert.ToInt32(r["anErpID"])
                                            ));
            }

            return lResp;
        }

        public static int Insert(Customer pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            int id = 0;
            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("declare @id int ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine("INSERT INTO Customers (anCustomerID, acShortTitle, acTitle, acAddress, anPostID, acPostTitle, acVATPrefix, acVATNumber, acVATTypeID, anCountryID, acTelephone, anActive, acNote, aceMaile, acContactName, adModificationDate, anUserMod, anErpID) ");
            sSQL.AppendLine("VALUES (@anCustomerID, @acShortTitle, @acTitle, @acAddress, @anPostID, @acPostTitle, @acVATPrefix, @acVATNumber, @acVATTypeID, @anCountryID, @acTelephone, @anActive, @acNote, @aceMaile, @acContactName, @adModificationDate, @anUserMod, @anErpID) ");
            sSQL.AppendLine("set @id=scope_identity() ");
            sSQL.AppendLine("select @id ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@anCustomerID", pData.anCustomerID),
                        new SqlParameter("@acShortTitle", pData.acShortTitle),
                        new SqlParameter("@anActive", pData.anActive),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@acAddress", pData.acAddress),
                        new SqlParameter("@acPostTitle", pData.acPostTitle),
                        new SqlParameter("@anPostID", pData.anPostID),
                        new SqlParameter("@acVATPrefix", pData.acVATPrefix),
                        new SqlParameter("@acVATNumber", pData.acVATNumber),
                        new SqlParameter("@acVATTypeID", pData.acVATTypeID),
                        new SqlParameter("@anCountryID", pData.anCountryID),
                        new SqlParameter("@acTelephone", pData.acTelephone),
                        new SqlParameter("@acNote", pData.acNote),
                        new SqlParameter("@aceMaile", pData.aceMaile),
                        new SqlParameter("@acContactName", pData.acContactName),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anErpID", pData.anErpID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams, out id);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

            osvezi_stranko(pData);

            return id;
        }
        public static void Update(Customer pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            // get data
            sSQL.Remove(0, sSQL.Length);
            sSQL.AppendLine("UPDATE Customers SET acShortTitle = @acShortTitle, acTitle = @acTitle, acAddress = @acAddress, anPostID = @anPostID, acPostTitle = @acPostTitle,  ");
            sSQL.AppendLine("    acVATPrefix=@acVATPrefix, acVATNumber = @acVATNumber, acVATTypeID = @acVATTypeID, anCountryID = @anCountryID, acTelephone=@acTelephone, aceMaile = @aceMaile, acContactName = @acContactName, ");
            sSQL.AppendLine("    anActive=@anActive, acNote=@acNote, adModificationDate=@adModificationDate, anUserMod=@anUserMod, anErpID=@anErpID ");
            sSQL.AppendLine("where anCustomerID=@id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@id", pData.anCustomerID),
                        new SqlParameter("@acShortTitle", pData.acShortTitle),
                        new SqlParameter("@anActive", pData.anActive),
                        new SqlParameter("@acTitle", pData.acTitle),
                        new SqlParameter("@acAddress", pData.acAddress),
                        new SqlParameter("@acPostTitle", pData.acPostTitle),
                        new SqlParameter("@anPostID", pData.anPostID),
                        new SqlParameter("@acVATPrefix", pData.acVATPrefix),
                        new SqlParameter("@acVATNumber", pData.acVATNumber),
                        new SqlParameter("@acVATTypeID", pData.acVATTypeID),
                        new SqlParameter("@anCountryID", pData.anCountryID),
                        new SqlParameter("@acTelephone", pData.acTelephone),
                        new SqlParameter("@acNote", pData.acNote),
                        new SqlParameter("@aceMaile", pData.aceMaile),
                        new SqlParameter("@acContactName", pData.acContactName),
                        new SqlParameter("@adModificationDate", pData.adModificationDate),
                        new SqlParameter("@anUserMod", pData.anUserMod),
                        new SqlParameter("@anErpID", pData.anErpID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }

            osvezi_stranko(pData);

        }
        public static void Delete(Customer pData)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM Customers  ");
            sSQL.AppendLine("WHERE (anCustomerID = @anCustomerID) ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@anCustomerID", pData.anCustomerID)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();

            if (lErr.Length > 0) { throw new Exception(lErr); }
            if (Seznam.Any(l => l.anCustomerID == pData.anCustomerID)) Seznam.Remove(Seznam.First(l => l.anCustomerID == pData.anCustomerID));

        }

    }
}
