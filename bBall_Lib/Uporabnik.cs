using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.SessionState;
using BCrypt.Net;

namespace mr.bBall_Lib
{
    public class Uporabnik : IDisposable
    {
        private cSQL lSql;
        private bool _logged_in = false;
        private int _id;
        private string _username = "";
        private string _ime = "";
        private string _priimek = "";
        private string _SessionID = "";
        private DateTime _LogedDate_created;
        private DateTime _ModifiedDate;
        private bool _active = false;
        private bool _admin = false;
        private bool _AppLogedIn = false;
        private string _pravice = "";
        private List<string> _skupine = new List<string>();
        private HttpSessionState session;
        private string _Email = "";
        private string _Gsm = "";
        private bool _DesktopApp = false;

        public int Id { get { return _id; } }
        public string Username { get { return _username; } }
        public string Ime { get { return _ime; } }
        public string Priimek { get { return _priimek; } }
        public string SessionID { get { return _SessionID; } }
        public DateTime LogedDate_created { get { return _LogedDate_created; } }
        public DateTime ModifiedDate { get { return _ModifiedDate; } }
        public bool Active { get { return _active; } }
        public bool Admin { get { return _admin; } }
        public bool AppLogedIn { get { return _AppLogedIn; } }
        public bool LoggedIn { get { return _logged_in; } }
        public string Pravice { get { return _pravice; } }
        public List<string> PraviceL { get { return Get_UserRolse(_pravice); } }
        public List<string> Skupine { get { return _skupine; } }
        public string Email { get { return _Email; } }
        public string Gsm { get { return _Gsm; } }
        public bool DesktopApp { get { return _DesktopApp; } }
        public Uporabnik()
        {
            cSettings.GetSettings("");
            lSql = new cSQL();
        }
        public Uporabnik(string username, string password, string pDivID, int pForceLogin) : this()
        {
            cSettings.GetSettings("");
            string lDT = "";
            login(username, password, pDivID, pForceLogin, out lDT);
        }
        public Uporabnik(HttpSessionState s) : this()
        {
            cSettings.GetSettings("");

            _id = Convert.ToInt32(s["id"]);
            _username = Convert.ToString(s["username"]);
            _ime = Convert.ToString(s["ime"]);
            _priimek = Convert.ToString(s["priimek"]);
            _LogedDate_created = Convert.ToDateTime(s["LogedDate_created"]);
            _ModifiedDate = Convert.ToDateTime(s["ModifiedDate"]);
            _SessionID = Convert.ToString(s["SessionID"]);
            _active = Convert.ToBoolean(s["active"]);
            _admin = Convert.ToBoolean(s["admin"]);
            _AppLogedIn = Convert.ToBoolean(s["AppLogedIn"]);
            _pravice = Convert.ToString(s["pravice"]);
            _skupine = ((List<string>)s["skupine"]) ?? new List<string>();
            _logged_in = Convert.ToBoolean(s["logged_in"]);
            _Email = Convert.ToString(s["Email"]);
            _Gsm = Convert.ToString(s["Gsm"]);
            session = s;
        }
        public string login(string username, string password, string pDivID, int pForceLogin, int pRespType, string pOption)
        {
            string lResponse = "";
            string lDT = "";

            //Nastavitve.LoadLicData();

            int lR = login(username, password, pDivID, pForceLogin, out lDT);

            if (pRespType == 0)
            {
                lResponse = lDT;
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, lR, Splosno.GetTranslateByID(lR), lResponse);
            }
            else if (pRespType == 1)
            {
                if (lDT.Length > 0)
                {
                    DataTable dt = Splosno.DeserializeDataTable_json(lDT);
                    lResponse = Splosno.SerializeDataTable_xml(dt);
                }

                lResponse = Splosno.AddHeadDataToResponseData(pRespType, lR, Splosno.GetTranslateByID(lR), lResponse);
            }

            return lResponse;
        }
        public static int login(string username, string password, HttpSessionState s, string pDivID, int pForceLogin)
        {
            s.Clear();
            int res = -1;
            string lDT = "";

            using (Uporabnik u = new Uporabnik())
            {
                res = u.login(username, password, pDivID, pForceLogin, out lDT);
                if (res == 0)
                {
                    u.setSession(s);
                }
            }
            return res;
        }
        private int login(string username, string password, string pDivID, int pForceLogin, out string pDT)
        {
            int lResponse = -1;
            pDT = "";
            string lUsername, lpassword;
            _DesktopApp = true;

            lUsername = username;
            lpassword = password;

            // Preveri ali se prijavlja servis, ali uporabnik
            if ((lUsername.Length == 0) && (pDivID.Length > 0))
            {
                _DesktopApp = false;
                int lUserID = Uporabniki.Get_UserID(pDivID);
                if (lUserID > 0) { Uporabniki.Get_UserLoginData(lUserID, out lUsername, out lpassword); }
            }

            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            if (String.IsNullOrEmpty(lpassword)) { lpassword = ""; }
            sSQL.AppendLine(" ");
            sSQL.AppendLine("SELECT * FROM _mrt_Users ");
            sSQL.AppendLine("WHERE  acUserName = @username and acPassword = @password ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                    
                    new SqlParameter("@username", lUsername),
                    new SqlParameter("@password", lpassword)
                };

            string lErr = "";
            DataTable lDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();

            if (lDT.Rows.Count > 0)
            {
                DataRow lrow = lDT.Rows[0];
                int lUserID = Convert.ToInt32(lrow["anUserID"]);
                string lUserSessionID = (lrow["acSesionID"].ToString());
                bool lUserActive = Convert.ToBoolean(lrow["anActive"]);
                bool lAppLogedIn = Convert.ToBoolean(lrow["anAppLogedIn"]);
                bool lLogedIn = Convert.ToBoolean(lrow["anLogedIn"]);

                pDT = Splosno.SerializeDataTable_json(lDT);

                if (lUserActive == true)
                {
                    int maxLic;
                    //int.TryParse(Nastavitve.Get("", "LicNumber", "0"), out maxLic);
                    int.TryParse(Nastavitve.LicData["LicNumberValue"], out maxLic);
                    int currLogedUsers = Uporabniki.Get_LoggedUsers();
                    bool lCheckLicNumber = Convert.ToBoolean(Nastavitve.LicData["LicNumberCheck"]);

                    if ((!lCheckLicNumber) || (maxLic >= (currLogedUsers + 1)))
                    {
                        if ((lAppLogedIn != true && !lLogedIn) || (pForceLogin == 1))
                        {
                            string lNewSessionID = Guid.NewGuid().ToString();

                            // Če je uporabnik prijavljen v drugem sistemu, ohrani sesionID, da ga ne vržemo ven!
                            //if ((lLogedIn) || (lAppLogedIn)) { lNewSessionID = lUserSessionID; }

                            Set_UserUpdate(lUserID, true, lNewSessionID, _DesktopApp);

                            // Ponovno pridobi podatke o userju z novim sessionid-jem
                            DataTable lDT1 = Uporabniki.Get_UserBySession(lUsername, lNewSessionID);
                            pDT = Splosno.SerializeDataTable_json(lDT1);

                            lSql.ConnectSQL(Splosno.AppSQLName);
                            sSQL.Remove(0, sSQL.Length);
                            sSQL.AppendLine("SELECT UR.acRoleID FROM _mrt_UserRoles UR WITH(NOLOCK) ");
                            sSQL.AppendLine("WHERE (UR.acUserName = @UserName) ");

                            sqlParams = null;
                            sqlParams = new SqlParameter[] {
                                new SqlParameter("@UserName", lUsername)
                            };

                            lDT = null;
                            lDT = lSql.FillDT(sSQL, sqlParams, out lErr);
                            lSql.DisconnectSQL();

                            if (lDT.Rows.Count > 0)
                            {
                                _pravice = Splosno.SerializeDataTable_json(lDT);
                            }
                            else
                            {
                                _pravice = "[]";
                            }

                            lrow = lDT1.Rows[0];
                            _id = Convert.ToInt32(lrow["anUserID"]);
                            _username = Convert.ToString(lrow["acUserName"]);
                            _ime = Convert.ToString(lrow["acFirstName"]);
                            _priimek = Convert.ToString(lrow["acLastName"]);
                            _ModifiedDate = Convert.ToDateTime(lrow["adTimeMod"]);
                            _active = Convert.ToBoolean(lrow["anActive"]);
                            _AppLogedIn = Convert.ToBoolean(lrow["anAppLogedIn"]);
                            _logged_in = Convert.ToBoolean(lrow["anLogedIn"]);
                            _SessionID = Convert.ToString(lrow["acSesionID"]);
                            _Email = Convert.ToString(lrow["acEmail"]);
                            _Gsm = Convert.ToString(lrow["acGSM"]);
                            _admin = Convert.ToBoolean(lrow["anAdmin"]);
                            lResponse = 0;
                        }
                        else
                        {
                            lResponse = 103; //Uporabnik je že prijavljen!
                        }

                    }
                    else
                    {
                        lResponse = 102;
                    }
                }
                else
                {
                    lResponse = 101;
                }
            }
            else
            {
                lResponse = 100;
            }

            return lResponse;
        }

        private void setSession(HttpSessionState s)
        {
            s["id"] = _id;
            s["username"] = _username;
            s["ime"] = _ime;
            s["priimek"] = _priimek;
            s["ModifiedDate"] = _ModifiedDate;
            s["AppLogedIn"] = _AppLogedIn;
            s["logged_in"] = _logged_in;
            s["active"] = _active;
            s["pravice"] = _pravice;
            s["skupine"] = _skupine;
            s["SessionID"] = _SessionID;
            s["Email"] = _Email;
            s["Gsm"] = _Gsm;
            s["admin"] = _admin;
        }
        public void logout(int pUserID, string pSessionID, HttpSessionState s, string pDivID)
        {
            string lSessionID = null;
            // Če sta prijavljena Client in Desktop, potem pusti SessionId, da ga ne vržeš ven.
            if (_AppLogedIn && _logged_in) { lSessionID = _SessionID; }

            Set_UserUpdate(pUserID, false, lSessionID, _DesktopApp);
            logout(s);
        }
        public void logout(HttpSessionState s)
        {
            logout();
            ClearSession(s);
        }
        private void logout()
        {
            _logged_in = false;
            _id = 0;
            _username = "";
            _ime = "";
            _priimek = "";
            _ModifiedDate = DateTime.MinValue;
            _AppLogedIn = false;
            _pravice = "";
            _DesktopApp = false;
            _skupine.Clear();
            _SessionID = "";
            _Email = "";
            _Gsm = "";
            _admin = false;
            _active = false;

        }
        public void ClearSession(HttpSessionState s)
        {
            if (s != null)
            {
                s.Clear();
                s.Abandon();
            }
        }
        public void Dispose()
        {
            lSql.DisconnectSQL();
        }

        private int Registration(string username, string password, string pDivID, string pName, string pEmail, string pUserRights, out string pDT)
        {
            int lResponse = -1;
            pDT = "";
            string lUsername, lpassword;

            lUsername = username;
            lpassword = password;

            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            if (String.IsNullOrEmpty(lpassword)) { lpassword = ""; }
            sSQL.AppendLine(" ");
            sSQL.AppendLine("SELECT * FROM _mrt_Users ");
            sSQL.AppendLine("WHERE  acUserName = @username ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {

                    new SqlParameter("@username", lUsername)
                };

            string lErr = "";
            DataTable lDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();

            if (lDT.Rows.Count == 0)
            {
                int _id = Uporabniki.Add(username, password, pName, "", true, pUserRights, pEmail, "", false, 99999);
                if (_id > 0)
                {
                    lResponse = 0;
                }
            }
            else
            {
                lResponse = 150;
            }

            return lResponse;
        }
        public string Registration(string username, string password, string pDivID, string pName, string pEmail, string pUserRights, int pRespType, string pOption)
        {
            string lResponse = "";
            string lDT = "";

            //Nastavitve.LoadLicData();

            int lR = Registration(username, password, pDivID, pName, pEmail, pUserRights, out lDT);

            if (pRespType == 0)
            {
                lResponse = lDT;
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, lR, Splosno.GetTranslateByID(lR), lResponse);
            }
            else if (pRespType == 1)
            {
                if (lDT.Length > 0)
                {
                    DataTable dt = Splosno.DeserializeDataTable_json(lDT);
                    lResponse = Splosno.SerializeDataTable_xml(dt);
                }

                lResponse = Splosno.AddHeadDataToResponseData(pRespType, lR, Splosno.GetTranslateByID(lR), lResponse);
            }

            return lResponse;
        }



        public int Set_UserUpdate(int pUserID, bool pLogedIn, string pSessionID, bool pDesktopApp)
        {
            int lResponse = 0;
            string lResp = lSql.ConnectSQL("s24WebPOS");
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();

            if (pDesktopApp)
            {
                sSQL.AppendLine("UPDATE _mrt_Users SET anAppLogedIn = @anAppLogedIn, acSesionID = @acSesionID, adTimeMod = GETDATE()  ");
                sSQL.AppendLine("WHERE  anUserID = @anUserID ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    
                    new SqlParameter("@anUserID", pUserID),
                    new SqlParameter("@anAppLogedIn", pLogedIn),
                    new SqlParameter("@acSesionID", pSessionID)
                };

                lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                lSql.DisconnectSQL();
                if (lResp.Length == 0) { lResponse = 1; }
            }
            else
            {
                sSQL.AppendLine("UPDATE _mrt_Users SET anLogedIn = @anLogedIn, acSesionID = @acSesionID, adTimeMod = GETDATE()  ");
                sSQL.AppendLine("WHERE  anUserID = @anUserID ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    
                    new SqlParameter("@anUserID", pUserID),
                    new SqlParameter("@anLogedIn", pLogedIn),
                    new SqlParameter("@acSesionID", pSessionID)
                };

                lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                lSql.DisconnectSQL();
                if (lResp.Length == 0) { lResponse = 1; }
            }

            return lResponse;
        }
        private List<string> Get_UserRolse(string pUR)
        {
            List<string> lRet = new List<string>();

            DataTable lDT = Splosno.DeserializeDataTable_json(pUR);
            foreach (DataRow r in lDT.Rows)
            {
                string lRoleID = Convert.ToString(r["acRoleID"]);
                //string lUN = Convert.ToString(r["acUserName"]);
                lRet.Add(lRoleID.ToString());
            }

            return lRet;
        }
    }
}
