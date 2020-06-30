namespace mr.bBall_Lib
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    public class cSQL
    {
        public SqlConnection scnMain;

        public cSQL()
        {
            scnMain = new SqlConnection();
        }
        public string ConnectSQL(string pAppName = "")
        {
            string lResp = "";

            if (scnMain != null && scnMain.State == ConnectionState.Closed)
            {
                scnMain.ConnectionString = "Data Source=%SERVER%;Initial Catalog=%DATABASE%;User ID=%USER%;Password=%PASS%;";
                scnMain.ConnectionString = scnMain.ConnectionString.Replace("%SERVER%", cSettings.sSQLServer);
                scnMain.ConnectionString = scnMain.ConnectionString.Replace("%DATABASE%", cSettings.sSQLDatabase); //cSettings.sSQLDatabase);
                scnMain.ConnectionString = scnMain.ConnectionString.Replace("%USER%", cSettings.sSQLUser);
                scnMain.ConnectionString = scnMain.ConnectionString.Replace("%PASS%", cSettings.sSQLPass);
                if (pAppName.Length > 0) { scnMain.ConnectionString = scnMain.ConnectionString + "Application Name=" + pAppName + ";"; }
                try
                {
                    scnMain.Open();
                    StringBuilder sSQL = new StringBuilder();

                    sSQL.Append("USE " + cSettings.sSQLDatabase);
                    ExecuteQuery(sSQL);

                }
                catch (Exception exception)
                {
                    lResp = "Napaka pri povezovanju na SQL strežnik!";
                    cLog.WriteError(exception.Message);
                }
            }

            return lResp;
        }

        public string GetCSForReports(string pAppName = "")
        {
            string lResp = "";

            lResp = "Server=%SERVER%;DataBase=%DATABASE%;User ID=%USER%;Password=%PASS%;";
            lResp = lResp.Replace("%SERVER%", cSettings.sSQLServer);
            lResp = lResp.Replace("%DATABASE%", cSettings.sSQLDatabase);
            lResp = lResp.Replace("%USER%", cSettings.sSQLUser);
            lResp = lResp.Replace("%PASS%", cSettings.sSQLPass);
            if (pAppName.Length > 0) { lResp = lResp + "Application Name=" + pAppName + ";"; }

            return lResp;
        }

        public void DisconnectSQL()
        {
            try
            {
                if (scnMain.State != ConnectionState.Closed)
                {
                    scnMain.Close();
                }
            }
            catch { }
        }

        public string ExecuteQuery(StringBuilder sSQL)
        {
            SqlParameter[] param = new SqlParameter[] { null };
            return ExecuteQuery(sSQL, param);
        }

        public string ExecuteQuery(StringBuilder sSQL, SqlParameter[] param)
        {
            string lResponse = "";
            SqlCommand command = new SqlCommand();
            command.Connection = scnMain;
            command.CommandTimeout = 300;
            command.CommandText = sSQL.ToString();
            if ((param.Length > 0) && (param[0] != null))
            {
                command.Parameters.AddRange(param);
                foreach (SqlParameter parameter in command.Parameters)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                //cLog.WriteError("E: [ExecuteQuery] - " + exception.Message);
                lResponse = "E: [ExecuteQuery] - " + exception.Message;
            }
            command.Dispose();
            return lResponse;
        }

        public string ExecuteQuery(StringBuilder sSQL, SqlParameter[] param, out int pID)
        {
            string lResponse = "";
            pID = 0;
            SqlCommand command = new SqlCommand();
            command.Connection = scnMain;
            command.CommandTimeout = 300;
            command.CommandText = sSQL.ToString();
            if ((param.Length > 0) && (param[0] != null))
            {
                command.Parameters.AddRange(param);
                foreach (SqlParameter parameter in command.Parameters)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
            try
            {
                pID = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception exception)
            {
                //cLog.WriteError("E: [ExecuteQuery] - " + exception.Message);
                lResponse = "E: [ExecuteQuery] - " + exception.Message;
            }
            command.Dispose();
            return lResponse;
        }

        public DataSet FillDataSet(string sTable, StringBuilder sSQL, out string pError)
        {
            SqlParameter[] param = new SqlParameter[] { null };
            return FillDataSet(sTable, sSQL, param, out pError);
        }

        public DataSet FillDataSet(string sTable, StringBuilder sSQL, SqlParameter param, out string pError)
        {
            SqlParameter[] parameterArray = new SqlParameter[] { param };
            return FillDataSet(sTable, sSQL, parameterArray, out pError);
        }

        public DataSet FillDataSet(string sTable, StringBuilder sSQL, SqlParameter[] param, out string pError)
        {
            pError = "";
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet dataSet = new DataSet();
            command.Connection = scnMain;
            command.CommandTimeout = 300;
            command.CommandText = sSQL.ToString();
            if ((param.Length > 0) && (param[0] != null))
            {
                command.Parameters.AddRange(param);
                foreach (SqlParameter parameter in command.Parameters)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
            adapter.SelectCommand = command;
            try
            {
                adapter.Fill(dataSet, sTable);
            }
            catch (Exception exception)
            {
                dataSet = null;
                pError = "E: [FillDataSet] - " + exception.Message;
            }
            command.Dispose();
            return dataSet;
        }

        public DataTable FillDT(StringBuilder sSQL, out string pError)
        {
            SqlParameter[] param = new SqlParameter[] { null };
            return FillDT(sSQL, param, out pError);
        }

        public DataTable FillDT(StringBuilder sSQL, SqlParameter[] param, out string pError)
        {
            pError = "";
            SqlCommand command = new SqlCommand();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable dt = new DataTable();
            command.Connection = scnMain;
            command.CommandTimeout = 300;
            command.CommandText = sSQL.ToString();
            if ((param.Length > 0) && (param[0] != null))
            {
                command.Parameters.AddRange(param);
                foreach (SqlParameter parameter in command.Parameters)
                {
                    if (parameter.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
            adapter.SelectCommand = command;
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception exception)
            {
                dt = null;
                pError = "E: [FillDT] - " + exception.Message;
            }
            command.Dispose();
            return dt;
        }

        public byte[] GetDataValue(StringBuilder sSQL, SqlParameter param)
        {
            SqlParameter[] parameterArray = new SqlParameter[] { param };
            return GetDataValue(sSQL, parameterArray);
        }

        public byte[] GetDataValue(StringBuilder sSQL, SqlParameter[] param)
        {
            byte[] buffer;
            SqlCommand command = new SqlCommand();
            command.Connection = scnMain;
            command.CommandTimeout = 300;
            command.CommandText = sSQL.ToString();
            if ((param.Length > 0) && (param[0] != null))
            {
                command.Parameters.AddRange(param);
            }
            try
            {
                buffer = (byte[]) command.ExecuteScalar();
            }
            catch (Exception)
            {
                buffer = new byte[0];
            }
            command.Dispose();
            return buffer;
        }

        public string GetStringValue(StringBuilder sSQL, SqlParameter[] param)
        {
            SqlCommand command = new SqlCommand();
            string str = "";
            command.Connection = scnMain;
            command.CommandTimeout = 300;
            command.CommandText = sSQL.ToString();
            if ((param.Length > 0) && (param[0] != null))
            {
                command.Parameters.AddRange(param);
            }
            try
            {
                str = command.ExecuteScalar().ToString();
            }
            catch (Exception)
            {
                str = "";
            }
            command.Dispose();
            return str;
        }

        public string GetStringValue(StringBuilder sSQL, SqlParameter param)
        {
            SqlParameter[] parameterArray = new SqlParameter[] { param };
            return GetStringValue(sSQL, parameterArray);
        }
    }
}

