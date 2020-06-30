using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.SessionState;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using SMSPDULib;
using System.Xml.Serialization;

namespace mr.bBall_Lib
{
    public class Sporocila
    {
        public static int SporocilaTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SporocilaTimeout"]);
        public static int SporocilaUra = Convert.ToInt32(ConfigurationManager.AppSettings["SporocilaUra"]);
        public struct Response
        {
            public Response(string Resp, string RespDesc, string RespData)
            {
                this.Resp = Resp;
                this.RespDesc = RespDesc;
                this.RespData = RespData;
            }
            public string Resp;
            public string RespDesc;
            public string RespData;
        }
        public struct Sporocilo
        {
            public Sporocilo(string tb, int Id, string Message_id, string Message_ref, string Message_number, DateTime Message_Time, DateTime Insert_Time, string Message)
            {
                this.tb = tb;
                this.Id = Id;
                this.Message_id = Message_id;
                this.Message_ref = Message_ref;
                this.Message_number = Message_number;
                this.Message_Time = Message_Time;
                this.Insert_Time = Insert_Time;
                this.Message = Message;
            }
            public string tb;
            public int Id;
            public string Message_id;
            public string Message_ref;
            public string Message_number;
            public DateTime Message_Time;
            public DateTime Insert_Time;
            public string Message;
        }
        [Serializable()]
        [System.Xml.Serialization.XmlRoot("XMLDATA")]
        public struct Vrste
        {
            [XmlArray("ROWDATA")]
            [XmlArrayItem("ROW", typeof(Vrsta))]
            public Vrsta[] Items;
        }
        [Serializable()]
        public struct Vrsta
        {
            public Vrsta(int Id, string Command_Type, string Command_ref, int Command_Priority, string Command, DateTime Execute_Time, DateTime Insert_Time, string Command_Response, string Command_status, string Command_System, string Command_Error)
            {
                this.Id = Id;
                this.Command_Type = Command_Type;
                this.Command_ref = Command_ref;
                this.Command_Priority = Command_Priority;
                this.Command = Command;
                this.Execute_Time = Execute_Time;
                this.Insert_Time = Insert_Time;
                this.Command_Response = Command_Response;
                this.Command_status = Command_status;
                this.Command_System = Command_System;
                this.Command_Error = Command_Error;
            }
            [System.Xml.Serialization.XmlElement("ID")]
            public int Id;
            [System.Xml.Serialization.XmlElement("Command_Type")]
            public string Command_Type;
            [System.Xml.Serialization.XmlElement("Command_ref")]
            public string Command_ref;
            [System.Xml.Serialization.XmlElement("Command_Priority")]
            public int Command_Priority;
            [System.Xml.Serialization.XmlElement("Command")]
            public string Command;
            [System.Xml.Serialization.XmlElement(DataType = "dateTime", ElementName = "Execute_Time")]
            public DateTime Execute_Time;
            [System.Xml.Serialization.XmlElement("Insert_Time")]
            public DateTime Insert_Time;
            [System.Xml.Serialization.XmlElement("Command_Response")]
            public string Command_Response;
            [System.Xml.Serialization.XmlElement("Command_status")]
            public string Command_status;
            [System.Xml.Serialization.XmlElement("Command_System")]
            public string Command_System;
            [System.Xml.Serialization.XmlElement("Command_Error")]
            public string Command_Error;
        }
        public static DataTable Get()
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter a = new SqlDataAdapter("select TOP 1000 * from sporocila where sistem=@sistem order by id desc", ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                a.SelectCommand.Parameters.AddWithValue("sistem", ConfigurationManager.AppSettings["Skupina"]);
                a.Fill(dt);
            }
            return dt;
        }
        public static DataTable GetAll()
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter a = new SqlDataAdapter("select TOP 1000 * from sporocila order by id desc", ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                a.Fill(dt);
            }
            return dt;
        }
        public static DataTable GetAllFilterByStatus(string pStatus)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter a = new SqlDataAdapter("select TOP 100 * from sporocila where stanje=@stanje order by id desc", ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                a.SelectCommand.Parameters.AddWithValue("stanje", pStatus);
                a.Fill(dt);
            }
            return dt;
        }
        public static DataTable GetAllFilterByStatusAndGSM(string pStatus, string pGSM, DateTime pMaxDateToCheck)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter a = new SqlDataAdapter("select * from sporocila where stanje=@stanje and stevilka=@stevilka and datum >= @datum order by id desc", ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                a.SelectCommand.Parameters.AddWithValue("stanje", pStatus);
                a.SelectCommand.Parameters.AddWithValue("stevilka", pGSM);
                a.SelectCommand.Parameters.AddWithValue("datum", pMaxDateToCheck);
                a.Fill(dt);
            }
            return dt;
        }
        public static DataTable GetFilterBySesionID(string pSesionID, string pGSM)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter a = new SqlDataAdapter("select * from sporocila where id_sporocilo=@id_sporocilo and stevilka=@stevilka ", ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                a.SelectCommand.Parameters.AddWithValue("id_sporocilo", pSesionID);
                a.SelectCommand.Parameters.AddWithValue("stevilka", pGSM);
                a.Fill(dt);
            }
            return dt;
        }
        public static int Dodaj(string id_sporocilo, DateTime datum, string stevilka, string besedilo, string stanje, string sistem, string napaka, string referenca)
        {
            int id = 0;
            string lIDSporocilo = id_sporocilo.Split(',')[0].ToString();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand("declare @id int;insert into sporocila (id_sporocilo,datum,stevilka,besedilo,stanje,sistem,napaka,referenca) values (@id_sporocilo,@datum,@stevilka,@besedilo,@stanje,@sistem,@napaka,@referenca);set @id=scope_identity();select @id;", conn))
                {
                    comm.Parameters.AddWithValue("id_sporocilo", lIDSporocilo);
                    comm.Parameters.AddWithValue("datum", datum);
                    comm.Parameters.AddWithValue("stevilka", stevilka);
                    comm.Parameters.AddWithValue("besedilo", besedilo);
                    comm.Parameters.AddWithValue("stanje", stanje);
                    comm.Parameters.AddWithValue("sistem", sistem);
                    comm.Parameters.AddWithValue("napaka", napaka);
                    comm.Parameters.AddWithValue("referenca", referenca);
                    conn.Open();
                    id = Convert.ToInt32(comm.ExecuteScalar());
                    conn.Close();
                }
            }
            return id;
        }
        public static void Popravi(int id, string id_sporocilo, DateTime datum, string stevilka, string besedilo, string stanje, string sistem, string napaka, string referenca, int duration)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand("update sporocila set id_sporocilo=@id_sporocilo,datum=@datum,stevilka=@stevilka,besedilo=@besedilo,stanje=@stanje,sistem=@sistem,napaka=@napaka,referenca=@referenca,duration=@duration where id=@id", conn))
                {
                    comm.Parameters.AddWithValue("id", id);
                    comm.Parameters.AddWithValue("id_sporocilo", id_sporocilo);
                    comm.Parameters.AddWithValue("datum", datum);
                    comm.Parameters.AddWithValue("stevilka", stevilka);
                    comm.Parameters.AddWithValue("besedilo", besedilo);
                    comm.Parameters.AddWithValue("stanje", stanje);
                    comm.Parameters.AddWithValue("sistem", sistem);
                    comm.Parameters.AddWithValue("napaka", napaka);
                    comm.Parameters.AddWithValue("referenca", referenca);
                    comm.Parameters.AddWithValue("duration", duration);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public static void PopraviStatus(string id_sporocilo, string stanje)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand("update sporocila set stanje=@stanje, datum=@datum where id_sporocilo=@id_sporocilo", conn))
                {
                    comm.Parameters.AddWithValue("id_sporocilo", id_sporocilo);
                    comm.Parameters.AddWithValue("datum", DateTime.Now);
                    comm.Parameters.AddWithValue("stanje", stanje);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public static void Brisi(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand("delete from sporocila where id=@id and sistem=@sistem", conn))
                {
                    comm.Parameters.AddWithValue("id", id);
                    comm.Parameters.AddWithValue("sistem", ConfigurationManager.AppSettings["Skupina"]);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public static int Zadnje()
        {
            int id = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
            {
                using (SqlCommand comm = new SqlCommand("select top 1 id from sporocila where sistem=@sistem order by id desc", conn))
                {
                    comm.Parameters.AddWithValue("sistem", ConfigurationManager.AppSettings["Skupina"]);
                    conn.Open();
                    id = Convert.ToInt32(comm.ExecuteScalar());
                    conn.Close();
                }
            }
            return id;
        }

        public static void Osvezi()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/getdata_messages");
            request.Method = "POST";
            request.Timeout = 15000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = null;
            using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"MESSAGE_NUMBER\": \"%\",\"MESSAGE_LAST_ID\": \"0\"}")))
            {
                byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
            }
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
            {
                resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
            }
            Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
            if (Resp.Resp == "0")
            {
                // 
                //string data = Encoding.UTF8.GetString(Convert.FromBase64String(Resp.RespData));
                //if (data.EndsWith("\r\n\r\nOK\r\n")) data = data.Substring(0, data.Length - 8);
                //if (data.EndsWith("\r\nOK\r\n")) data = data.Substring(0, data.Length - 6);
                //if (!string.IsNullOrWhiteSpace(data))
                //{
                //    string[] lines = data.Split(new string[] { "+CMGL:" }, StringSplitOptions.RemoveEmptyEntries);
                //    if (lines.Length > 1)
                //    {
                //        data = "";
                //        for (int i = 1; i < lines.Length; i++) data += "+CMGL:" + lines[i];
                //        string id = "";
                //        string stevilka = "";
                //        DateTime datum = DateTime.Now;
                //        string besedilo = "";
                //        List<Sporocilo> sporocila = new List<Sporocilo>();
                //        foreach (var line in data.Replace("\n", "").Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries))
                //        {
                //            #region
                //            if (line.StartsWith("+CMGL:"))
                //            {
                //                if (!string.IsNullOrWhiteSpace(besedilo))
                //                {
                //                    var sporocilo = sporocila.FirstOrDefault(l => l.Datum == datum && l.Stevilka == stevilka);
                //                    if (string.IsNullOrWhiteSpace(sporocilo.Id)) sporocila.Add(new Sporocilo(id, stevilka, datum, besedilo));
                //                    else
                //                    {
                //                        Sporocilo s = new Sporocilo(sporocilo.Id + "," + id, sporocilo.Stevilka, sporocilo.Datum, sporocilo.Besedilo + besedilo);
                //                        sporocila.Remove(sporocilo);
                //                        sporocila.Add(s);
                //                    }
                //                }
                //                besedilo = "";
                //                string[] segs = line.Replace("+CMGL:", "").Replace("\"", "").Split(new string[] { "," }, StringSplitOptions.None);
                //                id = segs[0].Trim();
                //                stevilka = segs[2].Trim();
                //                string[] dates = segs[4].Split('/');
                //                string[] times = segs[5].Split(new string[] { ":", "+" }, StringSplitOptions.None);
                //                datum = new DateTime(2000 + Convert.ToInt32(dates[0]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[2]), Convert.ToInt32(times[0]), Convert.ToInt32(times[1]), Convert.ToInt32(times[2]));
                //                id += "_" + datum.ToString("yyyyMMddHHmmssfff");
                //            }
                //            else besedilo += (string.IsNullOrWhiteSpace(besedilo) ? "" : "<br />") + line.Trim();
                //            #endregion
                //        }
                //        if (!string.IsNullOrWhiteSpace(besedilo))
                //        {
                //            #region
                //            var sporocilo = sporocila.FirstOrDefault(l => l.Datum == datum && l.Stevilka == stevilka);
                //            if (string.IsNullOrWhiteSpace(sporocilo.Id)) sporocila.Add(new Sporocilo(id, stevilka, datum, besedilo));
                //            else
                //            {
                //                Sporocilo s = new Sporocilo(sporocilo.Id + "," + id, sporocilo.Stevilka, sporocilo.Datum, sporocilo.Besedilo + besedilo);
                //                sporocila.Remove(sporocilo);
                //                sporocila.Add(s);
                //            }
                //            #endregion
                //        }
                //        if (sporocila.Count > 0)
                //        {
                //            #region
                //            DataTable dt = GetAll();
                //            foreach (var sporocilo in sporocila)
                //            {
                //                string sporocilo_stevilka = hex2string(sporocilo.Stevilka);
                //                string sporocilo_besedilo = hex2string(sporocilo.Besedilo);
                //                if (dt.Select("id_sporocilo='" + sporocilo.Id + "'").Length == 0)
                //                {
                //                    string sql;
                //                    string sistem = "";
                //                    stevilka = Splosno.OnlyNumeric(sporocilo_stevilka);
                //                    if (stevilka.Length < 7) stevilka = "+38670" + stevilka;
                //                    else if (stevilka.Length < 8) stevilka = "+386" + stevilka.TrimStart('0');
                //                    else stevilka = "+386" + stevilka.Substring(stevilka.Length - 8);
                //                    stevilka = stevilka.Replace("+", "00");
                //                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SporocilaRd"]))
                //                    {
                //                        #region nastavitve
                //                        sql = "";
                //                        foreach (string rd in ConfigurationManager.AppSettings["SporocilaRd"].Split(',')) sql += "select '" + rd.Replace("-", "") + "' sistem,obvescanje from [" + rd + "].[dbo].[Nastavitve] union select '" + rd.Replace("-", "") + "' sistem,obvescanje from [" + rd + "].[dbo].[Revirji] union ";
                //                        sql = "select * from (" + sql.Substring(0, sql.Length - 7) + ") x where obvescanje<>'' and obvescanje like '%'+@stevilka+'%'";
                //                        using (DataTable dt_nastavitve = new DataTable())
                //                        {
                //                            using (SqlDataAdapter a = new SqlDataAdapter(sql, ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
                //                            {
                //                                a.SelectCommand.Parameters.AddWithValue("stevilka", stevilka);
                //                                a.Fill(dt_nastavitve);
                //                            }
                //                            if (dt_nastavitve.Rows.Count > 0) sistem = Convert.ToString(dt_nastavitve.Rows[0]["sistem"]);
                //                        }
                //                        #endregion
                //                    }
                //                    if (string.IsNullOrWhiteSpace(sistem))
                //                    {
                //                        #region posiljatelj
                //                        DataRow[] dr = dt.Select("stevilka='" + stevilka + "'");
                //                        if (dr.Length > 0) sistem = Convert.ToString(dr[0]["sistem"]);
                //                        #endregion
                //                    }
                //                    Dodaj(sporocilo.Id, sporocilo.Datum, stevilka, sporocilo_besedilo, "prejeto", sistem, "", "");
                //                }
                //                foreach (string id_sporocilo in sporocilo.Id.Split(',')) Odstrani(id_sporocilo.Split('_')[0]);
                //            }
                //            #endregion
                //        }
                //    }
                //}

            }
            else throw new Exception(Resp.RespDesc);
        }
        public static void Osvezi_Poslano()
        {
            string lData = "";
            string lError = "";
            Boolean lProccessAll = true;

            // jemlje cakajoce, ki so v vrsti - MAX 100 na enkrat
            DataTable dt = GetAllFilterByStatus("caka");
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    DateTime datum = Convert.ToDateTime(r["datum"]);
                    if (datum < DateTime.Now)
                    {
                        int id = Convert.ToInt32(r["id"]);
                        string id_sporocilo = Convert.ToString(r["id_sporocilo"]);
                        string stevilka = Convert.ToString(r["stevilka"]);
                        string besedilo = Convert.ToString(r["besedilo"]);
                        string stanje = Convert.ToString(r["stanje"]);
                        string sistem = Convert.ToString(r["sistem"]);
                        string napaka = Convert.ToString(r["napaka"]);
                        string referenca = Convert.ToString(r["referenca"]);
                        string[] lRefIDs = referenca.Split(';');

                        lError = "";
                        lProccessAll = true;
                        foreach (string lRefID in lRefIDs)
                        {
                            lData = "";
                            lData = Preberi_Commands(lRefID);
                            if (!string.IsNullOrWhiteSpace(lData))
                            {
                                Sporocila.Vrste lCommands;
                                XmlSerializer serializer = new XmlSerializer(typeof(Sporocila.Vrste));
                                lCommands = (Sporocila.Vrste)serializer.Deserialize(new StringReader(lData));
                                if (lCommands.Items.Length > 0)
                                {
                                    Sporocila.Vrsta lCommand = lCommands.Items[0];
                                    string lstatus = lCommand.Command_status.Trim().ToUpper();
                                    if (lstatus != "WAIT")
                                    {
                                        if (lstatus == "END_ERR")
                                        {
                                            lError = lError + Environment.NewLine + lCommand.Command_Error;
                                        }
                                    }
                                    else { lProccessAll = false; }
                                }
                                else { lProccessAll = false; }
                            }
                            else { lProccessAll = false; }
                        }

                        if (lProccessAll == true)
                        {
                            if (lError.Length > 0)
                            {
                                Popravi(id, id_sporocilo, datum, stevilka, besedilo, "napaka", sistem, lError, referenca, 0);
                            }
                            else
                            {
                                Popravi(id, id_sporocilo, datum, stevilka, besedilo, "poslano", sistem, lError, referenca, 0);
                            }

                            // Izbriši commande iz liste na sms centrali
                            foreach (string lRefID in lRefIDs)
                            {
                                Odstrani_Commands(lRefID);
                            }
                        }
                        else if (datum < DateTime.Now.AddHours(-3)) 
                        {
                            Popravi(id, id_sporocilo, datum, stevilka, besedilo, "napaka", sistem, napaka, referenca, 0);
                            // Izbriši commande iz liste na sms centrali
                            foreach (string lRefID in lRefIDs)
                            {
                                Odstrani_Commands(lRefID);
                            }

                            Email.Send("mirkor@mr-avtomatika.com", "Skulpture (>3h)", "Sistem: " + sistem + "<br />Sporocilo: " + id + (string.IsNullOrWhiteSpace(napaka) ? "" : " (" + napaka + ")"));
                        }
                    }
                }
                catch (Exception e)
                {
                    Napake.Dodaj(e.ToString());
                    if (!e.ToString().Contains("The underlying connection was closed")) Email.Send("info@mr-avtomatika.com", "Skulpture", "Napaka pošlji: " + e);
                }
            }
        }
        public static void Poslji()
        {
            //DataTable dt = GetAll();
            //foreach (DataRow r in dt.Select("stanje='poslji'", "id asc"))

            // jemlje neposlane, MAX 2 na enkrat
            DataTable dt = GetAllFilterByStatus("poslji");
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    DateTime datum = Convert.ToDateTime(r["datum"]);
                    if (datum < DateTime.Now)
                    {
                        int id = Convert.ToInt32(r["id"]);
                        string id_sporocilo = Convert.ToString(r["id_sporocilo"]);
                        string stevilka = Convert.ToString(r["stevilka"]);
                        string besedilo = Convert.ToString(r["besedilo"]);
                        string stanje = Convert.ToString(r["stanje"]);
                        string sistem = Convert.ToString(r["sistem"]);
                        string napaka = Convert.ToString(r["napaka"]);
                        string referenca = Convert.ToString(r["referenca"]);
                        if (datum > DateTime.Now.AddHours(-3)) Poslji_txtSimple(datum, id, id_sporocilo, stevilka, besedilo, stanje, sistem, napaka, referenca);
                        else
                        {
                            Popravi(id, id_sporocilo, datum, stevilka, besedilo, "napaka", sistem, napaka, referenca, 0);
                            Email.Send("mirkor@mr-avtomatika.com", "Skulpture (>3h)", "Sistem: " + sistem + "<br />Sporocilo: " + id + (string.IsNullOrWhiteSpace(napaka) ? "" : " (" + napaka + ")"));
                        }
                    }
                }
                catch (Exception e)
                {
                    Napake.Dodaj(e.ToString());
                    if (!e.ToString().Contains("The underlying connection was closed")) Email.Send("info@mr-avtomatika.com", "Skulpture", "Napaka pošlji: " + e);
                }
            }
        }
        public static void Poslji(DateTime datum, int id, string id_sporocilo, string stevilka, string besedilo, string stanje, string sistem, string napaka, string referenca)
        {
            DateTime now = DateTime.Now;
            string RespResp = "0";
            string RespRespData = "";
            string RespRespDesc = "";
            string lRespReferenceID = "";
            besedilo = besedilo.Trim();
            if (!string.IsNullOrWhiteSpace(besedilo))
            {
                SMS sms = new SMS();
                sms.Direction = SMSDirection.Submited;
                sms.PhoneNumber = stevilka;
                sms.ValidityPeriod = new TimeSpan(1, 0, 0);
                sms.MessageEncoding = SMS.SMSEncoding.UCS2;
                sms.UserDataStartsWithHeader = true;
                sms.Message = besedilo;
                foreach (var line in sms.ComposeLongSMS())
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/setdata_commands_add");
                    request.Method = "POST";
                    request.Timeout = 15000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] byteArray = null;
                    string lData = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"SMS_LENGTH\": \"" + ((line.Length / 2) - 1) + "\", \"SMS_DATA\": \"" + line + "\"}"));
                    using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"COMMAND_TYPE\": \"SMS_SEND_PDU\", \"COMMAND_DATA\": \"" + lData + "\", \"COMMAND_SYSTEM\": \"" + sistem + "\", \"COMMAND_PRIORITY\": \"0\", \"COMMAND_EXEC_TIME\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}")))
                    {
                        byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
                    }
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    string resp = "";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        resp = reader.ReadToEnd();
                        reader.Close();
                    }
                    using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
                    {
                        resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
                    }
                    Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
                    RespResp = Resp.Resp;
                    if (!string.IsNullOrWhiteSpace(Resp.RespData)) RespRespData = (Resp.RespData);
                    if (!string.IsNullOrWhiteSpace(Resp.RespDesc)) RespRespDesc = (Resp.RespDesc);
                    if (RespResp == "0") { lRespReferenceID = lRespReferenceID + RespRespData + ";"; }

                    if (RespResp != "0" && !RespRespData.ToLower().Contains("timeout during operation") && !RespRespDesc.ToLower().Contains("timeout during operation")) break;
                }
            }
            if (RespResp != "0" && !RespRespData.ToLower().Contains("timeout during operation") && !RespRespDesc.ToLower().Contains("timeout during operation"))
            {
                Popravi(id, id_sporocilo, datum, stevilka, besedilo, stanje, sistem, napaka + "\r\n\r\n\r\n\r\nDatum:\r\n" + now + "\r\n\r\nResp.Resp:\r\n" + RespResp + "\r\n\r\nResp.RespData:\r\n" + RespRespData + "\r\n\r\nResp.RespDesc:\r\n" + RespRespDesc, referenca, (int)(DateTime.Now - now).TotalMilliseconds);
            }
            else
            {
                Popravi(id, id_sporocilo, datum, stevilka, besedilo, "caka", sistem, napaka, lRespReferenceID, (int)(DateTime.Now - now).TotalMilliseconds);
            }
        }
        public static void Poslji_noUnicode(DateTime datum, int id, string id_sporocilo, string stevilka, string besedilo, string stanje, string sistem, string napaka, string referenca)
        {
            DateTime now = DateTime.Now;
            string RespResp = "0";
            string RespRespData = "";
            string RespRespDesc = "";
            string lRespReferenceID = "";
            besedilo = besedilo.Trim();
            if (!string.IsNullOrWhiteSpace(besedilo))
            {
                SMS sms = new SMS();
                sms.Direction = SMSDirection.Submited;
                sms.PhoneNumber = stevilka;
                sms.ValidityPeriod = new TimeSpan(1, 0, 0);
                sms.MessageEncoding = SMS.SMSEncoding._7bit;
                sms.UserDataStartsWithHeader = true;
                sms.Message = besedilo;
                foreach (var line in sms.ComposeLongSMS())
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/setdata_commands_add");
                    request.Method = "POST";
                    request.Timeout = 60000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] byteArray = null;
                    string lData = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"SMS_LENGTH\": \"" + ((line.Length / 2) - 1) + "\", \"SMS_DATA\": \"" + line + "\"}"));
                    using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"COMMAND_TYPE\": \"SMS_SEND_PDU\", \"COMMAND_DATA\": \"" + lData + "\", \"COMMAND_SYSTEM\": \"" + sistem + "\", \"COMMAND_PRIORITY\": \"0\", \"COMMAND_EXEC_TIME\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}")))
                    {
                        byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
                    }
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    string resp = "";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        resp = reader.ReadToEnd();
                        reader.Close();
                    }
                    using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
                    {
                        resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
                    }
                    Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
                    RespResp = Resp.Resp;
                    if (!string.IsNullOrWhiteSpace(Resp.RespData)) RespRespData = (Resp.RespData);
                    if (!string.IsNullOrWhiteSpace(Resp.RespDesc)) RespRespDesc = (Resp.RespDesc);
                    if (RespResp == "0") { lRespReferenceID = lRespReferenceID + RespRespData + ";"; }

                    if (RespResp != "0" && !RespRespData.ToLower().Contains("timeout during operation") && !RespRespDesc.ToLower().Contains("timeout during operation")) break;
                }
            }
            if (RespResp != "0" && !RespRespData.ToLower().Contains("timeout during operation") && !RespRespDesc.ToLower().Contains("timeout during operation"))
            {
                Popravi(id, id_sporocilo, datum, stevilka, besedilo, stanje, sistem, napaka + "\r\n\r\n\r\n\r\nDatum:\r\n" + now + "\r\n\r\nResp.Resp:\r\n" + RespResp + "\r\n\r\nResp.RespData:\r\n" + RespRespData + "\r\n\r\nResp.RespDesc:\r\n" + RespRespDesc, referenca, (int)(DateTime.Now - now).TotalMilliseconds);
            }
            else
            {
                Popravi(id, id_sporocilo, datum, stevilka, besedilo, "caka", sistem, napaka, lRespReferenceID, (int)(DateTime.Now - now).TotalMilliseconds);
            }
        }
        public static void Poslji_txtSimple(DateTime datum, int id, string id_sporocilo, string stevilka, string besedilo, string stanje, string sistem, string napaka, string referenca)
        {
            DateTime now = DateTime.Now;
            string RespResp = "0";
            string RespRespData = "";
            string RespRespDesc = "";
            string lRespReferenceID = "";
            besedilo = besedilo.Trim();
            if (!string.IsNullOrWhiteSpace(besedilo))
            {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/setdata_commands_add");
                    request.Method = "POST";
                    request.Timeout = 15000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] byteArray = null;
                    string lData = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"SMS_NUMBER\": \"" + stevilka + "\", \"SMS_DATA\": \"" + besedilo + "\"}"));
                    using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"COMMAND_TYPE\": \"SMS_SEND_GSM\", \"COMMAND_DATA\": \"" + lData + "\", \"COMMAND_SYSTEM\": \"" + sistem + "\", \"COMMAND_PRIORITY\": \"0\", \"COMMAND_EXEC_TIME\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}")))
                    {
                        byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
                    }
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    string resp = "";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        resp = reader.ReadToEnd();
                        reader.Close();
                    }
                    using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
                    {
                        resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
                    }
                    Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
                    RespResp = Resp.Resp;
                    if (!string.IsNullOrWhiteSpace(Resp.RespData)) RespRespData = (Resp.RespData);
                    if (!string.IsNullOrWhiteSpace(Resp.RespDesc)) RespRespDesc = (Resp.RespDesc);
                    if (RespResp == "0") { lRespReferenceID = lRespReferenceID + RespRespData + ";"; }
            }
            if (RespResp != "0" && !RespRespData.ToLower().Contains("timeout during operation") && !RespRespDesc.ToLower().Contains("timeout during operation"))
            {
                Popravi(id, id_sporocilo, datum, stevilka, besedilo, stanje, sistem, napaka + "\r\n\r\n\r\n\r\nDatum:\r\n" + now + "\r\n\r\nResp.Resp:\r\n" + RespResp + "\r\n\r\nResp.RespData:\r\n" + RespRespData + "\r\n\r\nResp.RespDesc:\r\n" + RespRespDesc, referenca, (int)(DateTime.Now - now).TotalMilliseconds);
            }
            else
            {
                Popravi(id, id_sporocilo, datum, stevilka, besedilo, "caka", sistem, napaka, lRespReferenceID, (int)(DateTime.Now - now).TotalMilliseconds);
            }
        }
        public static string Preberi_Commands(string pRefID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/getdata_commands_byrefid");
            request.Method = "POST";
            request.Timeout = 15000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = null;
            using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"COMMAND_REFID\": \"" + pRefID + "\"}")))
            {
                byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
            }
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
            {
                resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
            }
            Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
            if (Resp.Resp != "0") throw new Exception(Resp.RespDesc);
            return (Encoding.UTF8.GetString(Convert.FromBase64String(Resp.RespData)));
        }
        public static string Preberi(string id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/getdata_messages_byid");
            request.Method = "POST";
            request.Timeout = 30000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = null;
            using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"MESSAGE_ID\": \"" + id + "\"}")))
            {
                byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
            }
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
            {
                resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
            }
            Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
            if (Resp.Resp != "0") throw new Exception(Resp.RespDesc);
            return (Resp.RespData);
        }
        public static void Odstrani_Commands(string pRefID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/setdata_commands_delete");
            request.Method = "POST";
            request.Timeout = 15000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = null;
            using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"COMMAND_REFID\": \"" + pRefID + "\"}")))
            {
                byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
            }
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
            {
                resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
            }
            Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
            if (Resp.Resp != "0")
            {
                throw new Exception(Resp.RespDesc);
            }
        }
        public static string ModemInfo(string pATCommand, string pSistem, int pPriority)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/setdata_commands_add");
            request.Method = "POST";
            request.Timeout = 15000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = null;
            string lData = Convert.ToBase64String(Encoding.UTF8.GetBytes(""+ pATCommand + ""));
            using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"COMMAND_TYPE\": \"MODEM_GET_AT\", \"COMMAND_DATA\": \"" + lData + "\", \"COMMAND_SYSTEM\": \"" + pSistem + "\", \"COMMAND_PRIORITY\": \""+ pPriority.ToString() + "\", \"COMMAND_EXEC_TIME\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}")))
            {
                byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
            }
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
            {
                resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
            }
            Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
            if (Resp.Resp != "0") throw new Exception(Resp.RespDesc);
            else
            {
                string data = (Resp.RespData);
                return data;
            }
        }
        public static string ModemInfo_wait(string pATCommand, string pSistem, int pPriority, double pTimeOut = 10000)
        {
            string lResp = "";
            string lData = "";
            string lRespID = ModemInfo(pATCommand, pSistem, pPriority);

            DateTime lDt = DateTime.Now.AddMilliseconds(pTimeOut);
            if (!string.IsNullOrWhiteSpace(lRespID))
            {
                while (lDt > DateTime.Now)
                {
                    lData = "";
                    lData = Preberi_Commands(lRespID);
                    if (!string.IsNullOrWhiteSpace(lData))
                    {
                        Sporocila.Vrsta lCommand = (Sporocila.Vrsta)JsonConvert.DeserializeObject(lData, typeof(Sporocila.Vrsta));
                        string lstatus = lCommand.Command_status.Trim().ToUpper();
                        if (lstatus != "WAIT")
                        {
                            if (lstatus == "END_ERR") { throw new Exception(lCommand.Command_Error); }
                            else
                            {
                                lResp = lCommand.Command_Response;
                                break;
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }

            return lResp;
        }
        public static string Actions(string pAction)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrlAdmin"] + "/setdata_action");
            request.Method = "POST";
            request.Timeout = 30000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = null;
            using (Stream data = new MemoryStream(Encoding.UTF8.GetBytes("{\"ActionID\": \"" + pAction + "\"}")))
            {
                byteArray = Encoding.UTF8.GetBytes(Convert.ToBase64String(Splosno.AddStreamToZipStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray()));
            }
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            using (Stream data = new MemoryStream(Convert.FromBase64String(resp)))
            {
                resp = Encoding.UTF8.GetString(Splosno.ExtractZipToStream(data, "data.txt", ConfigurationManager.AppSettings["SporocilaGeslo"]).ToArray());
            }
            resp = resp.Replace("\r\n", "");
            Sporocila.Response Resp = (Sporocila.Response)JsonConvert.DeserializeObject(resp, typeof(Sporocila.Response));
            if (Resp.Resp != "0") throw new Exception(Resp.RespDesc);
            else
            {
                string data = Encoding.UTF8.GetString(Convert.FromBase64String(Resp.RespData));
                if (data.EndsWith("\r\n\r\nOK\r\n")) data = data.Substring(0, data.Length - 8);
                return data;
            }
        }
        public static string Ping()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrl"] + "/getdata_ping");
            request.Method = "POST";
            request.Timeout = 30000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = Encoding.UTF8.GetBytes("{}");
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            return resp;
        }
        public static string PingAdmin()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["SporocilaUrlAdmin"] + "/getdata_ping");
            request.Method = "POST";
            request.Timeout = 30000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] byteArray = Encoding.UTF8.GetBytes("{}");
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resp = reader.ReadToEnd();
                reader.Close();
            }
            return resp;
        }
        private static string hex2string(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2) bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return Encoding.BigEndianUnicode.GetString(bytes);
        }
        public static void Start()
        {
            DateTime lResetDate = DateTime.Now.AddHours(2);
            try
            {
                while (true)
                {
                    Thread.Sleep(SporocilaTimeout);
                    try
                    {
                        Osvezi();
                    }
                    catch (Exception e)
                    {
                        Napake.Dodaj(e.ToString());
                        if (e.Message.Contains("The operation has timed out"))
                        {
                            try
                            {
                                // Restart SMS centrale
                                Actions("SYS_RESET");
                                Thread.Sleep(30000);
                                Napake.Dodaj("[Osvezi:] Sistem Reset!");
                            }
                            catch (Exception e1)
                            {
                                Napake.Dodaj("[Sistem Reset:]" + e1.ToString());
                            }

                        }
                    }
                    try
                    {
                        Poslji();
                    }
                    catch (Exception e)
                    {
                        Napake.Dodaj(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Napake.Dodaj(e.ToString());
            }
        }
    }
}
