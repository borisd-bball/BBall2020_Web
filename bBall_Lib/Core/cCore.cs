using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mr.bBall_Lib
{
    public class cCore
    {
        cSQL lSql;

        public bool Connect()
        {
            lSql = new cSQL();
            lSql.ConnectSQL("s24Registrations");
            return true;
        }

        public cCore()
        {
            Connect();
        }

        public void Disconnect()
        {
            lSql.DisconnectSQL();
        }


        private string GetRandom()
        {
            string lResult = "_";
            Random getrandom = new Random();
            lResult = lResult + getrandom.Next(0, 1000000000).ToString();
            return lResult;

        }

        public int CheckAccess(string pHash, string pUserName)
        {
            int lResponse = 0;
            try
            {
                string ltmpName = Guid.NewGuid().ToString();
                StringBuilder sSQL = new StringBuilder();

                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_queryFindUserByUserNameSessionID @username, @sessionid ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");

                SqlParameter[] sqlParams2 = new SqlParameter[] {
                    new SqlParameter("@username", pUserName),
                    new SqlParameter("@sessionid", pHash)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams2, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    lResponse = 1;
                }


            }
            catch (Exception exception)
            {
                cLog.WriteError("[CheckAccess] " + exception.Message);
            }

            return lResponse;
        }

        public string Login(string pUser, string pPassword, int pStoreID, string pDivID, int pForceLogin, int pDesktopApp, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                if (String.IsNullOrEmpty(pPassword)) { pPassword = ""; }
                // get data
                sSQL.Remove(0, sSQL.Length);

                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_queryFindUserByUserNamePassword @username, @password ");
                sSQL.AppendLine(" ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@username", pUser),
                    new SqlParameter("@password", pPassword)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    DataRow lrow = lTmpDS.Tables[ltmpName].Rows[0];
                    string lUserSessionID = (lrow["SessionID"].ToString());
                    bool lUserActive = (bool)(lrow["Active"]);
                    bool lAppLogedIn = (bool)(lrow["AppLogedIn"]);
                    bool lLogedIn = (bool)(lrow["LogedIn"]);
                    int lLocationID = (int)(lrow["LocationID"]);
                    bool lAppActive = (bool)(lrow["AppActive"]);

                    // Pridobimo podatke od Userja v serializirani obliki
                    string lUsesrData = "";
                    if (pRespType == 0)
                    {
                        lUsesrData = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lUsesrData = sw.ToString();
                    }

                    if (lUserActive == true)
                    {
                        int maxLic;
                        int.TryParse(GetWMSSettingsByName(pStoreID, "LicNumber", 0), out maxLic);
                        int currLogedUsers = GetWMSLoggedUsers();

                        if (maxLic <= (currLogedUsers + 1))
                        {
                            if ((pDesktopApp != 1) || (lAppActive == true))
                            {
                                if (((lAppLogedIn != true) || (pDesktopApp == 1 && !lAppLogedIn)) || (pForceLogin == 1))
                                {
                                    string lNewSessionID = Guid.NewGuid().ToString();

                                    Set_UserUpdate(pUser, 1, lNewSessionID, pDivID, pDesktopApp, DateTime.Now);

                                    // Ponovno pridobi podatke o userju z novim sessionid-jem
                                    lUsesrData = Get_UserBySession(pUser, lNewSessionID, pRespType);

                                    sSQL.Remove(0, sSQL.Length);
                                    sSQL.AppendLine("SELECT UserRoles.RoleID, UserRoles.UserName, URoles.TerminalRole FROM UserRoles WITH(NOLOCK) INNER JOIN ");
                                    sSQL.AppendLine(" URoles ON UserRoles.RoleID = URoles.RoleID ");
                                    sSQL.AppendLine("WHERE (UserRoles.UserName = @UserName) ");

                                    sqlParams = null;
                                    sqlParams = new SqlParameter[] {
                                    new SqlParameter("@UserName", pUser)
                                };

                                    ltmpName = Guid.NewGuid().ToString();
                                    lTmpDS = null;

                                    lErr = "";
                                    lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                                    if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                                    {
                                        if (pRespType == 0)
                                        {
                                            lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                                            lResponse = lUsesrData + "," + lResponse;

                                            lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                                        }
                                        else if (pRespType == 1)
                                        {
                                            StringWriter sw = new StringWriter();
                                            lTmpDS.Tables[ltmpName].WriteXml(sw);
                                            lResponse = sw.ToString();
                                            lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                                        }
                                    }
                                    else
                                    {
                                        if (pRespType == 0)
                                        {
                                            lResponse = lUsesrData + ", []";

                                            lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                                        }
                                        else if (pRespType == 1)
                                        {
                                            lResponse = lUsesrData;
                                            lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                                        }
                                    }
                                }
                                else
                                {
                                    lResponse = lUsesrData;
                                    lResponse = AddHeadDataToResponseData(pRespType, 103, cTranslate.GetTransByID(103), lResponse); //Uporabnik je že prijavljen!
                                }
                            }
                            else
                            {
                                lResponse = lUsesrData;
                                lResponse = AddHeadDataToResponseData(pRespType, 106, cTranslate.GetTransByID(106), lResponse);
                            }
                        }
                        else
                        {
                            lResponse = AddHeadDataToResponseData(pRespType, 102, cTranslate.GetTransByID(102), lResponse);
                        }
                    }
                    else
                    {
                        lResponse = AddHeadDataToResponseData(pRespType, 101, cTranslate.GetTransByID(101), lResponse);
                    }
                }
                else
                {
                    lResponse = AddHeadDataToResponseData(pRespType, 100, cTranslate.GetTransByID(100), lResponse);
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[Login] " + exception.Message);
            }

            return lResponse;
        }

        public void Logout(string pUser, string pSessionID, int pStoreID, string pDivID, int pDesktopApp, int pRespType, string pOption)
        {
            try
            {
                Set_UserUpdate(pUser, 0, null, pDivID, pDesktopApp, null);
            }
            catch (Exception exception)
            {
                cLog.WriteError("[Logout] " + exception.Message);
            }

            return;
        }

        public string GetAllStores(string pDivID, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_queryGetAllStores  ");
                sSQL.AppendLine("  ");

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllStores] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllUsersOnStore(int pStoreID, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_queryGetAllUsersOnStoreID @StoreID ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllUsersOnStore] " + exception.Message);
            }

            return lResponse;
        }

        public string GetLocationsByType(string lLocationTypes, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT LocationID, StoreID, LocationCode, LocationName ");
                sSQL.AppendLine("FROM  StoreLocations ");
                sSQL.AppendLine("WHERE (PATINDEX( '%,' + cast(LocationTypeID as varchar) + ',%', ',' + @LocationTypes + ',' ) > 0) AND (Active = 1) ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@LocationTypes", lLocationTypes)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetLocationsByType] " + exception.Message);
            }

            return lResponse;
        }

        public string GetLocationsAndStoreByType(string lLocationTypes, int pStoreID, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT  StoreLocations.LocationID, Zones.Zone, StoreLocations.LocationCode, StoreLocations.LocationName,  ");
                sSQL.AppendLine("   StoreLocations.Active, StoreLocations.Width, StoreLocations.Height, StoreLocations.Depth, StoreLocations.BruttoVolume, StoreLocations.MaxWight,  StoreLocationTypes .LocationTypeName, StoreLocations.LocationTypeID, ");
                sSQL.AppendLine("   StoreLocations.MinStock, StoreLocations.MaxStock, ");
                sSQL.AppendLine("       (SELECT        SUM(Stock) AS Expr1 ");
                sSQL.AppendLine("         FROM            ArticleStoreLocation ");
                sSQL.AppendLine("         WHERE        (LocationID = StoreLocations.LocationID)) AS Stock, ");
                sSQL.AppendLine("       (SELECT        COUNT(DISTINCT ArticleID) AS Expr1 ");
                sSQL.AppendLine("         FROM            ArticleStoreLocation AS ArticleStoreLocation_1 ");
                sSQL.AppendLine("         WHERE        (LocationID = StoreLocations.LocationID)) AS NumOfArticles ");
                sSQL.AppendLine("FROM StoreLocations INNER JOIN ");
                sSQL.AppendLine("  StoreLocationTypes ON StoreLocations.LocationTypeID = StoreLocationTypes.LocationTypeID LEFT OUTER JOIN ");
                sSQL.AppendLine("  Zones ON StoreLocations.ZoneID = Zones.ZoneID ");
                sSQL.AppendLine("WHERE (StoreLocations.StoreID = @StoreID) AND (PATINDEX( '%,' + cast(StoreLocations.LocationTypeID as varchar) + ',%', ',' + @LocationTypes + ',' ) > 0)  AND (StoreLocations.Active = 1) ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@LocationTypes", lLocationTypes),
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetLocationsAndStoreByType] " + exception.Message);
            }

            return lResponse;
        }

        public string GetLocationTypes(int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT LocationTypeID, LocationTypeName ");
                sSQL.AppendLine("FROM  StoreLocationTypes ");
                sSQL.AppendLine(" ");

                /*
                                SqlParameter[] sqlParams = new SqlParameter[] {
                                    new SqlParameter("@LocationTypes", lLocationTypes)
                                };
                */
                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetLocationTypes] " + exception.Message);
            }

            return lResponse;
        }

        public string GetMeasureUnits(int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT MeasureUnitID, MeasureUnitName ");
                sSQL.AppendLine("FROM  MeasureUnits ");
                sSQL.AppendLine(" ");

                /*
                                SqlParameter[] sqlParams = new SqlParameter[] {
                                    new SqlParameter("@LocationTypes", lLocationTypes)
                                };
                */
                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetMeasureUnits] " + exception.Message);
            }

            return lResponse;
        }

        public string GetPriority(int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT PriorityID, PriorityName ");
                sSQL.AppendLine("FROM  Priority ");
                sSQL.AppendLine(" ");

                /*
                                SqlParameter[] sqlParams = new SqlParameter[] {
                                    new SqlParameter("@LocationTypes", lLocationTypes)
                                };
                */
                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetPriority] " + exception.Message);
            }

            return lResponse;
        }

        public string GetUserActivity(int pStoreID, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_queryUserActivity @StoreID ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");

                
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetUserActivity] " + exception.Message);
            }

            return lResponse;
        }

        public string GetTermDocumentStatus(int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT * ");
                sSQL.AppendLine("FROM  TermDocumentStatus ");
                sSQL.AppendLine(" ");

                /*
                                SqlParameter[] sqlParams = new SqlParameter[] {
                                    new SqlParameter("@LocationTypes", lLocationTypes)
                                };
                */
                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetTermDocumentStatus] " + exception.Message);
            }

            return lResponse;
        }

        public string GetTermDocumentTypes(int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT * ");
                sSQL.AppendLine("FROM  TermDocumentTypes ");
                sSQL.AppendLine(" ");

                /*
                                SqlParameter[] sqlParams = new SqlParameter[] {
                                    new SqlParameter("@LocationTypes", lLocationTypes)
                                };
                */
                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetTermDocumentTypes] " + exception.Message);
            }

            return lResponse;
        }

        public string GetArticleCodeType(int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT ArticleCodeTypeID, CodeTypeName, ModifiedDate ");
                sSQL.AppendLine("FROM  ArticleCodeType ");
                sSQL.AppendLine(" ");

                /*
                                SqlParameter[] sqlParams = new SqlParameter[] {
                                    new SqlParameter("@LocationTypes", lLocationTypes)
                                };
                */

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetArticleCodeType] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllUsers(int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT * ");
                sSQL.AppendLine("FROM Users ");
                sSQL.AppendLine(" ");

                /*
                                SqlParameter[] sqlParams = new SqlParameter[] {
                                    new SqlParameter("@LocationTypes", lLocationTypes)
                                };
                */
                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllUsers] " + exception.Message);
            }

            return lResponse;
        }

        public int SetUsers(int pUpdateType, JArray pUpdateData, int pRespType, string pOption)
        {
            int lResponse = 0;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                int lStoreID = 0;
                string lUserName = "";
                string lPassword = "";
                string lFirstName = "";
                string lLastName = "";
                string lTermName = "";
                int? lLocationID = 0;
                bool lActive = false;
                bool lAppActive = false;

                if (pRespType == 0) // jSon data
                {
                    JObject jo = (JObject)pUpdateData[0]; //JObject.Parse(pUpdateData);
                    lStoreID = (int)jo["StoreID"];
                    lUserName = (string)jo["UserName"];
                    lPassword = (string)jo["Password"];
                    lFirstName = (string)jo["FirstName"];
                    lLastName = (string)jo["LastName"];
                    lTermName = (string)jo["TerminalName"];
                    if ((int)jo["LocationID"] <= 0) { lLocationID = null; } else { lLocationID = (int)jo["LocationID"]; } ;
                    lActive = (bool)jo["Active"];
                    lAppActive = (bool)jo["AppActive"];
                }


                sSQL.Remove(0, sSQL.Length);
                if (pUpdateType == 0) //Update
                {
                    sSQL.AppendLine("UPDATE [Users] SET [Password] = @Password, [FirstName] = @FirstName, [LastName] = @LastName, [StoreID] = @StoreID, ");
                    sSQL.AppendLine(" [LocationID] = @LocationID, [Active] = @Active, [TerminalName] = @TerminalName, [ModifiedDate] = @ModifiedDate, [AppActive] = @AppActive ");
                    sSQL.AppendLine("WHERE UserName = @UserName ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@UserName", lUserName),
                        new SqlParameter("@Password", lPassword),
                        new SqlParameter("@FirstName", lFirstName),
                        new SqlParameter("@LastName", lLastName),
                        new SqlParameter("@StoreID", lStoreID),
                        new SqlParameter("@LocationID", lLocationID),
                        new SqlParameter("@Active", lActive),
                        new SqlParameter("@TerminalName", lTermName),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@AppActive", lAppActive)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }
                else if (pUpdateType == 1) //Insert
                {
                    sSQL.AppendLine("INSERT INTO [Users] ([UserName], [Password], [FirstName], [LastName], [StoreID], [LocationID], [Active], [TerminalName], [ModifiedDate], [AppActive]) ");
                    sSQL.AppendLine(" VALUES (@UserName, @Password, @FirstName, @LastName, @StoreID, @LocationID, @Active, @TerminalName, @ModifiedDate, @AppActive) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@UserName", lUserName),
                        new SqlParameter("@Password", lPassword),
                        new SqlParameter("@FirstName", lFirstName),
                        new SqlParameter("@LastName", lLastName),
                        new SqlParameter("@StoreID", lStoreID),
                        new SqlParameter("@LocationID", lLocationID),
                        new SqlParameter("@Active", lActive),
                        new SqlParameter("@TerminalName", lTermName),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@AppActive", lAppActive)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }
                else if (pUpdateType == 2) //Delete
                {
                    sSQL.AppendLine("DELETE FROM [Users]  ");
                    sSQL.AppendLine("WHERE UserName = @UserName ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@UserName", lUserName)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetUsers] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllLocations(int pStoreID, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT * ");
                sSQL.AppendLine("FROM StoreLocations ");
                sSQL.AppendLine("WHERE StoreID = @StoreID ");

                
                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllLocations] " + exception.Message);
            }

            return lResponse;
        }

        public int SetLocations(int pUpdateType, JArray pUpdateData, int pStoreID, string pUserName, int pRespType, string pOption)
        {
            int lResponse = 0;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                //int lStoreID = 0;
                int lLocationID = 0;
                int lp = 0;
                int lx = 0;
                int ly = 0;
                int lz = 0;
                string LocationCode = "";
                string LocationName = "";
                int LocationTypeID = 0;
                bool lActive = false;

                if (pRespType == 0) // jSon data
                {
                    JObject jo = (JObject)pUpdateData[0]; //JObject.Parse(pUpdateData);
                    //lStoreID = (int)jo["StoreID"];
                    LocationCode = (string)jo["LocationCode"];
                    LocationName = (string)jo["LocationName"];
                    lp = (int)jo["p"];
                    lx = (int)jo["x"];
                    ly = (int)jo["y"];
                    lz = (int)jo["z"];
                    if (pUpdateType != 1) { lLocationID = (int)jo["LocationID"]; }
                    lActive = (bool)jo["Active"];
                    LocationTypeID = (int)jo["LocationTypeID"];
                }


                sSQL.Remove(0, sSQL.Length);
                if (pUpdateType == 0) //Update
                {
                    sSQL.AppendLine("UPDATE StoreLocations SET StoreID = @StoreID, ZoneID = @ZoneID, p = @p, x = @x, y = @y, z = @z, LocationCode = @LocationCode, LocationName = @LocationName, ");
                    sSQL.AppendLine("    LocationTypeID = @LocationTypeID, LocationPriorityID = @LocationPriorityID, Active = @Active, Width = @Width, Height = @Height, Depth = @Depth, ");
                    sSQL.AppendLine("    BruttoVolume = @BruttoVolume, MaxWight = @MaxWight, MinStock = @MinStock, MaxStock = @MaxStock, UserChanged = @UserChanged ");
                    sSQL.AppendLine("WHERE (LocationID = @LocationID) AND (StoreID = @StoreID) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@StoreID", pStoreID),
                        new SqlParameter("@LocationID", lLocationID),
                        new SqlParameter("@ZoneID", DBNull.Value),
                        new SqlParameter("@p", lp),
                        new SqlParameter("@x", lx),
                        new SqlParameter("@y", ly),
                        new SqlParameter("@z", lz),
                        new SqlParameter("@LocationCode", LocationCode),
                        new SqlParameter("@Active", lActive),
                        new SqlParameter("@LocationName", LocationName),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@UserChanged", pUserName),
                        new SqlParameter("@LocationTypeID", LocationTypeID),
                        new SqlParameter("@LocationPriorityID", DBNull.Value),
                        new SqlParameter("@Width", DBNull.Value),
                        new SqlParameter("@Height", DBNull.Value),
                        new SqlParameter("@Depth", DBNull.Value),
                        new SqlParameter("@BruttoVolume", DBNull.Value),
                        new SqlParameter("@MaxWight", DBNull.Value),
                        new SqlParameter("@MinStock", DBNull.Value),
                        new SqlParameter("@MaxStock", DBNull.Value)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }
                else if (pUpdateType == 1) //Insert
                {
                    sSQL.AppendLine("INSERT INTO StoreLocations (StoreID, ZoneID, p, x, y, z, LocationCode, LocationName, LocationTypeID, LocationPriorityID, Active, Width, Height, Depth, BruttoVolume, MaxWight, MinStock, MaxStock, UserChanged) ");
                    sSQL.AppendLine("VALUES (@StoreID,@ZoneID,@p,@x,@y,@z,@LocationCode,@LocationName,@LocationTypeID,@LocationPriorityID,@Active,@Width,@Height,@Depth,@BruttoVolume,@MaxWight,@MinStock,@MaxStock,@UserChanged)");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@StoreID", pStoreID),
                        new SqlParameter("@ZoneID", DBNull.Value),
                        new SqlParameter("@p", lp),
                        new SqlParameter("@x", lx),
                        new SqlParameter("@y", ly),
                        new SqlParameter("@z", lz),
                        new SqlParameter("@LocationCode", LocationCode),
                        new SqlParameter("@Active", lActive),
                        new SqlParameter("@LocationName", LocationName),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@UserChanged", pUserName),
                        new SqlParameter("@LocationTypeID", LocationTypeID),
                        new SqlParameter("@LocationPriorityID", DBNull.Value),
                        new SqlParameter("@Width", DBNull.Value),
                        new SqlParameter("@Height", DBNull.Value),
                        new SqlParameter("@Depth", DBNull.Value),
                        new SqlParameter("@BruttoVolume", DBNull.Value),
                        new SqlParameter("@MaxWight", DBNull.Value),
                        new SqlParameter("@MinStock", DBNull.Value),
                        new SqlParameter("@MaxStock", DBNull.Value)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }
                else if (pUpdateType == 2) //Delete
                {
                    sSQL.AppendLine("DELETE FROM StoreLocations  ");
                    sSQL.AppendLine("WHERE (LocationID = @LocationID) AND (StoreID = @StoreID) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@LocationID", lLocationID),
                        new SqlParameter("@StoreID", pStoreID)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetLocations] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllArticles(string pArticleCode, int pRespType, string pOption, out string pErr)
        {
            DataTable lTmpDT = null;
            string lResponse = "";

            pErr = "";

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT  Articles.*, ");
                sSQL.AppendLine("   ArticleCodes.Code ");
                sSQL.AppendLine("FROM  Articles LEFT OUTER JOIN ArticleCodes ON Articles.ArticleID = ArticleCodes.ArticleID AND ArticleCodes.ArticleCodeTypeID = 0 ");
                sSQL.AppendLine("WHERE (Articles.ArticleID IN ");
                sSQL.AppendLine("    (SELECT ArticleID ");
                sSQL.AppendLine("       FROM dbo.sp_queryFindMultiArticle(@ArticleCode))) ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@ArticleCode", pArticleCode)
                };


                lTmpDT = lSql.FillDT(sSQL, sqlParams, out pErr);

                if (lTmpDT.Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDT, Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        if (String.IsNullOrEmpty(lTmpDT.TableName)) { lTmpDT.TableName = "mrAntWMS"; }
                        lTmpDT.WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                pErr = "[GetAllArticles] " + exception.Message;
                cLog.WriteError(pErr);
            }

            return lResponse;
        }

        public int SetArticles(int pUpdateType, JArray pUpdateData, int pStoreID, string pUserName, int pRespType, string pOption, out string pErr)
        {
            int lResponse = 0;
            pErr = "";

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                int lArticleID = 0;
                float lNetoWeight = 0;
                float lBrutoWeight = 0;
                float lWidth = 0;
                float lHeight = 0;
                float lDepth = 0;
                float lBruttoVolume = 0;
                string lPrimaryCode = "";
                string lERPArticleID = "";
                string lArticleName = "";
                bool lActive = false;
                string lMeasureUnitID = "";

                if (pRespType == 0) // jSon data
                {
                    JObject jo = (JObject)pUpdateData[0]; //JObject.Parse(pUpdateData);
                    //lStoreID = (int)jo["StoreID"];
                    lPrimaryCode = jo.Value<string>("PrimaryCode") ?? null;
                    lERPArticleID = jo.Value<string>("ERPArticleID") ?? null;
                    lArticleName = jo.Value<string>("ArticleName") ?? null; 
                    lNetoWeight = jo.Value<float?>("NetoWeight") ?? 0;
                    lBrutoWeight = jo.Value<float?>("BrutoWeight") ?? 0;
                    lWidth = jo.Value<float?>("Width") ?? 0;
                    lHeight = jo.Value<float?>("Height") ?? 0;
                    lDepth = jo.Value<float?>("Depth") ?? 0;
                    lBruttoVolume = jo.Value<float?>("BruttoVolume") ?? 0;
                    lArticleID = jo.Value<int?>("ArticleID") ?? 0;
                    lActive = jo.Value<bool?>("Active") ?? false;
                    lMeasureUnitID = jo.Value<string>("MeasureUnitID") ?? null;
                }


                sSQL.Remove(0, sSQL.Length);
                if (pUpdateType == 0) //Update
                {
                    sSQL.AppendLine("UPDATE Articles SET PrimaryCode = @PrimaryCode, ERPArticleID = @ERPArticleID, ArticleName = @ArticleName, Active = @Active, MeasureUnitID = @MeasureUnitID,  ");
                    sSQL.AppendLine("    NetoWeight = @NetoWeight, BrutoWeight = @BrutoWeight, Width = @Width, Height = @Height, Depth = @Depth, BruttoVolume = @BruttoVolume, ");
                    sSQL.AppendLine("    UserChanged = @UserChanged ");
                    sSQL.AppendLine("WHERE (ArticleID = @ArticleID) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ArticleID", lArticleID),
                        new SqlParameter("@PrimaryCode", lPrimaryCode),
                        new SqlParameter("@ERPArticleID", lERPArticleID),
                        new SqlParameter("@ArticleName", lArticleName),
                        new SqlParameter("@MeasureUnitID", lMeasureUnitID),
                        new SqlParameter("@Active", lActive),
                        new SqlParameter("@NetoWeight", lNetoWeight),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@UserChanged", pUserName),
                        new SqlParameter("@BrutoWeight", lBrutoWeight),
                        new SqlParameter("@Width", DBNull.Value),
                        new SqlParameter("@Height", DBNull.Value),
                        new SqlParameter("@Depth", DBNull.Value),
                        new SqlParameter("@BruttoVolume", DBNull.Value)
                    };

                    pErr = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (pErr.Length == 0) { lResponse = 1; }
                }
                else if (pUpdateType == 1) //Insert
                {
                    sSQL.AppendLine("INSERT INTO Articles (PrimaryCode, ERPArticleID, ArticleName, Active, MeasureUnitID, NetoWeight, BrutoWeight, Width, Height, Depth, BruttoVolume, UserChanged, ModifiedDate) ");
                    sSQL.AppendLine("VALUES (@PrimaryCode,@ERPArticleID,@ArticleName,@Active,@MeasureUnitID,@NetoWeight,@BrutoWeight,@Width,@Height,@Depth,@BruttoVolume,@UserChanged,@ModifiedDate) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@PrimaryCode", lPrimaryCode),
                        new SqlParameter("@ERPArticleID", lERPArticleID),
                        new SqlParameter("@ArticleName", lArticleName),
                        new SqlParameter("@MeasureUnitID", lMeasureUnitID),
                        new SqlParameter("@Active", lActive),
                        new SqlParameter("@NetoWeight", lNetoWeight),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@UserChanged", pUserName),
                        new SqlParameter("@BrutoWeight", lBrutoWeight),
                        new SqlParameter("@Width", DBNull.Value),
                        new SqlParameter("@Height", DBNull.Value),
                        new SqlParameter("@Depth", DBNull.Value),
                        new SqlParameter("@BruttoVolume", DBNull.Value)
                    };

                    pErr = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (pErr.Length == 0) { lResponse = 1; }
                }
                else if (pUpdateType == 2) //Delete
                {
                    sSQL.AppendLine("DELETE FROM Articles  ");
                    sSQL.AppendLine("WHERE (ArticleID = @ArticleID) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ArticleID", lArticleID)
                    };

                    pErr = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (pErr.Length == 0) { lResponse = 1; }
                }

            }
            catch (Exception exception)
            {
                pErr = "[SetArticles] " + exception.Message;
                cLog.WriteError(pErr);
            }

            return lResponse;
        }

        public string GetAllBarcodes(string pArticleCode, string pUserName, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_queryFillBarCodes @UserName, @ArticleCode ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@ArticleCode", pArticleCode),
                    new SqlParameter("@UserName", pUserName)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllBarcodes] " + exception.Message);
            }

            return lResponse;
        }

        public int SetBarcodes(int pUpdateType, JArray pUpdateData, int pStoreID, string pUserName, int pRespType, string pOption)
        {
            int lResponse = 0;
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                int lArticleID = 0;
                string lArticleName = "";
                string lPrimaryCode = "";
                string lBarCode = "";
                bool lPrimaryBarCode = false;

                if (pRespType == 0) // jSon data
                {
                    JObject jo = (JObject)pUpdateData[0]; //JObject.Parse(pUpdateData);
                    //lStoreID = (int)jo["StoreID"];
                    lPrimaryCode = jo.Value<string>("PrimaryCode") ?? null;
                    lBarCode = jo.Value<string>("BarCode") ?? null;
                    lArticleName = jo.Value<string>("ArticleName") ?? null;
                    lArticleID = jo.Value<int?>("ArticleID") ?? 0;
                    lPrimaryBarCode = jo.Value<bool?>("PrimaryBarCode") ?? false;
                }


                sSQL.Remove(0, sSQL.Length);
                if (pUpdateType == 0) //Update
                {
                    sSQL.AppendLine("declare @p5 int ");
                    sSQL.AppendLine("set @p5=0 ");
                    sSQL.AppendLine("exec dbo.sp_BarCodeAdd @UserName, @ArticleCode, @BarCode, @PrimaryCode, @p5 output, @UseTRans, @ArticleCodeIsERPArticleID ");
                    sSQL.AppendLine("select @p5 as Resp ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ArticleCode", lPrimaryCode),
                        new SqlParameter("@BarCode", lBarCode),
                        new SqlParameter("@ArticleName", lArticleName),
                        new SqlParameter("@PrimaryCode", lPrimaryBarCode),
                        new SqlParameter("@UserName", pUserName),
                        new SqlParameter("@UseTRans", true),
                        new SqlParameter("@ArticleCodeIsERPArticleID", true)
                    };

                    string lErr = "";
                    DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                    if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                    {
                        DataRow lrow = lTmpDS.Tables[ltmpName].Rows[0];
                        lResponse = int.Parse(lrow["Resp"].ToString()) + 1; // Resp povečamo za ena, da je usklajeno z našim standardom! 0 - ni ok, 1 - ok, 2 - .....
                    }

                }
                else if (pUpdateType == 1) //Insert
                {
                    sSQL.AppendLine("declare @p5 int ");
                    sSQL.AppendLine("set @p5=0 ");
                    sSQL.AppendLine("exec dbo.sp_BarCodeAdd @UserName, @ArticleCode, @BarCode, @PrimaryCode, @p5 output, @UseTRans, @ArticleCodeIsERPArticleID ");
                    sSQL.AppendLine("select @p5 as Resp ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ArticleCode", lPrimaryCode),
                        new SqlParameter("@BarCode", lBarCode),
                        new SqlParameter("@ArticleName", lArticleName),
                        new SqlParameter("@PrimaryCode", lPrimaryBarCode),
                        new SqlParameter("@UserName", pUserName),
                        new SqlParameter("@UseTRans", true),
                        new SqlParameter("@ArticleCodeIsERPArticleID", true)
                    };

                    string lErr = "";
                    DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                    if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                    {
                        DataRow lrow = lTmpDS.Tables[ltmpName].Rows[0];
                        lResponse = int.Parse(lrow["Resp"].ToString())+1; // Resp povečamo za ena, da je usklajeno z našim standardom! 0 - ni ok, 1 - ok, 2 - .....
                    }
                }
                else if (pUpdateType == 2) //Delete
                {
                    sSQL.AppendLine("DELETE FROM Articles  ");
                    sSQL.AppendLine("WHERE (ArticleID = @ArticleID) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@ArticleID", lArticleID)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetBarcodes] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllSupplyHeadNotify(int pStoreID, int pSupplyNotifyStatus, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT exSupplyHeadNotify.SupplyNotifyID, exSupplyHeadNotify.SystemDate, exSupplyHeadNotify.SupplyDocID, exSupplyHeadNotify.SupplyDocType, ");
                sSQL.AppendLine("       TermDocumentTypes.DocumentTypeID, TermDocumentTypes.TypeName, exSupplyHeadNotify.SupplyDate, exSupplyHeadNotify.CustomerID, ");
                sSQL.AppendLine("       exSupplyHeadNotify.Customer, exSupplyHeadNotify.CustStreet, exSupplyHeadNotify.CustPostalCodeID, exSupplyHeadNotify.CustCity, ");
                sSQL.AppendLine("       (SELECT        COUNT(*) AS Expr1 ");
                sSQL.AppendLine("         FROM            exSupplyPosNotify WITH (NOLOCK) ");
                sSQL.AppendLine("         WHERE        (SupplyNotifyID = exSupplyHeadNotify.SupplyNotifyID)) AS PosCount, ");
                sSQL.AppendLine("       (SELECT        SUM(Quantity) AS Expr1 ");
                sSQL.AppendLine("         FROM            exSupplyPosNotify AS exSupplyPosNotify_1 WITH (NOLOCK) ");
                sSQL.AppendLine("         WHERE        (SupplyNotifyID = exSupplyHeadNotify.SupplyNotifyID)) AS SumQuantity, exSupplyHeadNotify.SrcStore, ");
                sSQL.AppendLine("       UPPER(exSupplyHeadNotify.DeliveryID) AS DeliveryID ");
                sSQL.AppendLine("FROM exSupplyHeadNotify WITH (NOLOCK) INNER JOIN ");
                sSQL.AppendLine("   ConvertStoreID WITH (NOLOCK) ON ConvertStoreID.exStoreID = exSupplyHeadNotify.StoreID LEFT OUTER JOIN ");
                sSQL.AppendLine("   ConvertDocumentTypeID WITH (NOLOCK) ON ConvertDocumentTypeID.exDocumentTypeID = exSupplyHeadNotify.SupplyDocType LEFT OUTER JOIN ");
                sSQL.AppendLine("   TermDocumentTypes WITH (NOLOCK) ON TermDocumentTypes.DocumentTypeID = ConvertDocumentTypeID.TermDocumentTypeID ");
                sSQL.AppendLine("WHERE (exSupplyHeadNotify.SupplyNotifyStatus = @SupplyNotifyStatus) AND (ConvertStoreID.StoreID = @StoreID) ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@SupplyNotifyStatus", pSupplyNotifyStatus),
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllSupplyHeadNotify] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllPublishHeadNotify(int pStoreID, string pPublishNotifyStatus, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("DECLARE @StoreID as int ");
                sSQL.AppendLine("DECLARE @PublishNotifyStatus as varchar(20) ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("SET @StoreID = @pStoreID ");
                sSQL.AppendLine("SET @PublishNotifyStatus = @pPublishNotifyStatus ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("SELECT exPublishHeadNotify.PublishNotifyID, exPublishHeadNotify.SystemDate, exPublishHeadNotify.PublishDocID, exPublishHeadNotify.PublishDocType, ");
                sSQL.AppendLine("       TermDocumentTypes.TypeName, exPublishHeadNotify.PublishDeliveryDate, exPublishHeadNotify.CustomerID, exPublishHeadNotify.Customer, ");
                sSQL.AppendLine("       exPublishHeadNotify.CustStreet, exPublishHeadNotify.CustPostalCodeID, exPublishHeadNotify.CustCity, exPublishHeadNotify.DeliveryPath, ");
                sSQL.AppendLine("       UPPER(exPublishHeadNotify.DeliveryID) AS DeliveryID, ");
                sSQL.AppendLine("           (SELECT     AVG(CASE WHEN ISNULL(wmsArticleStore.freeStock, 0) >= (P.Quantity - ISNULL(PC.Quantity, 0)) THEN CAST(0 AS money) ");
                sSQL.AppendLine("                WHEN ISNULL(wmsArticleStore.freeStock, 0) > 0 THEN CAST(1 AS money) ELSE CAST(2 AS money) END) AS Expr1 ");
                sSQL.AppendLine("           FROM exPublishPosNotify AS P WITH (NOLOCK) LEFT OUTER JOIN ");
                sSQL.AppendLine("              Articles WITH (NOLOCK) ON P.ERPArticleID = Articles.ERPArticleID LEFT OUTER JOIN ");
                sSQL.AppendLine("              exPublishPosConfirm AS PC ON PC.NotifyPosRowGuid = P.NotifyPosRowGuid LEFT OUTER JOIN ");
                sSQL.AppendLine("              wmsArticleStoreStock AS wmsArticleStore WITH (NOLOCK) ON wmsArticleStore.StoreID = @StoreID AND  ");
                sSQL.AppendLine("              wmsArticleStore.ArticleID = Articles.ArticleID ");
                sSQL.AppendLine("           WHERE      (P.PublishNotifyID = exPublishHeadNotify.PublishNotifyID) AND (P.Quantity - ISNULL(PC.Quantity, 0) > 0)) AS StockEstimate, ");
                sSQL.AppendLine("           (SELECT COUNT(*) AS C ");
                sSQL.AppendLine("           FROM exPublishPosNotify AS P WITH (NOLOCK) LEFT OUTER JOIN ");
                sSQL.AppendLine("               exPublishPosConfirm AS PC ON PC.NotifyPosRowGuid = P.NotifyPosRowGuid ");
                sSQL.AppendLine("           WHERE      (P.PublishNotifyID = exPublishHeadNotify.PublishNotifyID) AND (P.Quantity - ISNULL(PC.Quantity, 0) > 0)) AS PosCount, ");
                sSQL.AppendLine("           (SELECT     SUM(P.Quantity - ISNULL(PC.Quantity, 0)) AS Q ");
                sSQL.AppendLine("           FROM          exPublishPosNotify AS P WITH (NOLOCK) LEFT OUTER JOIN ");
                sSQL.AppendLine("               exPublishPosConfirm AS PC ON PC.NotifyPosRowGuid = P.NotifyPosRowGuid ");
                sSQL.AppendLine("           WHERE      (P.PublishNotifyID = exPublishHeadNotify.PublishNotifyID)) AS SumQuantity, exPublishHeadNotify.SrcStore, ");
                sSQL.AppendLine("       exPublishHeadNotify.PublishNotifyStatus ");
                sSQL.AppendLine("FROM exPublishHeadNotify WITH (NOLOCK) LEFT OUTER JOIN ");
                sSQL.AppendLine("       ConvertDocumentTypeID WITH (NOLOCK) ON ConvertDocumentTypeID.exDocumentTypeID = exPublishHeadNotify.PublishDocType LEFT OUTER JOIN ");
                sSQL.AppendLine("       TermDocumentTypes WITH (NOLOCK) ON TermDocumentTypes.DocumentTypeID = ConvertDocumentTypeID.TermDocumentTypeID ");
                sSQL.AppendLine("WHERE (exPublishHeadNotify.PublishNotifyStatus IN ");
                sSQL.AppendLine("       (SELECT     KeyID ");
                sSQL.AppendLine("        FROM          dbo.f_helperGetTableIntegerFromString(@PublishNotifyStatus) AS f_helperGetTableIntegerFromString_1)) AND EXISTS ");
                sSQL.AppendLine("       (SELECT     exPublishPosNotify_1.PublishNotifyID ");
                sSQL.AppendLine("        FROM          exPublishPosNotify AS exPublishPosNotify_1 INNER JOIN ");
                sSQL.AppendLine("           ConvertStoreID ON ConvertStoreID.exStoreID = exPublishPosNotify_1.StoreID ");
                sSQL.AppendLine("       WHERE      (exPublishPosNotify_1.PublishNotifyID = exPublishHeadNotify.PublishNotifyID) AND (ConvertStoreID.StoreID = @StoreID)) ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@pPublishNotifyStatus", pPublishNotifyStatus),
                    new SqlParameter("@pStoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllPublishHeadNotify] " + exception.Message);
            }

            return lResponse;
        }

        public string GetSupplyHeadNotifyPos(int pStoreID, int pSupplyNotifyID, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_exGetSupplyPosNotifiy @SupplyNotifyID ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@SupplyNotifyID", pSupplyNotifyID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetSupplyHeadNotifyPos] " + exception.Message);
            }

            return lResponse;
        }

        public string GetPublishHeadNotifyPos(int pStoreID, int pPublishNotifyID, int pAllPos, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_exGetPublishPosNotifiy @PublishNotifyID, @StoreID, @AllPos ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@PublishNotifyID", pPublishNotifyID),
                    new SqlParameter("@StoreID", pStoreID),
                    new SqlParameter("@AllPos", pAllPos)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetPublishHeadNotifyPos] " + exception.Message);
            }

            return lResponse;
        }

        public int SetTransSupplyNotifiysToTermDocuments(int pDstLocationID, int pPriorityID, DateTime pPlannedDate, string pSupplyNotifyIDs, string pOwnerIDs, int pStoreID, string pUserName, int pRespType, string pOption)
        {
            int lResponse = -1;
            string ltmpName = Guid.NewGuid().ToString();

            int lretValue = -1;
            string lretArticleERP;
            string lretTermDocumentsRowGuid;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("declare @p6 int ");
                sSQL.AppendLine("declare @p7 nchar(50) ");
                sSQL.AppendLine("declare @p8 uniqueidentifier ");
                sSQL.AppendLine("declare @lUsers nchar(10) ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("BEGIN TRAN T1 ");
                sSQL.AppendLine("exec dbo.sp_exTransSupplyNotifiysToTermDocuments @OwnerID, @StringSupplyNotifyIDs, @PriorityID, @PlannedDate, @DstLocationID, @p6 output, @p7 output, @p8 output, @OutInTrans ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("IF @p6 = 0 ");
                sSQL.AppendLine("BEGIN ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  DECLARE m_c CURSOR FOR ");
                sSQL.AppendLine("  SELECT UserName FROM Users ");
                sSQL.AppendLine("  WHERE (PATINDEX( '%,' + RTRIM(UserName) + ',%', ',' + @pOwnerIDs + ',' ) > 0) ");
                //sSQL.AppendLine("  WHERE UserName IN (" + pOwnerIDs + ") ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  OPEN m_c ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  FETCH NEXT FROM m_c ");
                sSQL.AppendLine("  INTO @lUsers ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  WHILE @@FETCH_STATUS = 0 ");
                sSQL.AppendLine("  BEGIN ");
                sSQL.AppendLine("    exec dbo.sp_TermDocumentOwners_insert @p8, @lUsers ");
                sSQL.AppendLine("    FETCH NEXT FROM m_c ");
                sSQL.AppendLine("    INTO @lUsers ");
                sSQL.AppendLine("  END ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  CLOSE m_c; ");
                sSQL.AppendLine("  DEALLOCATE m_c; ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  IF @@ERROR <> 0 SET @p6 = -1		-- Napaka pri nastavitvi skladišnikov ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("END	 ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("IF @p6 = 0 COMMIT TRAN T1 ELSE ROLLBACK TRAN T1  ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("select @p6 as retValue, @p7 as retArticleERP, @p8 as retTermDocumentsRowGuid ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@OwnerID", pUserName),
                        new SqlParameter("@StringSupplyNotifyIDs", pSupplyNotifyIDs),
                        new SqlParameter("@PriorityID", pPriorityID),
                        new SqlParameter("@PlannedDate", pPlannedDate),
                        new SqlParameter("@DstLocationID", pDstLocationID),
                        new SqlParameter("@pOwnerIDs", pOwnerIDs),
                        new SqlParameter("@OutInTrans", 1)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    DataRow lrow;
                    if (lTmpDS.Tables.Count > 1)
                    {
                        lrow = lTmpDS.Tables[1].Rows[0];
                    }
                    else
                    {
                        lrow = lTmpDS.Tables[ltmpName].Rows[0];
                    }
                    lretValue = int.Parse(lrow["retValue"].ToString());
                    lretArticleERP = (lrow["retArticleERP"].ToString());
                    lretTermDocumentsRowGuid = (lrow["retTermDocumentsRowGuid"].ToString());
                }

                lResponse = lretValue;

            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetTransSupplyNotifiysToTermDocuments] " + exception.Message);
            }

            return lResponse;
        }

        public int SetTransPublishNotifiysToTermDocuments(int pDstLocationID, int pPriorityID, DateTime pPlannedDate, string pPublishNotifyIDs, string pOwnerIDs, int pStoreID, string pUserName, int pRespType, string pOption)
        {
            int lResponse = -1;
            string ltmpName = Guid.NewGuid().ToString();

            int lretValue = -1;
            string lretArticleERP;
            string lretTermDocumentsRowGuid;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("declare @p6 int ");
                sSQL.AppendLine("declare @p7 nchar(50) ");
                sSQL.AppendLine("declare @p8 uniqueidentifier ");
                sSQL.AppendLine("declare @lUsers nchar(10) ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("BEGIN TRAN T1 ");
                sSQL.AppendLine("exec dbo.sp_exTransPublishNotifiysToTermDocuments @OwnerID, @StringPublishNotifyIDs, @PriorityID, @PlannedDate, @DstLocationID, @PutDeliveryPathID, @p6 output, @p7 output, @p8 output, @OutInTrans ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("IF @p6 = 0 ");
                sSQL.AppendLine("BEGIN ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  DECLARE m_c CURSOR FOR ");
                sSQL.AppendLine("  SELECT UserName FROM Users ");
                sSQL.AppendLine("  WHERE (PATINDEX( '%,' + RTRIM(UserName) + ',%', ',' + @pOwnerIDs + ',' ) > 0) ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  OPEN m_c ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  FETCH NEXT FROM m_c ");
                sSQL.AppendLine("  INTO @lUsers ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  WHILE @@FETCH_STATUS = 0 ");
                sSQL.AppendLine("  BEGIN ");
                sSQL.AppendLine("    exec dbo.sp_TermDocumentOwners_insert @p8, @lUsers ");
                sSQL.AppendLine("    FETCH NEXT FROM m_c ");
                sSQL.AppendLine("    INTO @lUsers ");
                sSQL.AppendLine("  END ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  CLOSE m_c; ");
                sSQL.AppendLine("  DEALLOCATE m_c; ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  IF @@ERROR <> 0 SET @p6 = -1		-- Napaka pri nastavitvi skladišnikov ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("END	 ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("IF @p6 = 0 COMMIT TRAN T1 ELSE ROLLBACK TRAN T1  ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("select @p6 as retValue, @p7 as retArticleERP, @p8 as retTermDocumentsRowGuid ");
                sSQL.AppendLine(" ");


                SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@OwnerID", pUserName),
                        new SqlParameter("@StringPublishNotifyIDs", pPublishNotifyIDs),
                        new SqlParameter("@PriorityID", pPriorityID),
                        new SqlParameter("@PlannedDate", pPlannedDate),
                        new SqlParameter("@DstLocationID", pDstLocationID),
                        new SqlParameter("@pOwnerIDs", pOwnerIDs),
                        new SqlParameter("@PutDeliveryPathID", null),
                        new SqlParameter("@OutInTrans", 1)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    DataRow lrow;
                    if (lTmpDS.Tables.Count > 1)
                    {
                        lrow = lTmpDS.Tables[1].Rows[0];
                    }
                    else
                    {
                        lrow = lTmpDS.Tables[ltmpName].Rows[0];
                    }
                    lretValue = int.Parse(lrow["retValue"].ToString());
                    lretArticleERP = (lrow["retArticleERP"].ToString());
                    lretTermDocumentsRowGuid = (lrow["retTermDocumentsRowGuid"].ToString());
                }

                lResponse = lretValue;

            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetTransPublishNotifiysToTermDocuments] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllTermDocuments(int pStoreID, int pTermDocumentsStatusID_From, int pTermDocumentsStatusID_To, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT  TermDocuments.rowguid, TermDocuments.StoreID, TermDocuments.DocumentID, TermDocuments.DocumentTypeID, TermDocumentTypes.TypeName, TermDocuments.StatusID, ");
                sSQL.AppendLine("   TermDocuments.PriorityID, TermDocuments.DeliveryPathID, TermDocuments.PostionRows, TermDocuments.SystemDate, TermDocuments.PlannedDate, ");
                sSQL.AppendLine("   TermDocuments.CompletedDate, TermDocuments.DstLocationID, TermDocuments.ModifiedDate, TermDocuments.UserChange, ");
                sSQL.AppendLine("   TermDocuments.UserChangeDate, StoreLocations.LocationCode, StoreLocations.LocationName, (CASE WHEN TermDocumentTypes.Credit = 0 AND ");
                sSQL.AppendLine("   TermDocuments.StatusID = 100 THEN CASE WHEN ISNULL ");
                sSQL.AppendLine("       ((SELECT  TOP 1 1  FROM TermDocumentPos WITH (NOLOCK) INNER JOIN ");
                sSQL.AppendLine("                 wmsArticleStoreStock wArticleStore ON wArticleStore.StoreID = TermDocuments.StoreID AND wArticleStore.ArticleID = TermDocumentPos.ArticleID ");
                sSQL.AppendLine("         WHERE        TermDocumentPos.TermDocumentsRowGuid = TermDocuments.rowguid AND wArticleStore.freeStock > 0 ");
                sSQL.AppendLine("         GROUP BY TermDocumentPos.ArticleID ");
                sSQL.AppendLine("         HAVING   SUM(TermDocumentPos.OrderedQuantity) > ISNULL ");
                sSQL.AppendLine("                  ((SELECT        SUM(PrepareQuantity) ");
                sSQL.AppendLine("                      FROM  TermDocumentSubPos WITH (NOLOCK) ");
                sSQL.AppendLine("                      WHERE TermDocumentSubPos.TermDocumentsRowGuid = TermDocuments.rowguid AND  ");
                sSQL.AppendLine("                            TermDocumentSubPos.ArticleID = TermDocumentPos.ArticleID), 0)), 0) = 1 THEN 1 WHEN ISNULL ");
                sSQL.AppendLine("       ((SELECT  TOP 1 2 FROM  TermDocumentPos WITH (NOLOCK) ");
                sSQL.AppendLine("         WHERE        TermDocumentPos.TermDocumentsRowGuid = TermDocuments.rowguid ");
                sSQL.AppendLine("         GROUP BY TermDocumentPos.ArticleID ");
                sSQL.AppendLine("         HAVING        SUM(TermDocumentPos.OrderedQuantity) > ISNULL ");
                sSQL.AppendLine("                  ((SELECT  SUM(PrepareQuantity) ");
                sSQL.AppendLine("                      FROM TermDocumentSubPos WITH (NOLOCK) ");
                sSQL.AppendLine("                      WHERE TermDocumentSubPos.TermDocumentsRowGuid = TermDocuments.rowguid AND TermDocumentSubPos.ArticleID = TermDocumentPos.ArticleID), 0)), 0) ");
                sSQL.AppendLine("   = 2 THEN 2 ELSE 0 END WHEN TermDocumentTypes.Credit = 1 AND TermDocuments.StatusID = 100 THEN ISNULL ");
                sSQL.AppendLine("       ((SELECT TOP 1 1 FROM TermDocumentPos WITH (NOLOCK) ");
                sSQL.AppendLine("         WHERE TermDocumentPos.TermDocumentsRowGuid = TermDocuments.rowguid ");
                sSQL.AppendLine("         GROUP BY TermDocumentPos.ArticleID ");
                sSQL.AppendLine("         HAVING SUM(TermDocumentPos.OrderedQuantity) <> ISNULL ");
                sSQL.AppendLine("                  ((SELECT  SUM(PrepareQuantity) FROM TermDocumentSubPos WITH (NOLOCK) ");
                sSQL.AppendLine("                    WHERE        TermDocumentSubPos.TermDocumentsRowGuid = TermDocuments.rowguid AND TermDocumentSubPos.ArticleID = TermDocumentPos.ArticleID), 0)), 0) ELSE 0 END) AS EstimateValue, ");
                sSQL.AppendLine("       (SELECT TOP 1 CASE WHEN ");
                sSQL.AppendLine("           (SELECT COUNT(DISTINCT TermDocumentPos.CustomerID) FROM TermDocumentPos ");
                sSQL.AppendLine("              WHERE        TermDocumentPos.TermDocumentsRowGuid = TermDocuments.rowguid) = 1 THEN Customers.CompanyName ELSE NULL END AS Expr1 ");
                sSQL.AppendLine("         FROM TermDocumentPos WITH (NOLOCK) INNER JOIN  Customers WITH (NOLOCK) ON Customers.CustomerID = TermDocumentPos.CustomerID ");
                sSQL.AppendLine("         WHERE (TermDocumentPos.TermDocumentsRowGuid = TermDocuments.rowguid)) AS CompanyName ");
                sSQL.AppendLine("FROM TermDocuments WITH (NOLOCK) LEFT OUTER JOIN ");
                sSQL.AppendLine("     StoreLocations WITH (NOLOCK) ON TermDocuments.DstLocationID = StoreLocations.LocationID INNER JOIN ");
                sSQL.AppendLine("     TermDocumentTypes WITH (NOLOCK) ON TermDocumentTypes.DocumentTypeID = TermDocuments.DocumentTypeID ");
                sSQL.AppendLine("WHERE (TermDocuments.StoreID = @StoreID) AND (TermDocuments.StatusID BETWEEN @TermDocumentsStatusID_From AND @TermDocumentsStatusID_To) ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@TermDocumentsStatusID_From", pTermDocumentsStatusID_From),
                    new SqlParameter("@TermDocumentsStatusID_To", pTermDocumentsStatusID_To),
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllTermDocuments] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllTermDocumentPos(int pStoreID, string pTermDocumentsRowGuid, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT  TermDocumentPos.rowguid, TermDocumentPos.TermDocumentsRowGuid, TermDocumentPos.ERPDocumentID, TermDocumentPos.ERPDocDate, ");
                sSQL.AppendLine("   Customers.CompanyName, TermDocumentPos.PositionID, Articles.ArticleName, Articles.ERPArticleID, Articles.ArticleID, Articles.MeasureUnitID, ");
                sSQL.AppendLine("   TermDocumentPos.OrderedQuantity, Articles.PrimaryCode, ArticleCodes.Code, TermDocumentPos.OrderedQuantity AS PrintQuantity, ISNULL ");
                sSQL.AppendLine("       ((SELECT  SUM(wmsTermDocumentSubPos.PrepareQuantity) AS PrepareQuantity ");
                sSQL.AppendLine("         FROM            wmsTermDocumentSubPos ");
                sSQL.AppendLine("         WHERE        (wmsTermDocumentSubPos.TermDocumentPosRowGuid = TermDocumentPos.rowguid)), 0) AS PrepareQuantity, wmsArticleStoreStock.Stock, ");
                sSQL.AppendLine("   wmsArticleStoreStock.freeStock, ");
                sSQL.AppendLine("   (SELECT        SUM(PrepareQuantity)  FROM TermDocumentSubPosPackage ");
                sSQL.AppendLine("         WHERE        (TermDocumentPosRowGuid = TermDocumentPos.rowguid)) AS PreparePackageQuantity ");
                sSQL.AppendLine("FROM  TermDocumentPos INNER JOIN ");
                sSQL.AppendLine("    TermDocuments ON TermDocuments.rowguid = TermDocumentPos.TermDocumentsRowGuid INNER JOIN ");
                sSQL.AppendLine("    Articles ON TermDocumentPos.ArticleID = Articles.ArticleID INNER JOIN ");
                sSQL.AppendLine("    Customers ON TermDocumentPos.CustomerID = Customers.CustomerID LEFT OUTER JOIN ");
                sSQL.AppendLine("    ArticleCodes ON Articles.ArticleID = ArticleCodes.ArticleID AND ArticleCodes.ArticleCodeTypeID = 0 LEFT OUTER JOIN ");
                sSQL.AppendLine("    wmsArticleStoreStock ON wmsArticleStoreStock.StoreID = TermDocuments.StoreID AND wmsArticleStoreStock.ArticleID = TermDocumentPos.ArticleID ");
                sSQL.AppendLine("WHERE (TermDocumentPos.TermDocumentsRowGuid = @TermDocumentsRowGuid) ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@TermDocumentsRowGuid", pTermDocumentsRowGuid)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllTermDocumentPos] " + exception.Message);
            }

            return lResponse;
        }

        public int SetTermDocument(int pUpdateType, JArray pUpdateData, int pStoreID, string pUserName, int pRespType, string pOption)
        {
            int lResponse = 0;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                int lStatusID = 0;
                int lPriorityID = 0;
                DateTime? lPlannedDate = null;
                string lrowguid = "";

                if (pRespType == 0) // jSon data
                {
                    JObject jo = (JObject)pUpdateData[0]; //JObject.Parse(pUpdateData);
                    //lStoreID = (int)jo["StoreID"];
                    lrowguid = jo.Value<string>("rowguid") ?? null;
                    lStatusID = jo.Value<int?>("StatusID") ?? 0;
                    lPriorityID = jo.Value<int?>("PriorityID") ?? 0;
                    lPlannedDate = jo.Value<DateTime?>("PlannedDate") ?? null;
                }


                sSQL.Remove(0, sSQL.Length);
                if (pUpdateType == 0) //Update
                {
                    sSQL.AppendLine("UPDATE TermDocuments SET StatusID = @StatusID, PriorityID = @PriorityID, PlannedDate = @PlannedDate,  ");
                    sSQL.AppendLine("    UserChange = @UserChange, ModifiedDate = @ModifiedDate ");
                    sSQL.AppendLine("WHERE (rowguid = @rowguid) ");
                    sSQL.AppendLine(" ");


                    SqlParameter[] sqlParams = new SqlParameter[] {
                        new SqlParameter("@rowguid", lrowguid),
                        new SqlParameter("@StatusID", lStatusID),
                        new SqlParameter("@PriorityID", lPriorityID),
                        new SqlParameter("@PlannedDate", lPlannedDate),
                        new SqlParameter("@ModifiedDate", DateTime.Now),
                        new SqlParameter("@UserChange", pUserName)
                    };

                    string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                    if (lResp.Length == 0) { lResponse = 1; }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetTermDocument] " + exception.Message);
            }

            return lResponse;
        }

        public string GetArticleLocationTraffic(int pStoreID, DateTime pSystemDate_From, DateTime pSytemDate_To, string pArticleCode, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT  TOP 300000 Articles.PrimaryCode, Articles.ERPArticleID, ArticleCodes.Code, Articles.ArticleName, Articles.MeasureUnitID, StoreLocations.LocationCode, ");
                sSQL.AppendLine("   StoreLocations.LocationName, Zones.Zone, TermDocumentTypes.TypeName, ArticleLocationTraffic.Credit, ArticleLocationTraffic.Quantity, ");
                sSQL.AppendLine("   ArticleLocationTraffic.OwnerID, TermDocuments.DocumentID, TermDocumentPos.ERPDocumentID, TermDocumentPos.ERPDocDate, ");
                sSQL.AppendLine("   TermDocumentPos.CustomerID, Customers.CompanyName, ArticleLocationTraffic.SystemDate, StoreLocationTypes.LocationTypeName, ");
                sSQL.AppendLine("   StoreLocationsSrc.LocationCode AS LocationCodeSrc ");
                sSQL.AppendLine("FROM ArticleLocationTraffic INNER JOIN ");
                sSQL.AppendLine("   Articles ON ArticleLocationTraffic.ArticleID = Articles.ArticleID INNER JOIN ");
                sSQL.AppendLine("   StoreLocations ON ArticleLocationTraffic.LocationID = StoreLocations.LocationID INNER JOIN ");
                sSQL.AppendLine("   TermDocumentTypes ON ArticleLocationTraffic.DocumentTypeID = TermDocumentTypes.DocumentTypeID LEFT OUTER JOIN ");
                sSQL.AppendLine("   TermDocumentPos ON TermDocumentPos.rowguid = ArticleLocationTraffic.TermDocumentPosRowGuid LEFT OUTER JOIN ");
                sSQL.AppendLine("   TermDocuments ON TermDocuments.rowguid = TermDocumentPos.TermDocumentsRowGuid LEFT OUTER JOIN ");
                sSQL.AppendLine("   ArticleCodes ON ArticleCodes.ArticleID = Articles.ArticleID AND ArticleCodes.ArticleCodeTypeID = 0 LEFT OUTER JOIN ");
                sSQL.AppendLine("   Zones ON StoreLocations.ZoneID = Zones.ZoneID LEFT OUTER JOIN ");
                sSQL.AppendLine("   Customers ON Customers.CustomerID = TermDocumentPos.CustomerID INNER JOIN ");
                sSQL.AppendLine("   StoreLocationTypes ON StoreLocationTypes.LocationTypeID = StoreLocations.LocationTypeID LEFT OUTER JOIN ");
                sSQL.AppendLine("   ArticleLocationTraffic AS ArticleLocationTrafficSrc ON ArticleLocationTrafficSrc.rowguid = ArticleLocationTraffic.SrcALTRowGuid LEFT OUTER JOIN ");
                sSQL.AppendLine("   StoreLocations AS StoreLocationsSrc ON StoreLocationsSrc.LocationID = ArticleLocationTrafficSrc.LocationID ");
                sSQL.AppendLine("WHERE (ArticleLocationTraffic.StoreID = @StoreID) AND (ArticleLocationTraffic.ArticleID IN ");
                sSQL.AppendLine("   (SELECT  ArticleID ");
                sSQL.AppendLine("     FROM  dbo.sp_queryFindMultiArticle(@ArticleCode) AS sp_queryFindMultiArticle_1)) AND (ArticleLocationTraffic.SystemDate BETWEEN ");
                sSQL.AppendLine("     @SystemDate_From AND @SytemDate_To ) ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@SystemDate_From", pSystemDate_From),
                    new SqlParameter("@SytemDate_To", pSytemDate_To),
                    new SqlParameter("@ArticleCode", pArticleCode),
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetArticleLocationTraffic] " + exception.Message);
            }

            return lResponse;
        }

        public string GetArticleStoreLocation(int pStoreID, string pLocationCode, string pArticleCode, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT  ArticleStoreLocation.StoreID, ArticleStoreLocation.ArticleID, ArticleStoreLocation.LocationID, ArticleStoreLocation.Stock, ArticleStoreLocation.ReservedStock, ");
                sSQL.AppendLine("   ArticleStoreLocation.ModifiedDate, StoreLocations.LocationCode, StoreLocations.LocationName, Articles.PrimaryCode, Articles.ERPArticleID, ");
                sSQL.AppendLine("   Articles.ArticleName, Articles.MeasureUnitID, ArticleCodes.Code, StoreLocationTypes.LocationTypeName ");
                sSQL.AppendLine("FROM ArticleStoreLocation INNER JOIN ");
                sSQL.AppendLine("   Articles ON ArticleStoreLocation.ArticleID = Articles.ArticleID INNER JOIN ");
                sSQL.AppendLine("   StoreLocations ON ArticleStoreLocation.LocationID = StoreLocations.LocationID LEFT OUTER JOIN ");
                sSQL.AppendLine("   ArticleCodes ON Articles.ArticleID = ArticleCodes.ArticleID AND ArticleCodes.ArticleCodeTypeID = 0 INNER JOIN ");
                sSQL.AppendLine("   StoreLocationTypes ON StoreLocationTypes.LocationTypeID = StoreLocations.LocationTypeID ");
                sSQL.AppendLine("WHERE (ArticleStoreLocation.StoreID = @StoreID)  AND (StoreLocations.LocationCode LIKE RTRIM(LTRIM(@LocationCode)) + '%') ");
                sSQL.AppendLine("   AND (ArticleStoreLocation.ArticleID IN ");
                sSQL.AppendLine("   (SELECT  ArticleID ");
                sSQL.AppendLine("     FROM  dbo.sp_queryFindMultiArticle(@ArticleCode) AS sp_queryFindMultiArticle_1)) ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@LocationCode", pLocationCode),
                    new SqlParameter("@ArticleCode", pArticleCode),
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetArticleStoreLocation] " + exception.Message);
            }

            return lResponse;
        }

        public string GetArticleLocationFIFO(string pOwnerID, string pLocationCode, string pArticleCode, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_queryArticleLocationFIFO @OwnerID, @ArtcileCode, @LocationCode ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@LocationCode", pLocationCode),
                    new SqlParameter("@ArticleCode", pArticleCode),
                    new SqlParameter("@OwnerID", pOwnerID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetArticleLocationFIFO] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllTermTasks(int pStoreID, string pUserName, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_queryGetTasks @StoreID, @UserName ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@UserName", pUserName),
                    new SqlParameter("@StoreID", pStoreID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllTermTasks] " + exception.Message);
            }

            return lResponse;
        }

        public string GetAllTermTasksByTaskIDAndUN(string pTaskID, string pUserName, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_queryGetTasksByOwnerAndTask @TaskID, @UserName ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@UserName", pUserName),
                    new SqlParameter("@TaskID", pTaskID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetAllTermTasksByTaskIDAndUN] " + exception.Message);
            }

            return lResponse;
        }


        public JObject TermUseTask(string pTaskID, int pStoreID, string pUserName, int pRespType, string pOption)
        {
            JObject lResponse = new JObject();

            lResponse.Add("Resp", -1);

            string ltmpName = Guid.NewGuid().ToString();

            JObject lChSta = CheckTaskStatus(pTaskID, pUserName, pRespType, "");
            if ((int)lChSta["Resp"] == 0)
            {
                JObject lResp1 = SetUserActiveOnDocument(pTaskID, pUserName, pRespType, "");
                switch ((int)lResp1["Resp"])
                {
                    case 0:
                        var ltmp = lResponse.Property("Resp");
                        ltmp.Value = 0;
                        break;
                    case 2:
                        lResponse.Add("RespDesc", cTranslate.GetTransByID(110)); //Uporabnik ni aktiven na opravilu!
                        break;
                    case 12:
                        lResponse.Add("RespDesc", cTranslate.GetTransByID(111)); //Uporabnik ni dodeljen opravilu!
                        break;
                }
                if (((int)lResp1["Resp"] == 0) && !SetTermDocumentStatus(pTaskID, pUserName, 1, pRespType, ""))
                {

                    lResponse.Add("RespDesc", cTranslate.GetTransByID(107)); //Napaka pri pridobivanju podatkov!
                    var ltmp = lResponse.Property("Resp");
                    ltmp.Value = -1;
                }

            }
            else
            {
                var ltmp = lResponse.Property("RespDesc");
                ltmp.Value = (string)lChSta["RespDesc"];
            }

            return lResponse;
        }

        public JObject CheckTaskStatus(string pTaskID, string pUserName, int pRespType, string pOption)
        {
            JObject lResponse = new JObject();

            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_queryGetTasksByOwnerAndTask @TaskID, @UserName ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                            new SqlParameter("@UserName", pUserName),
                            new SqlParameter("@TaskID", pTaskID)
                        };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS != null)
                {
                    if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                    {
                        DataRow lrow;
                        lrow = lTmpDS.Tables[ltmpName].Rows[0];
                        if (int.Parse(lrow["StatusID"].ToString()) <= 63)
                        {
                            lResponse.Add("RespDesc", "");
                            lResponse.Add("DocumentTypeID", (lrow["DocumentTypeID"].ToString()));
                            lResponse.Add("StatusID", (lrow["StatusID"].ToString()));
                            lResponse.Add("StoreID", (lrow["StoreID"].ToString()));
                            lResponse.Add("Resp", 0);
                        }
                        else
                        {
                            lResponse.Add("RespDesc", cTranslate.GetTransByID(109)); // Dokument je zaključen.
                            lResponse.Add("Resp", -1);
                        }
                    }
                    else
                    {
                        lResponse.Add("RespDesc", cTranslate.GetTransByID(108));
                        lResponse.Add("Resp", -1);
                    }
                }
                else
                {
                    lResponse.Add("RespDesc", cTranslate.GetTransByID(108));
                    lResponse.Add("Resp", -1);
                }

            }
            catch (Exception exception)
            {
                lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                lResponse.Add("Resp", -1);
                cLog.WriteError("[CheckTaskStatus] " + exception.Message);
            }

            return lResponse;
        }

        public JObject SetUserActiveOnDocument(string pTaskID, string pUserName, int pRespType, string pOption)
        {
            JObject lResponse = new JObject();

            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("declare @retValue int ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_bookSetOwnerActiveOnDocument @TaskID, @UserName, @retValue OUTPUT ");
                sSQL.AppendLine("select @retValue as retValue ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@UserName", pUserName),
                    new SqlParameter("@TaskID", pTaskID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                    {
                        DataRow lrow;
                        lrow = lTmpDS.Tables[ltmpName].Rows[0];
                        lResponse.Add("RespDesc","");
                        lResponse.Add("Resp", int.Parse(lrow["retValue"].ToString()));
                }
                else
                {
                    lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                    lResponse.Add("Resp", -1);
                }

            }
            catch (Exception exception)
            {
                lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                lResponse.Add("Resp", -1);
                cLog.WriteError("[SetUserActiveOnDocument] " + exception.Message);
            }

            return lResponse;
        }

        public JObject bookSupplyCloseInventory(string pTaskID, string pUserName, int pRespType, string pOption)
        {
            JObject lResponse = new JObject();

            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("declare @retValue int ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_bookSupplyCloseInventory @TaskID, @UserName, @retValue OUTPUT ");
                sSQL.AppendLine("select @retValue as retValue ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@UserName", pUserName),
                    new SqlParameter("@TaskID", pTaskID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    DataRow lrow;
                    lrow = lTmpDS.Tables[ltmpName].Rows[0];
                    int lRespID = int.Parse(lrow["retValue"].ToString());
                    lResponse.Add("Resp", lRespID);
                    switch (lRespID)
                    {
                        case 0:
                            {
                                lResponse.Add("RespDesc", "");
                                break;
                            }
                        case 1:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(112)); //Vsi skladiščniki se še niso odjavili.
                                break;
                            }
                        case 2:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(113)); //Skladiščniki še niso iz svojih lokacij izpraznili zaloge za prevzem.
                                break;
                                /*
                                MessageReturn return2 = new MessageReturn();
                                return2.AddMessage("", "Skladiščniki še niso iz svojih lokacij izpraznili zaloge za prevzem.");
                                foreach (dsSupply.bookSupplyCloseInventoryRow row in table)
                                {
                                    return2.AddMessage("", row.UserName);
                                }
                                ret.MessageRet = return2;
                                break;
                                */
                            }
                    }
                }
                else
                {
                    lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                    lResponse.Add("Resp", -1);
                }

            }
            catch (Exception exception)
            {
                lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                lResponse.Add("Resp", -1);
                cLog.WriteError("[SetUserActiveOnDocument] " + exception.Message);
            }

            return lResponse;
        }

        public bool SetTermDocumentStatus(string pTaskID, string pUserName, int pStatusID, int pRespType, string pOption)
        {
            bool lResponse = false;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_bookSetStatus @TaskID, @StatusID, @UserName ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@UserName", pUserName),
                    new SqlParameter("@TaskID", pTaskID),
                    new SqlParameter("@StatusID", pStatusID)
                };


                string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                if (lResp.Length == 0) { lResponse = true; }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetTermDocumentStatus] " + exception.Message);
            }

            return lResponse;
        }

        public bool SetTermFreeTask(string pTaskID, string pUserName, int pRespType, string pOption)
        {
            bool lResponse = false;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("declare @retValue int ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_bookFreeOwnerActiveOnDocument @TaskID, @UserName, @retValue OUTPUT");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@UserName", pUserName),
                    new SqlParameter("@TaskID", pTaskID)
                };


                string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                if (lResp.Length == 0) { lResponse = true; }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[SetTermFreeTask] " + exception.Message);
            }

            return lResponse;
        }

        public string GetInitSupply(string pTaskID, int pStoreID, string pUserName, int pRespType, string pOption)
        {
            string lResponse = "";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("exec dbo.sp_QueryGetSupplyInfo @TaskID ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@TaskID", pTaskID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                        lResponse = AddHeadDataToResponseData(pRespType, 0, "", lResponse);
                    }
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetInitSupply] " + exception.Message);
            }

            return lResponse;
        }

        public JObject GetTermSupplyScanPosition(string pTaskID, string pScanCode, int pRespType, string pOption)
        {
            JObject lResponse = new JObject();

            string ltmpName = Guid.NewGuid().ToString();
            int lretValue = -1;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("declare @retValue int ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_querySupplyScanPosition @TaskID, @ScanCode, @retValue OUTPUT ");
                sSQL.AppendLine("select @retValue as retValue ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@ScanCode", pScanCode),
                    new SqlParameter("@TaskID", pTaskID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    DataRow lrow, lDatarow;
                    if (lTmpDS.Tables.Count > 1)
                    {
                        lDatarow = lTmpDS.Tables[0].Rows[0];
                        lrow = lTmpDS.Tables[1].Rows[0];

                        lretValue = int.Parse(lrow["retValue"].ToString());
                        lResponse.Add("RespDesc", "");
                        lResponse.Add("Resp", 0);
                        lResponse.Add("PrimaryCode", (lDatarow["PrimaryCode"].ToString().Trim()));
                        lResponse.Add("ArticleCode", (lDatarow["ArticleCode"] as string) ?? "");
                        lResponse.Add("Count", (lDatarow["PostionRows"].ToString()));
                        lResponse.Add("CurrentPosition", (lDatarow["PostionID"].ToString()));
                        lResponse.Add("DocPrepareQuantity", (lDatarow["DocPrepareQuantity"].ToString()));
                        lResponse.Add("ERPArticleID", (lDatarow["ERPArticleID"].ToString()));
                        lResponse.Add("OrderedQuantity", (lDatarow["OrderedQuantity"].ToString()));
                        if (lretValue == 2)
                        { lResponse.Add("ArticeNotExistsOnSupply", 1); }
                        else { lResponse.Add("ArticeNotExistsOnSupply", 0); }
                        lResponse.Add("CountUnInventoryPositions", (int.Parse(lDatarow["PostionRows"].ToString()) - int.Parse(lDatarow["CountInventoryPositions"].ToString())));
                        lResponse.Add("MeasureUnitID", (lDatarow["MeasureUnitID"].ToString().Trim()));

                    }
                    else
                    {
                        lrow = lTmpDS.Tables[ltmpName].Rows[0];
                        lretValue = int.Parse(lrow["retValue"].ToString());
                        if (lretValue == 1)
                        {
                            lResponse.Add("RespDesc", cTranslate.GetTransByID(114));
                            lResponse.Add("Resp", -1);
                        }
                        else
                        {
                            lResponse.Add("RespDesc", "");
                            lResponse.Add("Resp", -1);
                        }
                    }

                }
                else
                {
                    lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                    lResponse.Add("Resp", -1);
                }

            }
            catch (Exception exception)
            {
                lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                lResponse.Add("Resp", -1);
                cLog.WriteError("[GetTermSupplyScanPosition] " + exception.Message);
            }

            return lResponse;
        }

        public JObject GetTermSupplyConfirmPosition(string pTaskID, string pScanCode, string pUserName, float pPrepareQuantity, bool pNewArticleOnSupply, int pRespType, string pOption)
        {
            JObject lResponse = new JObject();

            string ltmpName = Guid.NewGuid().ToString();
            int lretValue = -1;
            float lretMissingQuantity;
            float lretRestQuantity;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("declare @retValue int ");
                sSQL.AppendLine("declare @retMissingQuantity money ");
                sSQL.AppendLine("declare @retRestQuantity money ");
                sSQL.AppendLine("declare @lPrepareQuantity money ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("SET @lPrepareQuantity = @PrepareQuantity ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("BEGIN TRAN T1 ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("Label_1: ");
                sSQL.AppendLine("SET @retMissingQuantity = NULL ");
                sSQL.AppendLine("SET @retRestQuantity = NULL ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_querySupplyScanPosition @TaskID, @ScanCode, @OwnerID, @lPrepareQuantity, @AddNewmsArticleOnSupply, @retValue OUTPUT, @retMissingQuantity OUTPUT, @retRestQuantity OUTPUT ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("IF @retRestQuantity is not null SET @lPrepareQuantity = @retRestQuantity  ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("IF ((@retRestQuantity is null) OR (@retValue <> 0)) ");
                sSQL.AppendLine("BEGIN ");
                sSQL.AppendLine("   IF @retValue = 0 COMMIT TRAN T1 ELSE ROLLBACK TRAN T1  ");
                sSQL.AppendLine("END ");
                sSQL.AppendLine("ELSE ");
                sSQL.AppendLine("BEGIN ");
                sSQL.AppendLine("   GOTO Label_1 ");
                sSQL.AppendLine("END ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("select @retValue as retValue, ISNULL(@retMissingQuantity,0) as retMissingQuantity, ISNULL(@retRestQuantity,0) as retRestQuantity ");
                sSQL.AppendLine(" ");



                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@ScanCode", pScanCode),
                    new SqlParameter("@TaskID", pTaskID),
                    new SqlParameter("@OwnerID", pUserName),
                    new SqlParameter("@PrepareQuantity", pPrepareQuantity),
                    new SqlParameter("@AddNewmsArticleOnSupply", pNewArticleOnSupply)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    DataRow lrow;
                    if (lTmpDS.Tables.Count > 1)
                    {
                        lrow = lTmpDS.Tables[1].Rows[0];
                    }
                    else
                    {
                        lrow = lTmpDS.Tables[ltmpName].Rows[0];
                    }
                    lretValue = int.Parse(lrow["retValue"].ToString());
                    lretMissingQuantity = float.Parse(lrow["retMissingQuantity"].ToString());
                    lretRestQuantity = float.Parse(lrow["retRestQuantity"].ToString());

                    switch (lretValue)
                    {
                        case 0:
                            {
                                lResponse.Add("RespDesc", "");
                                break;
                            }
                        case 1:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(114)); //Artikel ne obstaja
                                break;
                            }
                        case 2:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(115)); //Artikel na prevzemu ne obstaja.
                                break;
                            }
                        case 3:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(116)); //Uporabnik nima dodeljene lokacije.
                                break;
                            }
                        case 4:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(117)); //Zapis v popisno listo prevzema ni uspel.
                                break;
                            }
                        case 5:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(118) + lretMissingQuantity.ToString("R")); //Artikel na prevzemu ne obstaja.
                                break;
                            }
                        case 6:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(119)); //Status dokumenta ni pravilen. Popis ni možen.
                                break;
                            }
                        case 7:
                            {
                                lResponse.Add("RespDesc", cTranslate.GetTransByID(120)); //Artikel ni aktiven. Popis ni možen.
                                break;
                            }
                    }

                    if (lretValue == 0) { lResponse.Add("Resp", 0); } else { lResponse.Add("Resp", -1); }

                }
                else
                {
                    lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                    lResponse.Add("Resp", -1);
                }

            }
            catch (Exception exception)
            {
                lResponse.Add("RespDesc", cTranslate.GetTransByID(107));
                lResponse.Add("Resp", -1);
                cLog.WriteError("[GetTermSupplyConfirmPosition] " + exception.Message);
            }

            return lResponse;
        }


        /////////////////////////////////           Local Core
        public string GetWMSSettingsByName(int pStoreID, string pValueName, int pIsTerminalSett)
        {
            string lResponse = "";

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT  SettingsEnum, StoreID, TerminalSettings, Value, rowguid FROM TermSettings");
                sSQL.AppendLine("WHERE (SystemSettings = 1) AND (StoreID IS NULL OR StoreID = @StoreID) ");
                sSQL.AppendLine(" AND SettingsEnum = @SettingsEnum AND TerminalSettings = @TerminalSettings ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@StoreID", pStoreID),
                    new SqlParameter("@SettingsEnum", pValueName),
                    new SqlParameter("@TerminalSettings", pIsTerminalSett)
                };

                string lErr = "";
                DataTable lDT = lSql.FillDT(sSQL, sqlParams, out lErr);

                if (lDT.Rows.Count > 0)
                {
                    DataRow lrow = lDT.Rows[0];
                    lResponse = (lrow["Value"].ToString());
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetWMSSettingsByName] " + exception.Message);
            }

            return lResponse;
        }

        public int GetWMSLoggedUsers()
        {
            int lResponse = 0;
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("SELECT UserName, [Password], FirstName, LastName, LogedIn, LogedDate, SessionID, Active, TerminalName, StoreID, LocationID, AppActive, AppLogedIn ");
                sSQL.AppendLine("FROM Users (NOLOCK) WHERE LogedIn = 1 ");
                sSQL.AppendLine(" ");

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    lResponse = lTmpDS.Tables[ltmpName].Rows.Count;
                }
            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetWMSLoggedUsers] " + exception.Message);
            }

            return lResponse;
        }

        public int Set_UserUpdate(string pusername, int pLogedIn, string pSessionID, string pTermName, int pAppLogedIn, DateTime? pLogedDate)
        {
            int lResponse = 0;

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.Remove(0, sSQL.Length);
                sSQL.AppendLine("dbo.sp_Users_update @username, @LogedIn, @LogedDate, @SessionID, @TermName, @AppLogedIn ");
                sSQL.AppendLine(" ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@username", pusername),
                    new SqlParameter("@LogedIn", pLogedIn),
                    new SqlParameter("@LogedDate", pLogedDate),
                    new SqlParameter("@SessionID", pSessionID),
                    new SqlParameter("@TermName", pTermName),
                    new SqlParameter("@AppLogedIn", pAppLogedIn)
                };

                string lResp = lSql.ExecuteQuery(sSQL, sqlParams);
                if (lResp.Length == 0) { lResponse = 1; }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[Set_UserUpdate] " + exception.Message);
            }

            return lResponse;
        }

        public string Get_UserBySession(string pusername, string pSessionID, int pRespType)
        {
            string lResponse = "[]";
            string ltmpName = Guid.NewGuid().ToString();

            try
            {
                StringBuilder sSQL = new StringBuilder();
                // get data
                sSQL.AppendLine(" ");
                sSQL.AppendLine("exec dbo.sp_queryFindUserByUserNameSessionID @username, @sessionid ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");

                SqlParameter[] sqlParams = new SqlParameter[] {
                    new SqlParameter("@username", pusername),
                    new SqlParameter("@sessionid", pSessionID)
                };

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, sqlParams, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    if (pRespType == 0)
                    {
                        lResponse = JsonConvert.SerializeObject(lTmpDS.Tables[ltmpName], Newtonsoft.Json.Formatting.Indented);
                    }
                    else if (pRespType == 1)
                    {
                        StringWriter sw = new StringWriter();
                        lTmpDS.Tables[ltmpName].WriteXml(sw);
                        lResponse = sw.ToString();
                    }
                }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[Get_UserBySession] " + exception.Message);
            }

            return lResponse;
        }



        /////////////////////////////////           Local System
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public String DecodeUtf8(string Path)
        {
            String text;
            Byte[] bytes;
            using (StreamReader sr = new StreamReader(Path))
            {
                text = sr.ReadToEnd();
                UTF8Encoding Encoding = new UTF8Encoding();
                bytes = Encoding.GetBytes(text);
                text = Encoding.GetString(bytes);
                return text;
            }
        }


        public void GetFTP_Data(out string pHost, out string pUser, out string pPass)
        {

            pHost = "";
            pUser = "";
            pPass = "";

            try
            {
                StringBuilder sSQL = new StringBuilder();
                string ltmpName = Guid.NewGuid().ToString();

                // get data
                sSQL.Remove(0, sSQL.Length);

                sSQL.AppendLine(" ");
                sSQL.AppendLine("SELECT * FROM _mrt_WebC_Settings ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine(" ");
                sSQL.AppendLine("  ");

                sSQL.AppendLine(" ");

                string lErr = "";
                DataSet lTmpDS = lSql.FillDataSet(ltmpName, sSQL, out lErr);

                if (lTmpDS.Tables[ltmpName].Rows.Count > 0)
                {
                    DataRow lrow = lTmpDS.Tables[ltmpName].Rows[0];
                    pHost = (lrow["acFtpHost"].ToString());
                    pUser = (lrow["acFptUsername"].ToString());
                    pPass = (lrow["acFptPassword"].ToString());
                }

            }
            catch (Exception exception)
            {
                cLog.WriteError("[GetFTP_Data] " + exception.Message);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public String Decode_mrStr(string input)
        {
            String text = "";

            for (int i = 0; i < input.Length; i++)
            {
                text = text + (Char)(Convert.ToUInt16(input[i])+1);
            }

            return text.ToUpper();
        }

        /// <summary>
        /// Add Head data in Response
        /// </summary>
        /// <param name="pDataType"></param>
        /// <param name="pHeadDataID"></param>
        /// <param name="pHeadDataDescr"></param>
        /// <param name="pData"></param>
        /// <returns></returns>
        public String AddHeadDataToResponseData(int pDataType, int pHeadDataID, string pHeadDataDescr, string pData)
        {
            string lResponse = "";

            if (pDataType == 0)
            {
                if (pData.Length == 0) { pData = "[]"; }

                string ltmpHead = "{\"RespID\": \"" + pHeadDataID.ToString() + "\""
                            + ", \"RespDesc\": \"" + pHeadDataDescr + "\"}";
                lResponse = "[" + ltmpHead + "," + pData + "]";
            }
            else
            {
                lResponse = pData;
            }

            return lResponse;
        }
    }
}
