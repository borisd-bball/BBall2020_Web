using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using mr.bBall_Lib;
using System.Text;

public partial class _Nastavitve : System.Web.UI.Page
{
    protected string msg = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Nastavitve";
            if (!Master.Uporabnik.Pravice.Contains("nastavitve")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            if (IsPostBack)
            {
                try
                {
                    naziv.Text = naziv.Text.Trim();
                    naziv_dolg.Text = naziv_dolg.Text.Trim();
                    telefon.Text = telefon.Text.Trim();
                    email.Text = email.Text.Trim();
                    naslov.Text = naslov.Text.Trim();
                    spletna_stran.Text = spletna_stran.Text.Trim();
                    izdajatelj.Text = izdajatelj.Text.Trim();
                    davcna.Text = davcna.Text.Trim();
                    trr.Text = trr.Text.Trim();
                    banka.Text = banka.Text.Trim();
                    klen_uporabnik.Text = klen_uporabnik.Text.Trim();
                    klen_geslo.Text = klen_geslo.Text.Trim();
                    List<string> l_obvescanje = new List<string>();
                    if (!string.IsNullOrWhiteSpace(obvescanje.Text))
                    {
                        foreach (var item in obvescanje.Text.Split(','))
                        {
                            string s_obvescanje = Splosno.OnlyNumeric(item);
                            s_obvescanje = "+386" + s_obvescanje.Substring(s_obvescanje.Length - 8);
                            if (!l_obvescanje.Contains(s_obvescanje)) l_obvescanje.Add(s_obvescanje);
                        }
                    }
                    obvescanje.Text = string.Join(",", l_obvescanje);

                    List<string> l_povzetek = new List<string>();
                    if (!string.IsNullOrWhiteSpace(povzetek.Text))
                    {
                        foreach (var item in povzetek.Text.Split(','))
                        {
                            string s_povzetek = Splosno.OnlyNumeric(item);
                            s_povzetek = "+386" + s_povzetek.Substring(s_povzetek.Length - 8);
                            if (!l_povzetek.Contains(s_povzetek)) l_povzetek.Add(s_povzetek);
                        }
                    }
                    povzetek.Text = string.Join(",", l_povzetek);

                    List<string> l_promet = new List<string>();
                    if (!string.IsNullOrWhiteSpace(promet.Text))
                    {
                        foreach (var item in promet.Text.Split(','))
                        {
                            string s_promet = Splosno.OnlyNumeric(item);
                            s_promet = "+386" + s_promet.Substring(s_promet.Length - 8);
                            if (!l_promet.Contains(s_promet)) l_promet.Add(s_promet);
                        }
                    }
                    promet.Text = string.Join(",", l_promet);

                    List<string> l_kvote = new List<string>();
                    if (!string.IsNullOrWhiteSpace(kvote.Text))
                    {
                        foreach (var item in kvote.Text.Split(','))
                        {
                            string s_kvote = Splosno.OnlyNumeric(item);
                            s_kvote = "+386" + s_kvote.Substring(s_kvote.Length - 8);
                            if (!l_kvote.Contains(s_kvote)) l_kvote.Add(s_kvote);
                        }
                    }
                    kvote.Text = string.Join(",", l_kvote);

                    if (string.IsNullOrWhiteSpace(naziv.Text)) throw new Exception("Polje Naziv ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(naziv_dolg.Text)) throw new Exception("Polje Dolgi naziv ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(naslov.Text)) throw new Exception("Polje Naslov ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(telefon.Text)) throw new Exception("Polje Telefon ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(email.Text)) throw new Exception("Polje Elektronska pošta ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(spletna_stran.Text)) throw new Exception("Polje Spletna stran ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(izdajatelj.Text)) throw new Exception("Polje Glava računa ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(trr.Text)) throw new Exception("Polje TRR ne sme biti prazno");
                    if (string.IsNullOrWhiteSpace(banka.Text)) throw new Exception("Polje Banka ne sme biti prazno");
                    if (davcna.Text.Length != 8) throw new Exception("Polje Davcna mora biti dolga 8 znakov");
                    int i_davcna;
                    if (!int.TryParse(davcna.Text, out i_davcna)) throw new Exception("Polje Davcna mora biti številka");

                    //string _klen_geslo = Convert.ToString(Nastavitve.Get().Rows[0]["klen_geslo"]);
                    //if (!string.IsNullOrWhiteSpace(klen_geslo.Text)) _klen_geslo = klen_geslo.Text;
                    //if (string.IsNullOrWhiteSpace(klen_uporabnik.Text)) _klen_geslo = "";

                    Nastavitve.Naziv = naziv.Text;
                    Nastavitve.NazivDolg = naziv_dolg.Text;
                    Nastavitve.Telefon = telefon.Text;
                    Nastavitve.EmailFrom = email.Text;
                    Nastavitve.Naslov = naslov.Text;
                    Nastavitve.SpletnaStran = spletna_stran.Text;
                    Nastavitve.Zavezanec = zavezanec.Checked ? 1:0;
                    Nastavitve.Trr = trr.Text;
                    Nastavitve.Banka = banka.Text;
                    Nastavitve.Davcna = davcna.Text;
                    Nastavitve.Osvezevanje = osvezevanje.Checked ? 1:0;


                    Response.Redirect("Nastavitve.aspx?msg=" + HttpUtility.UrlEncode("Podatki shranjeni"));
                }
                catch (Exception er)
                {
                    msg = er.Message + "<br />";
                }
            }

            naziv.Text = Nastavitve.Naziv;
            naziv_dolg.Text = Nastavitve.NazivDolg;
            telefon.Text = Nastavitve.Telefon;
            email.Text = Nastavitve.EmailFrom;
            naslov.Text = Nastavitve.Naslov;
            spletna_stran.Text = Nastavitve.SpletnaStran;
            zavezanec.Checked = Nastavitve.Zavezanec == 1;
            trr.Text = Nastavitve.Trr;
            banka.Text = Nastavitve.Banka;
            davcna.Text = Nastavitve.Davcna;
//            obvescanje.Text = Nastavitve.Obvescanje == 1;
            osvezevanje.Checked = Nastavitve.Osvezevanje == 1;

            if (!string.IsNullOrWhiteSpace(msg)) Master.SetMessage(msg);
        }
        catch (Exception ee)
        {
            Master.SetMessage(ee);
        }
    }
}