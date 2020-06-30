using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.SessionState;
using System.IO;

namespace mr.bBall_Lib
{
    public class Napake
    {
        public static void Dodaj(string data)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
                {
                    using (SqlCommand comm = new SqlCommand("INSERT INTO Napake (data) VALUES (@data)", conn))
                    {
                        comm.Parameters.AddWithValue("data", data);
                        conn.Open();
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e1)
            {
                WriteError(e1.ToString());
            }
        }

        public static void WriteError(string sMessage)
        {
            try
            {
                FileStream stream = new FileStream(Convert.ToString(ConfigurationManager.AppSettings["LogFile"]), FileMode.Append, FileAccess.Write);
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

    }
}
