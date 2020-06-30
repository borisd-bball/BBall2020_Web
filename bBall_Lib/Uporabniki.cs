using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.SessionState;

namespace mr.bBall_Lib
{
    public class Uporabniki
    {
        public static DataTable Get()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM _mrt_Users ");
            sSQL.AppendLine(" ");

            //SqlParameter[] sqlParams = new SqlParameter[] {
            //            new SqlParameter("@acSystem", ConfigurationManager.AppSettings["systemID"])
            //        };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }
        public static DataTable Get(int id)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM _mrt_Users ");
            sSQL.AppendLine("WHERE  anUserID = @id ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                
                new SqlParameter("@id", id)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }

        public static string Get(int pID, int pRespType, string pOption)
        {
            string lResponse = "";

            DataTable dt = Get(pID);
            if (pRespType == 0)
            {
                lResponse = Splosno.SerializeDataTable_json(dt);
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.SerializeDataTable_xml(dt, false);
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }

            return lResponse;
        }
        public static string Get(int pRespType, string pOption)
        {
            string lResponse = "";

            DataTable dt = Get();
            if (pRespType == 0)
            {
                lResponse = Splosno.SerializeDataTable_json(dt);
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }
            else if (pRespType == 1)
            {
                lResponse = Splosno.SerializeDataTable_xml(dt, false);
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }

            return lResponse;
        }

        public static string Add(string pUserName, string pPassword, string pFirstName, string pLastName, bool pActive, string pPravice, string pEmail, string pGsm, bool pAdmin, int pModifUser, int pRespType, string pOption)
        {
            string lResponse = "";

            int lID = Add(pUserName, pPassword, pFirstName, pLastName, pActive, pPravice, pEmail, pGsm, pAdmin, pModifUser);
            if (pRespType == 0)
            {
                lResponse = lID.ToString();
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }
            else if (pRespType == 1)
            {
                lResponse = lID.ToString();
                lResponse = Splosno.AddHeadDataToResponseData(pRespType, 0, "", lResponse);
            }

            return lResponse;
        }
        public static int Add(string pUserName, string pPassword, string pFirstName, string pLastName, bool pActive, string pPravice, string pEmail, string pGsm, bool pAdmin, int pModifUser)
        {
            foreach (DataRow r in Get().Rows)
            {
                string username_check = Convert.ToString(r["acUserName"]).ToLower();
                if (username_check == pUserName.ToLower()) throw new Exception("Uporabnik s tem uporabniškim imenom že obstaja");
            }
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            int id = 0;
            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("declare @id int ");
            sSQL.AppendLine(" ");
            sSQL.AppendLine("INSERT INTO [_mrt_Users] ([acUserName], [acPassword], [acFirstName], [acLastName], [anActive], [adTimeMod], anUserMod, [anAdmin], acEmail, acGsm) ");
            sSQL.AppendLine(" VALUES (@UserName, @Password, @FirstName, @LastName, @Active, @ModifiedDate, @anUserMod, @anAdmin, @Email, @Gsm) ");
            sSQL.AppendLine("set @id=scope_identity() ");
            sSQL.AppendLine("select @id ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        
                        new SqlParameter("@UserName", pUserName),
                        new SqlParameter("@Password", pPassword),
                        new SqlParameter("@FirstName", pFirstName),
                        new SqlParameter("@LastName", pLastName),
                        new SqlParameter("@Active", pActive),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@anAdmin", pAdmin),
                        new SqlParameter("@anUserMod", pModifUser),
                        new SqlParameter("@Email", pEmail),
                        new SqlParameter("@Gsm", pGsm)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams, out id);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            //if (id > 0 && pPravice.Length > 0) // add user rolse
            //{
            //    Add_UserRoles(pPravice, pUserName, pModifUser);
            //}

            return id;
        }

        public static string Edit(int id, string pUserName, string pPassword, string pFirstName, string pLastName, bool pActive, bool pAdmin, string pPravice, HttpSessionState s, string pEmail, string pGsm, int pModifUser, int pRespType, string pOption)
        {
            string lResponse = "";

            Edit(id, pUserName, pPassword, pFirstName, pLastName, pActive, pAdmin, pPravice, s, pEmail, pGsm, pModifUser);
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
        public static void Edit(int id, string pUserName, string pPassword, string pFirstName, string pLastName, bool pActive, bool pAdmin, string pPravice, HttpSessionState s, string pEmail, string pGsm, int pModifUser)
        {
            foreach (DataRow r in Get().Rows)
            {
                int id_check = Convert.ToInt32(r["anUserID"]);
                if (id_check != id)
                {
                    string username_check = Convert.ToString(r["acUserName"]).ToLower();
                    if (username_check == pUserName.ToLower()) throw new Exception("Uporabnik s tem uporabniškim imenom že obstaja");
                }
            }

            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("UPDATE [_mrt_Users] SET [acFirstName] = @FirstName, [acLastName] = @LastName, anUserMod = @anUserMod, acEmail = @Email, acGSM = @Gsm, ");
            sSQL.AppendLine(" [anActive] = @Active, [adTimeMod] = @ModifiedDate, [anAdmin] = @anAdmin ");
            sSQL.AppendLine("WHERE  anUserID = @ID ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        
                        new SqlParameter("@UserName", pUserName),
                        new SqlParameter("@FirstName", pFirstName),
                        new SqlParameter("@LastName", pLastName),
                        new SqlParameter("@anUserMod", pModifUser),
                        new SqlParameter("@Active", pActive),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@anAdmin", pAdmin),
                        new SqlParameter("@ID", id),
                        new SqlParameter("@Email", pEmail),
                        new SqlParameter("@Gsm", pGsm)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            if (!string.IsNullOrWhiteSpace(pPassword))
            {
                lResp = lSql.ConnectSQL(Splosno.AppSQLName);
                if (lResp.Length > 0) { throw new Exception(lResp); }

                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("UPDATE [_mrt_Users] SET [acPassword] = @Password ");
                sSQL.AppendLine("WHERE  anUserID = @ID ");
                sSQL.AppendLine(" ");

                SqlParameter[] sqlParams1 = new SqlParameter[] {
                        
                        new SqlParameter("@Password", pPassword),
                        new SqlParameter("@ID", id)
                    };

                lErr = lSql.ExecuteQuery(sSQL, sqlParams1);
                lSql.DisconnectSQL();
                if (lErr.Length > 0) { throw new Exception(lErr); }
            }

            if (s != null && Convert.ToInt32(s["id"]) == id)
            {
                s["ime"] = pFirstName;
                s["priimek"] = pLastName;
                s["Email"] = pEmail;
                s["Gsm"] = pGsm;
            }

            Add_UserRoles(pPravice, pUserName, pModifUser);

        }
        public static void Delete(int id)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM [_mrt_Users]  ");
            sSQL.AppendLine("WHERE  anUserID = @ID ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                        
                        new SqlParameter("@ID", id)
                    };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

        }
        public static string Delete(int pID, int pRespType, string pOption)
        {
            string lResponse = "";

            Delete(pID);
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

        public static DataTable Get_URoles()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM _mrt_URoles ");
            sSQL.AppendLine(" ");

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }
        public static DataTable Get_UserRoles()
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM _mrt_UserRoles ");
            sSQL.AppendLine(" ");

            //SqlParameter[] sqlParams = new SqlParameter[] {
            //            new SqlParameter("@acSystem", ConfigurationManager.AppSettings["systemID"])
            //        };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }
        public static DataTable Get_UserRoles(int id)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM _mrt_UserRoles ");
            sSQL.AppendLine("WHERE  ID = @ID ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                
                new SqlParameter("@ID", id)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }
        public static DataTable Get_UserRoles(string pusername)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM _mrt_UserRoles ");
            sSQL.AppendLine("WHERE  acUserName = @acUserName ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {

                new SqlParameter("@acUserName", pusername)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }

        public static DataTable Get_UserRoles(string pRoleID, string pUserName)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT  * FROM _mrt_UserRoles ");
            sSQL.AppendLine("WHERE acRoleID = @RoleID and acUserName = @UserName ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@RoleID", pRoleID),
                new SqlParameter("@UserName", pUserName)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }
        public static void Delete_UserRoles(string pRoleID, string pUserName)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM _mrt_UserRoles ");
            sSQL.AppendLine("WHERE acRoleID = @RoleID and acUserName = @UserName ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@RoleID", pRoleID),
                new SqlParameter("@UserName", pUserName)
            };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return;
        }
        public static void Delete_UserRoles(string pUserName)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("DELETE FROM _mrt_UserRoles ");
            sSQL.AppendLine("WHERE acUserName = @UserName ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@UserName", pUserName)
            };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return;
        }
        public static void Add_UserRole(string pRoleID, string pUserName, int pModifUser)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("INSERT INTO _mrt_UserRoles (acRoleID, acUserName, anUserMod, adTimeMod) ");
            sSQL.AppendLine("VALUES (@RoleID, @UserName, @ModifiedUser, GETDATE() ) ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@RoleID", pRoleID),
                new SqlParameter("@UserName", pUserName),
                new SqlParameter("@ModifiedUser", pModifUser)
            };

            string lErr = lSql.ExecuteQuery(sSQL, sqlParams);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return;
        }
        public static void Add_UserRoles(string pUserRoles, string pUserName, int pModifUser)
        {
            string[] lDT = pUserRoles.Split(',');
            if (lDT == null) return;

            if (lDT.Length > 0)
            {
                // Delate all user rolse
                Delete_UserRoles(pUserName);

                // Add all user roles
                foreach (string s in lDT)
                {
                    string lRoleID = s.Trim();
                    if (s.Length > 0)
                    {
                        Add_UserRole(lRoleID, pUserName, pModifUser);
                    }
                }
            }

            return;
        }
        public static int Get_UserID(string pDivID)
        {
            int lResponse = 0;

            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("Select ISNULL(anUserID,0) as anUserID FROM _mrt_Clients ");
            sSQL.AppendLine("WHERE  acClientid = @acClientid and anIsActive = 1 ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                
                new SqlParameter("@acClientid", pDivID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            if (lTmpDT.Rows.Count > 0)
            {
                DataRow lrow = lTmpDT.Rows[0];
                lResponse = Convert.ToInt32(lrow["anUserID"]);
            }

            return lResponse;
        }
        public static void Get_UserLoginData(int pUserID, out string pUserName, out string pUserPassword)
        {
            pUserName = "";
            pUserPassword = "";

            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("Select * FROM _mrt_Users ");
            sSQL.AppendLine("WHERE  anUserID = @anUserID ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                
                new SqlParameter("@anUserID", pUserID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            if (lTmpDT.Rows.Count > 0)
            {
                DataRow lrow = lTmpDT.Rows[0];
                pUserName = Convert.ToString(lrow["acUserName"]).Trim();
                pUserPassword = Convert.ToString(lrow["acPassword"]).Trim();
            }

        }
        public static int Check_UserAccess(string pHash, int pUserID, string pModulGroup, string pModul, Boolean pChLic = true)
        {
            int lResponse = 0;

            // Preveri Klasiko SesionID in UserID
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT * FROM _mrt_Users ");
            sSQL.AppendLine("WHERE  anUserID = @anUserID and acSesionID = @acSesionID ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                
                new SqlParameter("@acSesionID", pHash),
                new SqlParameter("@anUserID", pUserID)
            };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            if (lTmpDT.Rows.Count > 0)
            {
                lResponse = 1;
            }

            //if ((lResponse == 1) && (pChLic))
            //{
            //    // Preveri če je pravica do modula
            //    string lCurrModul = pModulGroup + "--" + pModul;
            //    string[] lModules = Convert.ToString(Nastavitve.LicData["Modules"]).Split(',');
            //    if (!lModules.Contains(lCurrModul)) { return 0; }
            //}

            return lResponse;
        }
        public static DataTable Get_UserBySession(string pUsername, string pSessionID)
        {
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT * FROM _mrt_Users");
            sSQL.AppendLine("WHERE  acUserName = @acUserName and acSesionID = @acSesionID ");
            sSQL.AppendLine(" ");

            SqlParameter[] sqlParams = new SqlParameter[] {
                    
                    new SqlParameter("@acUserName", pUsername),
                    new SqlParameter("@acSesionID", pSessionID)
                };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, sqlParams, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            return lTmpDT;
        }
        public static int Get_LoggedUsers()
        {
            int lresponse = 0;
            cSettings.GetSettings("");
            cSQL lSql = new cSQL();
            string lResp = lSql.ConnectSQL(Splosno.AppSQLName);
            if (lResp.Length > 0) { throw new Exception(lResp); }

            StringBuilder sSQL = new StringBuilder();
            sSQL.AppendLine("SELECT * ");
            sSQL.AppendLine("FROM _mrt_Users (NOLOCK) WHERE  anLogedIn = 1 ");
            sSQL.AppendLine(" ");

            //SqlParameter[] sqlParams = new SqlParameter[] {
            //        new SqlParameter("@acSystem", ConfigurationManager.AppSettings["systemID"])
            //    };

            string lErr;
            DataTable lTmpDT = lSql.FillDT(sSQL, out lErr);
            lSql.DisconnectSQL();
            if (lErr.Length > 0) { throw new Exception(lErr); }

            lresponse = lTmpDT.Rows.Count;

            return lresponse;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
