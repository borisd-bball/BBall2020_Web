using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.SessionState;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace mr.bBall_Lib
{
    public class item
    {
        [XmlAttribute]
        public string key;
        [XmlAttribute]
        public string value;
    }

    public class Nastavitve
    {
        public static Dictionary<string, string> LicData = new Dictionary<string, string>();

        public static string Naziv
        {
            get { return Convert.ToString(Get("Common", "acNaziv", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acNaziv", 99999, ltmp);
            }
        }
        public static string NazivDolg
        {
            get { return Convert.ToString(Get("Common", "acNazivDolg", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acNazivDolg", 99999, ltmp);
            }
        }
        public static string Naslov
        {
            get { return Convert.ToString(Get("Common", "acNaslov", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acNaslov", 99999, ltmp);
            }
        }
        public static string Telefon
        {
            get { return Convert.ToString(Get("Common", "acTelefon", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acTelefon", 99999, ltmp);
            }
        }
        public static string EmailFrom
        {
            get { return Convert.ToString(Get("Common", "acEmailFrom", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acEmailFrom", 99999, ltmp);
            }
        }
        public static string Davcna
        {
            get { return Convert.ToString(Get("Common", "acDavcna", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acDavcna", 99999, ltmp);
            }
        }
        public static int Zavezanec
        {
            get { return Convert.ToInt32(Get("Common", "anZavezanec", "0")) ; }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "anZavezanec",99999, ltmp);
            }
        }
        public static string Trr
        {
            get { return Convert.ToString(Get("Common", "acTrr", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acTrr", 99999, ltmp);
            }
        }
        public static string Banka
        {
            get { return Convert.ToString(Get("Common", "acBanka", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acBanka", 99999, ltmp);
            }
        }
        public static int Obvescanje
        {
            get { return Convert.ToInt32(Get("Common", "anObvescanje", "0")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "anObvescanje", 99999, ltmp);
            }
        }
        public static int Osvezevanje
        {
            get { return Convert.ToInt32(Get("Common", "anOsvezevanje", "0")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "anOsvezevanje", 99999, ltmp);
            }
        }
        public static string SpletnaStran
        {
            get { return Convert.ToString(Get("Common", "acSpletnaStran", "")); }
            set
            {
                string ltmp = Convert.ToString(value);
                Set("Common", "acSpletnaStran", 99999, ltmp);
            }
        }

        public static void Nalozi()
        {
            LoadLicData();
        }
        public static string Get(string pGroup, string pKey, string pDefaultValue = "")
        {
            string lResponse = pDefaultValue;

            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lAppName = "Registrations";
            string lResp = lSql.ConnectSQL(lAppName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("select acKeyValue from _mrt_Settings ");
            sSQL.AppendLine("WHERE  acGroup = @acGroup AND acKey = @acKey ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        
                        new SqlParameter("@acGroup", pGroup),
                        new SqlParameter("@acKey", pKey)
                    };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            if (lTmpDT.Rows.Count > 0)
            {
                DataRow lrow = lTmpDT.Rows[0];
                lResponse = (lrow["acKeyValue"].ToString());
            }

            return lResponse;

        }
        public static void Set(string pGroup, string pKey, int pUserMod, string pKeyValue = "")
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("declare @acGroup varchar(50), @acKey varchar(200), @acKeyValue varchar(max), @tmpKeyValue varchar(max), @anUserMod int ");
            sSQL.AppendLine("SET @acGroup = @pGroup ");
            sSQL.AppendLine("SET @acKey = @pKey ");
            sSQL.AppendLine("SET @acKeyValue = @pKeyValue ");
            sSQL.AppendLine("SET @anUserMod = @pUserMod ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine("select @tmpKeyValue = acKeyValue from _mrt_Settings ");
            sSQL.AppendLine("WHERE acGroup = @acGroup AND acKey = @acKey ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine("IF @tmpKeyValue IS NULL ");
            sSQL.AppendLine(" BEGIN ");
            sSQL.AppendLine("   INSERT INTO _mrt_Settings (acGroup, acKey, acKeyValue, adTimeMod, anUserMod) ");
            sSQL.AppendLine("   VALUES (@acGroup, @acKey, @acKeyValue, GETDATE(), @anUserMod) ");
            sSQL.AppendLine(" END ");
            sSQL.AppendLine("ELSE ");
            sSQL.AppendLine(" BEGIN ");
            sSQL.AppendLine("   UPDATE _mrt_Settings SET acKeyValue = @acKeyValue, adTimeMod = GETDATE(), anUserMod = @anUserMod ");
            sSQL.AppendLine("   WHERE acGroup = @acGroup AND acKey = @acKey ");
            sSQL.AppendLine(" END ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@pGroup", pGroup),
                        new SqlParameter("@pKey", pKey),
                        new SqlParameter("@pKeyValue", pKeyValue),
                        new SqlParameter("@pUserMod", pUserMod)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static void LoadLicData()
        {
            try
            {
                string lLicData = Convert.ToString(Get("", "LicData")).Trim();
                if (String.IsNullOrEmpty(lLicData))
                {
                    LicData.Clear();

                    // Set Default data - Demo
                    LicData.Add("LicNumberCheck", "True");
                    LicData.Add("LicNumberValue", "10");
                    LicData.Add("CustomerID", ThumbPrint.Value());
                    LicData.Add("TimeLimitCheck", "True");
                    LicData.Add("TimeLimitDate", DateTime.Now.AddDays(30).ToString("yyyy-MM-ddT00:00:00"));
                    LicData.Add("AppRegSrv", "appreg.mr-avtomatika.com");
                    LicData.Add("RegCode", "DEMO");
                    LicData.Add("LicProgram", "s24WebPOS DEMO");
                    LicData.Add("naziv", "Nova DEMO");
                    LicData.Add("naziv_dolg", "");
                    LicData.Add("telefon", "");
                    LicData.Add("email", "");
                    LicData.Add("naslov", "");
                    LicData.Add("spletna_stran", "");
                    LicData.Add("zavezanec", "False");
                    LicData.Add("davcna", "");
                    LicData.Add("obvescanje", "False");
                    LicData.Add("osvezevanje", "False");
                    LicData.Add("Modules", "");

                    string lData = ExportLicData(LicData);
                    if (lData.Length > 0)
                    {
                        string lEncData = Varnost.EncryptAES256_Lic(lData);
                        SaveLicData(lEncData, 99999);
                    }
                }
                else
                {
                    string ldecData = Varnost.DecryptAES256_Lic(lLicData);
                    LicData.Clear();
                    LicData = ImportLicData(ldecData);
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("Get Settings: " + exception.Message);
            }

        }

        public static void SaveLicData(string pLicData, int pUserMod)
        {
            Set("", "LicData", pUserMod, pLicData);
        }
        private static string ExportLicData(Dictionary<string, string> pLicData)
        {
            string lResponse = "";
            StringWriter sw = new StringWriter();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(item[]),
                                 new XmlRootAttribute() { ElementName = "items" });

                serializer.Serialize(sw, pLicData.Select(kv => new item() { key = kv.Key, value = kv.Value }).ToArray());

                lResponse = sw.ToString();
                sw.Dispose();
            }
            catch
            { }

            return lResponse;
        }
        private static Dictionary<string, string> ImportLicData(string pLicData)
        {
            Dictionary<string, string> newDict = new Dictionary<string, string>(); ;

            try
            {
                XElement xElem2 = XElement.Parse(pLicData);

                newDict = xElem2.Descendants("item").ToDictionary(x => (string)x.Attribute("key"), x => (string)x.Attribute("value"));
            }
            catch
            { }

            return newDict;
        }

        public static string Get(string pGroup, string pKey, int pRespType, string pOption)
        {
            string lResponse = "";

            string dt = Convert.ToString(Get(pGroup, pKey));
            if (pRespType == 0)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", dt);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", dt);
            }

            return lResponse;
        }
        public static string Set(string pGroup, string pKey, int pUserMod, string pKeyValue, int pRespType, string pOption)
        {
            string lResponse = "";

            Set(pGroup, pKey, pUserMod, pKeyValue);
            //SaveLicData(pLicData, pUserMod);
            //LoadLicData();

            if (pRespType == 0)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }

            return lResponse;
        }

        public static string GetLicData(int pRespType, string pOption)
        {
            string lResponse = "";

            string dt = Convert.ToString(Get("", "LicData"));
            if (pRespType == 0)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", dt);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", dt);
            }

            return lResponse;
        }
        public static string SetLicData(int pUserMod, string pLicData, int pRespType, string pOption)
        {
            string lResponse = "";

            SaveLicData(pLicData, pUserMod);
            LoadLicData();

            if (pRespType == 0)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }

            return lResponse;
        }

        public static Boolean CheckRegCode(string pRegCode)
        {
            Boolean lResponse = false;

            try
            {
                string lrsp1 = ThumbPrint.Value();
                string lrsp2 = Varnost.GatSHA256Hash_S(lrsp1);
                if (lrsp2 == pRegCode) { lResponse = true; }
            }
            catch
            { }

            return lResponse;
        }
        public static string GetLicDataReport(int pRespType, string pOption)
        {
            string lResponse = "";

            string dt = GetLicDataReport();
            if (pRespType == 0)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", dt);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", dt);
            }

            return lResponse;
        }
        public static string GetLicDataReport()
        {
            string lResponse = "";

            Dictionary<string, string> lLicDataReport = new Dictionary<string, string>();

            if (Convert.ToBoolean(LicData["LicNumberCheck"])) { lLicDataReport.Add("LicNumbers", Convert.ToString(LicData["LicNumberValue"])); }
            else { lLicDataReport.Add("LicNumbers", Convert.ToString("UNLIMITED")); }

            if (Convert.ToBoolean(LicData["TimeLimitCheck"])) { lLicDataReport.Add("TimeLimits", Convert.ToString(LicData["TimeLimitDate"])); }
            else { lLicDataReport.Add("TimeLimits", Convert.ToString("UNLIMITED")); }

            if (CheckRegCode(LicData["RegCode"])) { lLicDataReport.Add("RegistrationCode", Convert.ToString("OK")); }
            else
            {
                if (LicData["RegCode"] == "DEMO") { lLicDataReport.Add("RegistrationCode", Convert.ToString("DEMO")); }
                else { lLicDataReport.Add("RegistrationCode", Convert.ToString("ERROR")); }
            }

            lLicDataReport.Add("LicProgram", Convert.ToString(LicData["LicProgram"]));
            lLicDataReport.Add("CustomerID", Convert.ToString(LicData["CustomerID"]));
            lLicDataReport.Add("naziv", Convert.ToString(LicData["naziv"]));
            lLicDataReport.Add("naziv_dolg", Convert.ToString(LicData["naziv_dolg"]));
            lLicDataReport.Add("telefon", Convert.ToString(LicData["telefon"]));
            lLicDataReport.Add("email", Convert.ToString(LicData["email"]));
            lLicDataReport.Add("naslov", Convert.ToString(LicData["naslov"]));
            lLicDataReport.Add("spletna_stran", Convert.ToString(LicData["spletna_stran"]));
            lLicDataReport.Add("zavezanec", Convert.ToString(LicData["zavezanec"]));
            lLicDataReport.Add("davcna", Convert.ToString(LicData["davcna"]));
            lLicDataReport.Add("obvescanje", Convert.ToString(LicData["obvescanje"]));
            lLicDataReport.Add("osvezevanje", Convert.ToString(LicData["osvezevanje"]));

            lResponse = ExportLicData(lLicDataReport);

            return lResponse;
        }

    }
}
