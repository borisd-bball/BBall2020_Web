using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Xml;
public partial class _Pregled : System.Web.UI.Page
{
    protected string _racuni = "";
    protected string _prihodkiodhodki = "";
    protected string _blagajne = "";
    protected string _clani = "";
    protected string _Skulpture = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Pregled";
            Master.EnableDatePicker();
            if (!Master.Uporabnik.Pravice.Contains("pregled")) throw new Exception("Nimate pravice!");

//            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString))
//            {
//                using (SqlCommand comm = new SqlCommand("", conn))
//                {
//                    conn.Open();
//                    comm.Parameters.AddWithValue("year", Master.Date.Year);
//                    #region racuni
//                    if (Master.Uporabnik.Pravice.Contains("racuni"))
//                    {
//                        comm.CommandText = "select oblika,tip,sum(znesek) znesek,count(*) c from racuni where datepart(year,datum_racuna)=@year and izdan=1 group by oblika,tip order by sum(znesek) desc";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {

//                            int cs = 0;
//                            double zs = 0;
//                            while (r.Read())
//                            {
//                                int c = Convert.ToInt32(r["c"]);
//                                double z = Convert.ToDouble(r["znesek"]);
//                                _racuni += "<tr><td>" + (Convert.ToString(r["tip"]) == "vezana_knjiga" ? "Vezana knjiga" : "Elektronski račun") + " (" + r["oblika"] + ")</td><td style=\"text-align:right;\">" + c + "</td><td style=\"text-align:right;\"><b>" + z.ToString("#,##0.00") + "</b></td></tr>";
//                                cs += c;
//                                zs += z;
//                            }
//                            _racuni += "<tr><td>Skupaj</td><td style=\"text-align:right;\">" + cs + "</td><td style=\"text-align:right;\"><b>" + zs.ToString("#,##0.00") + "</b></td></tr>";
//                        }
//                        _racuni += "</table><table class=\"table noalt sep\" style=\"margin-top:0;\"><tr><th colspan=\"3\">Zadnjih 5</th></tr>";
//                        comm.CommandText = "select top 5 * from racuni where datepart(year,datum_racuna)=@year and izdan=1 order by datum_racuna desc";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {
//                            while (r.Read())
//                            {
//                                _racuni += "<tr class=\"pt\" onclick=\"window.location='Racun.aspx?id=" + r["id"] + "'\"><td>" + Splosno.Truncate(Convert.ToString(r["stranka_naziv"]), "...", 25) + "</td><td colspan=\"2\" style=\"text-align:right;\"><b>" + Convert.ToDouble(r["znesek"]).ToString("#,##0.00") + "</b></td></tr>";
//                            }
//                        }
//                    }
//                    #endregion
//                    #region prihodkiodhodki
//                    if (Master.Uporabnik.Pravice.Contains("prihodki_odhodki"))
//                    {
//                        comm.CommandText = "select tip,sum(znesek_pridobitno) znesek_pridobitno,sum(znesek_nepridobitno) znesek_nepridobitno,count(*) c from prihodkiodhodki where datepart(year,datum)=@year group by tip order by tip desc";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {
//                            _prihodkiodhodki += "<tr><td></td><td style=\"text-align:right;\"></td><td style=\"text-align:right;\"><b>Pridobitno</b></td><td style=\"text-align:right;\"><b>Nepridobitno</b></td></tr>";
//                            int cs = 0;
//                            double zs_pridobitno = 0;
//                            double zs_nepridobitno = 0;
//                            while (r.Read())
//                            {
//                                int c = Convert.ToInt32(r["c"]);
//                                double z_pridobitno = Convert.ToDouble(r["znesek_pridobitno"]);
//                                double z_nepridobitno = Convert.ToDouble(r["znesek_nepridobitno"]);
//                                _prihodkiodhodki += "<tr><td>" + (Convert.ToString(r["tip"]) == "prihodek" ? "Prihodki" : "Odhodki") + "</td><td style=\"text-align:right;\">" + c + "</td><td style=\"text-align:right;\"><b>" + z_pridobitno.ToString("#,##0.00") + "</b></td><td style=\"text-align:right;\"><b>" + z_nepridobitno.ToString("#,##0.00") + "</b></td></tr>";
//                                cs += c;
//                                zs_pridobitno += z_pridobitno;
//                                zs_nepridobitno += z_nepridobitno;
//                            }
//                            _prihodkiodhodki += "<tr><td>Skupaj</td><td style=\"text-align:right;\">" + cs + "</td><td style=\"text-align:right;\"><b>" + zs_pridobitno.ToString("#,##0.00") + "</b></td><td style=\"text-align:right;\"><b>" + zs_nepridobitno.ToString("#,##0.00") + "</b></td></tr>";
//                        }
//                        _prihodkiodhodki += "</table><table class=\"table noalt sep\" style=\"margin-top:0;\"><tr><th colspan=\"3\">Zadnjih 5</th></tr>";
//                        comm.CommandText = "select top 5 * from prihodkiodhodki where datepart(year,datum)=@year order by datum desc";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {
//                            while (r.Read())
//                            {
//                                _prihodkiodhodki += "<tr class=\"pt\" onclick=\"window.location='PrihodekOdhodek.aspx?id=" + r["id"] + "'\"><td>" + Splosno.Truncate(Convert.ToString(r["stranka_naziv"]), "...", 20) + "</td><td style=\"text-align:right;\"><b>" + Convert.ToDouble(r["znesek_pridobitno"]).ToString("#,##0.00") + "</b></td><td style=\"text-align:right;\"><b>" + Convert.ToDouble(r["znesek_nepridobitno"]).ToString("#,##0.00") + "</b></td></tr>";
//                            }
//                        }
//                    }
//                    #endregion
//                    #region blagajne
//                    if (Master.Uporabnik.Pravice.Contains("blagajne"))
//                    {
//                        comm.CommandText = "select CASE WHEN znesek>0 THEN 'Prejemek' ELSE 'Izdatek' END tip,sum(znesek) znesek,count(*) c from blagajne where datepart(year,datum)=@year group by CASE WHEN znesek>0 THEN 'Prejemek' ELSE 'Izdatek' END";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {
//                            int cs = 0;
//                            double zs = 0;
//                            while (r.Read())
//                            {
//                                int c = Convert.ToInt32(r["c"]);
//                                double z = Convert.ToDouble(r["znesek"]);
//                                _blagajne += "<tr><td>" + r["tip"] + "</td><td style=\"text-align:right;\">" + c + "</td><td style=\"text-align:right;\"><b>" + z.ToString("#,##0.00") + "</b></td></tr>";
//                                cs += c;
//                                zs += z;
//                            }
//                            _blagajne += "<tr><td>Skupaj</td><td style=\"text-align:right;\">" + cs + "</td><td style=\"text-align:right;\"><b>" + zs.ToString("#,##0.00") + "</b></td></tr>";
//                        }
//                        _blagajne += "</table><table class=\"table noalt sep\" style=\"margin-top:0;\"><tr><th colspan=\"3\">Zadnjih 5</th></tr>";
//                        comm.CommandText = "select top 5 * from blagajne where datepart(year,datum)=@year order by datum desc";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {
//                            while (r.Read())
//                            {
//                                _blagajne += "<tr class=\"pt\" onclick=\"window.location='Blagajna.aspx?id=" + r["id"] + "'\"><td>" + Splosno.Truncate(r["stranka_naziv"] + (string.IsNullOrWhiteSpace(Convert.ToString(r["stranka_naziv"])) || string.IsNullOrWhiteSpace(Convert.ToString(r["naziv"])) ? "" : " | ") + r["naziv"], "...", 25) + "</td><td style=\"text-align:right;\"><b>" + Convert.ToDouble(r["znesek"]).ToString("#,##0.00") + "</b></td></tr>";
//                            }
//                        }
//                    }
//                    #endregion
//                    #region clani
//                    if (Master.Uporabnik.Pravice.Contains("clani"))
//                    {
//                        int _clani_c = 0;
//                        bool flag = true;
//                        comm.CommandText = "select aktiven,count(*) c from clani group by aktiven order by aktiven desc";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {
//                            int cs = 0;
//                            if (r.HasRows)
//                            {
//                                while (r.Read())
//                                {
//                                    int c = Convert.ToInt32(r["c"]);
//                                    _clani += "<tr><td>" + (Convert.ToBoolean(r["aktiven"]) ? "Aktivni" : "Neaktivni") + "</td><td style=\"text-align:right;\"><b>" + c + "</b></td></tr>";
//                                    _clani_c++;
//                                    cs += c;
//                                }
//                                _clani += "<tr><td>Skupaj</td><td style=\"text-align:right;\"><b>" + cs + "</b></td></tr></table><table class=\"table noalt sep\" style=\"margin-top:0;\">";
//                            }
//                            else flag = false;
//                        }
//                        if (flag)
//                        {
//                            comm.CommandText = @"select 1 o,'manj kot 20' interval,count(*) c from clani where rojstvo<>'1900-01-01' and aktiven=1 and DATEDIFF(year,rojstvo,current_timestamp)<20
//union select 2 o,'20 do 40' interval,count(*) c from clani where rojstvo<>'1900-01-01' and aktiven=1 and DATEDIFF(year,rojstvo,current_timestamp) between 20 and 40
//union select 3 o,'40 do 60' interval,count(*) c from clani where rojstvo<>'1900-01-01' and aktiven=1 and DATEDIFF(year,rojstvo,current_timestamp) between 40 and 60
//union select 4 o,'več kot 60' interval,count(*) c from clani where rojstvo<>'1900-01-01' and aktiven=1 and DATEDIFF(year,rojstvo,current_timestamp)>60 order by o";
//                            using (SqlDataReader r = comm.ExecuteReader())
//                            {
//                                _clani += "<tr><th" + (_clani_c == 1 ? " style=\"padding-top:43px;\"" : "") + " colspan=\"2\">Starostna struktura aktivnih članov</th></tr>";
//                                while (r.Read())
//                                {
//                                    _clani += "<tr><td>" + r["interval"] + "</td><td style=\"text-align:right;\"><b>" + r["c"] + "</b></td></tr>";
//                                }
//                            }
//                            comm.CommandText = "select avg(DATEDIFF(year,rojstvo,current_timestamp)) from clani where rojstvo<>'1900-01-01' and aktiven=1";
//                            _clani += "<tr><td>Povprečna starost</td><td style=\"text-align:right;\"><b>" + comm.ExecuteScalar() + "</b> let</td></tr>";
//                        }
//                    }
//                    #endregion
//                    #region skulpture
//                    if (Master.Uporabnik.Pravice.Contains("skulpture"))
//                    {
//                        comm.CommandText = "select * from Sku_Skulptures order by Name desc";
//                        using (SqlDataReader r = comm.ExecuteReader())
//                        {
//                            while (r.Read())
//                            {
//                                _Skulpture += "<tr class=\"pt\" onclick=\"window.location='Skulp_Modules.aspx?id=" + r["id"] + "'\"><td><b>" + Splosno.Truncate(Convert.ToString(r["Name"]), "...", 25) + "</b></td><td colspan=\"2\" style=\"text-align:left;\">" + Splosno.Truncate(Convert.ToString(r["ReferenceName"]), "...", 25) + "</td></tr>";
//                            }
//                        }
//                    }
//                    #endregion

//                    conn.Close();
//                }
//            }
        }
        catch (Exception ee)
        {
            Master.SetMessage(ee);
        }
    }
}