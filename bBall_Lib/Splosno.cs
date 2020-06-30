using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using Newtonsoft.Json;

namespace mr.bBall_Lib
{
    public static class Splosno
    {
        public static string AppSQLName = "mrRemote2";
        public static string Verzija = "1.1";
        public static bool Produkcija = Nastavitve.Davcna != "10165096";
        public static char[] NumericCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static bool IsNumeric(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            foreach (char c in input.ToCharArray()) if (!NumericCharacters.Contains(c)) return false;
            return true;
        }
        public static string OnlyNumeric(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            string res = "";
            foreach (char c in input.ToCharArray()) if (NumericCharacters.Contains(c)) res += c;
            return res;
        }
        public static string Capitalize(string input, bool first_only)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (input.Length > 1)
            {
                if (first_only) return char.ToUpper(input[0]) + input.Substring(1).ToLower();
                else
                {
                    string res = "";
                    char previous = ' ';
                    foreach (var c in input.ToCharArray())
                    {
                        if (previous == ' ' || previous == '-') res += char.ToUpper(c);
                        else res += char.ToLower(c);
                        previous = c;
                    }
                    return res;
                }
            }
            else return input.ToUpper();
        }
        public static string Capitalize(string input)
        {
            return Capitalize(input, true);
        }
        public static string Csv(DataTable dt, string table, DateTime datum)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn c in dt.Columns)
            {
                if (c.ColumnName == "password") continue;
                if (table == "racuni" && (c.ColumnName == "qr" || c.ColumnName == "request_xml" || c.ColumnName == "response_xml")) continue;
                sb.Append("=\"" + c.ColumnName + "\";");
            }
            if (table == "racuni")
            {
                sb.Append("=\"ddv95\";");
                sb.Append("=\"ddv22\";");
            }
            if (table == "osnovnasredstva")
            {
                sb.Append("=\"preteklo\";");
                sb.Append("=\"trenutno\";");
                sb.Append("=\"prihodnje\";");
            }
            sb.Append("\r\n");
            foreach (DataRow r in dt.Rows)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    if (c.ColumnName == "password") continue;
                    if (table == "racuni" && (c.ColumnName == "qr" || c.ColumnName == "request_xml" || c.ColumnName == "response_xml")) continue;
                    object o = r[c.ColumnName];
                    if (Convert.IsDBNull(o)) sb.Append(";");
                    else if (c.DataType == typeof(int) || c.DataType == typeof(decimal) || c.DataType == typeof(double) || c.DataType == typeof(float) || c.DataType == typeof(DateTime)) sb.Append(r[c.ColumnName] + ";");
                    else
                    {
                        string temp = fix4csv(Convert.ToString(r[c.ColumnName]));
                        sb.Append((temp.Contains("\r\n") ? "" : "=") + "\"" + temp + "\";");
                    }
                }
                //if (table == "racuni")
                //{
                //    int id_racun = Convert.ToInt32(r["id"]);
                //    DataTable dt_vrstice;
                //    DataTable dt_popravki;
                //    DataTable dt_prihodekodhodek;
                //    DataTable dt_blagajna;
                //    DataTable dt_racun = Racuni.Get(id_racun, out dt_vrstice, out dt_popravki, out dt_prihodekodhodek, out dt_blagajna);
                //    double d_ddv95 = 0;
                //    double d_ddv22 = 0;
                //    foreach (DataRow v in dt_vrstice.Rows)
                //    {
                //        double ddv = Convert.ToDouble(v["ddv"]);
                //        double davek = Convert.ToDouble(v["davek"]);
                //        if (ddv == 9.5) d_ddv95 += davek;
                //        else if (ddv == 22.0) d_ddv22 += davek;
                //    }
                //    sb.Append(d_ddv95 + ";");
                //    sb.Append(d_ddv22 + ";");
                //}
                //if (table == "osnovnasredstva")
                //{
                //    double preteklo;
                //    double prihodnje;
                //    double trenutno = OsnovnaSredstva.GetAmortizacija(Convert.ToDouble(r["vrednost"]), Convert.ToDouble(r["amortizacija"]), Convert.ToDateTime(r["datum"]), datum.Year, out prihodnje, out preteklo);
                //    sb.Append("=\"" + preteklo + "\";");
                //    sb.Append("=\"" + trenutno + "\";");
                //    sb.Append("=\"" + prihodnje + "\";");
                //}
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        public static string Csv(DataTable dt, string table)
        {
            return Csv(dt, table, DateTime.Now);
        }
        private static string fix4csv(string text)
        {
            string res = "";
            res = text.Replace("\"", "").Replace("'", "").Replace(";", "").Replace("<br />", "\r\n");
            while (res.Contains("\r\n\r\n")) res = res.Replace("\r\n\r\n", "\r\n");
            if (res.StartsWith("\r\n")) res = res.Substring(2);
            return res;
        }
        public static string JsSafe(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return System.Web.HttpUtility.HtmlEncode(input.Trim().Replace("'", "\'").Replace("\"", "\\\"").Replace("&", "").Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace("*", ""));
        }
        public static byte[] CreateZipFromDirectory(string path)
        {
            byte[] res = null;
            DirectoryInfo di = new DirectoryInfo(path);
            string zip = AppDomain.CurrentDomain.BaseDirectory + "Cache\\" + Guid.NewGuid() + ".zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(path, zip);
            res = File.ReadAllBytes(zip);
            File.Delete(zip);
            return res;
        }
        public static MemoryStream AddStreamToZipStream(Stream stream, string datoteka, string geslo)
        {
            Stream res = new MemoryStream();
            ZipOutputStream zip_input_stream = new ZipOutputStream(res);
            try
            {
                zip_input_stream.SetLevel(9);
                zip_input_stream.Password = geslo;
                byte[] buffer = new byte[4096];
                ZipEntry zip_entry = new ZipEntry(datoteka);
                zip_entry.DateTime = DateTime.Now;
                zip_input_stream.PutNextEntry(zip_entry);
                using (stream)
                {
                    int source_bytes;
                    do
                    {
                        source_bytes = stream.Read(buffer, 0, buffer.Length);
                        zip_input_stream.Write(buffer, 0, source_bytes);
                    }
                    while (source_bytes > 0);
                }
                zip_input_stream.Finish();
            }
            catch
            {
                res = null;
                zip_input_stream = null;
            }
            res.Position = 0;
            MemoryStream ms = new MemoryStream();
            res.CopyTo(ms);
            return ms;
        }
        public static MemoryStream ExtractZipToStream(Stream zip, string datoteka, string geslo)
        {
            ZipInputStream zip_input_stream = new ZipInputStream(zip);
            zip_input_stream.Password = geslo;
            ZipEntry zip_entry = zip_input_stream.GetNextEntry();
            Stream res = new MemoryStream();
            while (zip_entry != null)
            {
                string entry_filename = zip_entry.Name;
                byte[] buffer = new byte[4096];
                if (entry_filename == datoteka) StreamUtils.Copy(zip_input_stream, res, buffer);
                try
                {
                    zip_entry = zip_input_stream.GetNextEntry();
                }
                catch (Exception)
                {
                    zip_entry = null;
                }
            }
            res.Position = 0;
            MemoryStream ms = new MemoryStream();
            res.CopyTo(ms);
            return ms;
        }
        public static string Truncate(string input, int length)
        {
            if (!string.IsNullOrEmpty(input) && input.Length > length) return input.Remove(length);
            return input;
        }
        public static string Truncate(string input, string addiftruncate, int length)
        {
            if (!string.IsNullOrEmpty(input) && input.Length > length) return input.Remove(length) + addiftruncate;
            return input;
        }
        public static string Truncate(string input, int length, string ifnull)
        {
            if (!string.IsNullOrEmpty(input) && input.Length > length) return input.Remove(length);
            if (string.IsNullOrEmpty(input)) input = ifnull;
            return input;
        }
        public static string Sortiranje(string sort, string sorted, string naslov)
        {
            return Sortiranje(sort, sorted, naslov, null, null);
        }
        public static string Sortiranje(string sort, string sorted, string naslov, int id)
        {
            return Sortiranje(sort, sorted, naslov, id, null);
        }
        public static string Sortiranje1(string sort, string sorted, string naslov, string id)
        {
            return Sortiranje1(sort, sorted, naslov, id, null);
        }
        public static string Sortiranje(string sort, string sorted, string naslov, int? id, int? id_parent)
        {
            if (sorted == sort) sort = sort.Replace(" asc", " 1111").Replace(" desc", " 000").Replace(" 000", " asc").Replace(" 1111", " desc");
            return "<a " + (sorted == sort || sorted == sort.Replace(" asc", " 1111").Replace(" desc", " 000").Replace(" 000", " asc").Replace(" 1111", " desc") ? "class=\"act\"" : "") + " href=\"?" + (id_parent.HasValue ? "id_parent=" + id_parent.Value + "&" : "") + (id.HasValue ? "id=" + id.Value + "&" : "") + "sort=" + sort + "\">" + naslov + "</a>";
        }
        public static string Sortiranje1(string sort, string sorted, string naslov, string id, string id_parent)
        {
            if (sorted == sort) sort = sort.Replace(" asc", " 1111").Replace(" desc", " 000").Replace(" 000", " asc").Replace(" 1111", " desc");
            return "<a " + (sorted == sort || sorted == sort.Replace(" asc", " 1111").Replace(" desc", " 000").Replace(" 000", " asc").Replace(" 1111", " desc") ? "class=\"act\"" : "") + " href=\"?" + (!String.IsNullOrEmpty(id_parent) ? "id_s_parent=" + id_parent + "&" : "") + (!String.IsNullOrEmpty(id) ? "id_s=" + id + "&" : "") + "sort=" + sort + "\">" + naslov + "</a>";
        }

        public static string GetTranslateByID(int pID)
        {
            return cTranslate.GetTransByID(pID);
        }
        public static string SerializeDataTable_json(DataTable pDT)
        {
            return JsonConvert.SerializeObject(pDT, Newtonsoft.Json.Formatting.Indented);
        }
        public static DataTable DeserializeDataTable_json(string pData)
        {
            return JsonConvert.DeserializeObject<DataTable>(pData);
        }
        public static DataTable DeserializeDataTable_xml(string pData)
        {
            //DataTable lDt = new DataTable("Data");
            DataSet lDs = new DataSet();
            StringReader sr = new StringReader(pData);
            //lDt.ReadXml(sr);
            lDs.ReadXml(sr);
            if (lDs.Tables.Count > 0)
            {
                return lDs.Tables[0];
            }
            else
            {
                return null;
            }
        }
        public static string SerializeDataTable_xml(DataTable pDT, bool pUseSchema = false)
        {
            string lResponse = "";
            if (pDT != null)
            {
                pDT.TableName = "RespData";
                StringWriter sw = new StringWriter();
                if (pUseSchema) { pDT.WriteXml(sw, XmlWriteMode.WriteSchema); } else { pDT.WriteXml(sw); }
                lResponse = sw.ToString();
                sw.Dispose();
            }
            return lResponse;
        }

        /// <summary>
        /// Add Head data in Response
        /// </summary>
        /// <param name="pDataType"></param>
        /// <param name="pHeadDataID"></param>
        /// <param name="pHeadDataDescr"></param>
        /// <param name="pData"></param>
        /// <returns></returns>
        public static String AddHeadDataToResponseData(int pDataType, int pHeadDataID, string pHeadDataDescr, string pData)
        {
            string lResponse = "";

            if (pDataType == 0) // JSON
            {
                if (pData.Length == 0) { pData = "[]"; }

                string ltmpHead = "{\"RespID\": \"" + pHeadDataID.ToString() + "\""
                            + ", \"RespDesc\": \"" + pHeadDataDescr + "\"}";
                lResponse = "[" + ltmpHead + "," + pData + "]";
            }
            else if (pDataType == 1) // XML
            {
                DataTable dtabResp = new DataTable("RespAll");
                dtabResp.Columns.Add(new DataColumn("HeadDataID", typeof(int)));
                dtabResp.Columns.Add(new DataColumn("HeadDataDescr", typeof(string)));
                dtabResp.Columns.Add(new DataColumn("Data", typeof(string)));

                DataRow drowResp = dtabResp.NewRow();
                drowResp["HeadDataID"] = pHeadDataID;
                drowResp["HeadDataDescr"] = pHeadDataDescr;
                drowResp["Data"] = pData;

                dtabResp.Rows.Add(drowResp);

                StringWriter sw = new StringWriter();
                dtabResp.WriteXml(sw);
                lResponse = sw.ToString();

                dtabResp.Dispose();
                sw.Dispose();
            }
            else
            {

                lResponse = pData;
            }

            return lResponse;
        }
        public struct xmlData
        {
            public xmlData(string pName, string pValue)
            {
                this.Value = pValue;
                this.Name = pName;
            }
            public string Value;
            public string Name;
        }
        public static string PrepareXMLData(List<xmlData> pData)
        {
            string lReturn = "";

            DataTable dtabResp = new DataTable("mrXML");
            foreach (xmlData lR in pData)
            {
                dtabResp.Columns.Add(new DataColumn(lR.Name, typeof(string)));
            }

            DataRow drowResp = dtabResp.NewRow();
            foreach (xmlData lR in pData)
            {
                drowResp[lR.Name] = lR.Value;
            }
            dtabResp.Rows.Add(drowResp);

            StringWriter sw = new StringWriter();
            dtabResp.WriteXml(sw);
            lReturn = sw.ToString();

            dtabResp.Dispose();
            sw.Dispose();

            return lReturn;
        }
        public static DataTable PrepareDTData(List<xmlData> pData)
        {
            string lReturn = "";

            DataTable dtabResp = new DataTable("mrXML");
            foreach (xmlData lR in pData)
            {
                dtabResp.Columns.Add(new DataColumn(lR.Name, typeof(string)));
            }

            DataRow drowResp = dtabResp.NewRow();
            foreach (xmlData lR in pData)
            {
                drowResp[lR.Name] = lR.Value;
            }
            dtabResp.Rows.Add(drowResp);

            return dtabResp;
        }

    }
}
