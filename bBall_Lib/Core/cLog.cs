namespace mr.bBall_Lib
{
    using System;
    using System.IO;

    public static class cLog
    {
        public static void WriteError(string sMessage)
        {
            try
            {
                FileStream stream = new FileStream(cSettings.sLogFile, FileMode.Append, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                stream.Position = stream.Length;
                writer.WriteLine("[" + DateTime.Now.ToString() + "] => " + sMessage);
                writer.Close();
                stream.Close();
            }
            catch (Exception)
            {
            }
        }

        public static void WriteEvent(string sMessage)
        {
        }
    }
}

