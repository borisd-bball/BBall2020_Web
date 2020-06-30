
namespace mr.bBall_Lib
{

using System;
using System.Configuration;
using System.IO;

    public static class cSettings
    {
//        public static int iWaitSec = 300;
        public static string sLogFile = @"C:\mrWMS_Ant_ws_Log.txt";
        public static string sSQLDatabase = "dgtMobile_Server";
        public static string sSQLPass = "";
        public static string sSQLServer = "mr-avtomatika";
        public static string sSQLUser = "sa";
        public static string sFileLocations = @"C:\";

        public static void GetSettings(string sFilename)
        {
            try
            {
                sLogFile = ConfigurationManager.AppSettings["LogFile"];
                sSQLDatabase = ConfigurationManager.AppSettings["SQLDatabase"];
                sSQLServer = ConfigurationManager.AppSettings["SQLServer"];
                sSQLUser = ConfigurationManager.AppSettings["SQLUser"];
                sSQLPass = ConfigurationManager.AppSettings["SQLPass"];
            }
            catch (Exception exception)
            {
                cLog.WriteError("Get Settings: " + exception.Message);
            }
        }

    }
}

