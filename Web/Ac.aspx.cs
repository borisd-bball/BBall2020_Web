using Microsoft.Web.Services3.Security.Cryptography;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mr.bBall_Lib;
using mr_Remote_Lib.com.inetis.ddv;

public partial class Ac : System.Web.UI.Page
{
    private string _s;
    private Uporabnik _u;
    public struct Odgovor
    {
        public Odgovor(string title, string message, string type)
        {
            this.title = title;
            this.message = message;
            this.type = type;
        }
        public string title;
        public string message;
        public string type;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            _u = new Uporabnik(Session);
            string t = Request.QueryString["t"];
            if (!_u.AppLogedIn && t != "geslo") throw new Exception("Nimate pravice!");
            _s = (Request.QueryString["s"] ?? "").ToLower().Trim();
            switch (t)
            {
                case "inetis": inetis(); break;
                case "tipi": tipi(); break;
                case "vrstica_tipi": vrstica_tipi(); break;
                case "qr": qr(); break;
                case "revizije": revizije(); break;
                case "stranke": stranke(); break;
                case "stranka": stranka(); break;
                case "poste": poste(); break;
                case "artikli": artikli(); break;
                case "artikel": artikel(); break;
                case "artikli_skupine": artikli_skupine(); break;
                case "glave": glave(); break;
                case "glava": glava(); break;
                case "set": set(); break;
                case "serije": serije(); break;
                case "stevilka": stevilka(); break;
                case "enote": enote(); break;
                case "geslo": geslo(); break;
                case "pravice": pravice(); break;
                case "skupine": skupine(); break;
                case "revirji": revirji(); break;
                case "sporocila": sporocila(); break;
                case "blagajne": blagajne(); break;
                case "persistence": persistence(); break;
                case "transakcije": transakcije(); break;
                case "tip": tip(); break;
                default: break;
            }
        }
        catch (ThreadAbortException ee) { }
        catch (Exception ee)
        {
            Response.Write(HttpUtility.JavaScriptStringEncode(ee.Message));
        }
    }

    private void inetis()
    {
        string s = Request.QueryString["s"];
        string kljuc = _u.Username + "_inetis";
        var stranke = new List<Stranke.Customer>();
        if (Predpomnilnik.Keys.Contains(kljuc))
        {
            stranke = ((List<Stranke.Customer>)Predpomnilnik.Get(kljuc));
            if (stranke.Any(l => l.acShortTitle == s))
            {
                stranke = new List<Stranke.Customer>(new Stranke.Customer[] { stranke.First(l => l.acShortTitle == s) });
                Response.Write(JsonConvert.SerializeObject(stranke));
                return;
            }
        }
        using (Iskalnik iskalnik = new Iskalnik())
        {
            stranke.Clear();
            Predpomnilnik.Remove(kljuc);
            iskalnik.Url = ConfigurationManager.AppSettings["InetisUrl"];
            object[] res = iskalnik.Isci(_s);
            foreach (object item in res)
            {
                xmlZavezanec zavezanec = (xmlZavezanec)item;
                string[] temp = zavezanec.xmlNaslov.Split(',');
                string naslov = temp[0].Trim();
                string posta = "";
                string kraj = "";
                if (temp.Length > 1)
                {
                    temp = temp[1].Trim().Split(' ');
                    posta = temp[0].Trim();
                    if (temp.Length > 1)
                    {
                        for (int i = 1; i < temp.Length; i++) kraj = kraj + temp[i].Trim() + " ";
                        kraj = kraj.Trim();
                    }
                }

                int lPostID = 0;
                var lP = Posts.fGet("", posta);
                if (lP.Count > 0)
                {
                    lPostID = lP[0].anPostID;
                }

                string ldavcna = zavezanec.xmlZavezanecZaDDV ? zavezanec.xmlDavcnaStevilka.Substring(2) : zavezanec.xmlDavcnaStevilka;

                stranke.Add(new Stranke.Customer(0, 0, lPostID, 0,
                                                    zavezanec.xmlNaziv,
                                                    zavezanec.xmlNaziv,
                                                    naslov, kraj, zavezanec.xmlZavezanecZaDDV ? "SI" : "", ldavcna, zavezanec.xmlZavezanecZaDDV ? "Z":"K","", Convert.ToString(zavezanec.xmlStatus), "","",1,DateTime.Now,_u.Id,0));
            }
            Predpomnilnik.Add(kljuc, stranke);
        }
        Response.Write(JsonConvert.SerializeObject(stranke.Select(l => l.acShortTitle).OrderBy(l => l).Take(20)));

    }

    private void tipi()
    {
        //Response.Write(JsonConvert.SerializeObject(Clani.Tipi.OrderBy(l => l.oznaka).Select(l => l.oznaka + " | " + l.naziv)));
    }
    private void vrstica_tipi()
    {
        //if (_s == "prihodek") Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Tipi.Where(l => l.EndsWith(" P"))));
        //else Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Tipi.Where(l => !l.EndsWith(" P"))));
    }
    private void qr()
    {
        //if (_u.Pravice.Contains("racuni")) Response.BinaryWrite((byte[])Racuni.Get(Convert.ToInt32(_s)).Rows[0]["qr"]);
        //else if (_u.Pravice.Contains("prodaje")) Response.BinaryWrite((byte[])Prodaje.Get(Convert.ToInt32(_s), _u).Rows[0]["qr"]);
    }
    private void revizije()
    {
        //if (!_u.Pravice.Contains("revizije")) throw new Exception("Nimate pravice!");
        //byte[] res = Splosno.CreateZipFromDirectory(AppDomain.CurrentDomain.BaseDirectory + "Revizija");
        //Response.ContentType = "application/zip; name=Revizija.zip";
        //Response.AddHeader("content-transfer-encoding", "binary");
        //Response.AddHeader("Content-Disposition", "attachment; filename=Revizija.zip");
        //Response.OutputStream.Write(res, 0, res.Length);
        //Response.Flush();
        //Response.End();
    }
    private void stranke()
    {
        //if (Request.QueryString["m"] == "dovolilnice")
        //{
        //    if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(Dovolilnice.Stranke.Select(l => l.naziv).OrderBy(l => l)));
        //    else Response.Write(JsonConvert.SerializeObject(Dovolilnice.Stranke.Where(l => l.naziv.ToLower().Contains(_s)).Select(l => l.naziv).OrderBy(l => l).Take(20)));
        //}
        //else if (Request.QueryString["m"] == "blagajne")
        //{
        //    if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(Stranke.Seznam.Where(l => l.vir != "stranke").Select(l => l.naziv).OrderBy(l => l)));
        //    else Response.Write(JsonConvert.SerializeObject(Stranke.Seznam.Where(l => l.naziv.ToLower().Contains(_s)).Select(l => l.naziv).OrderBy(l => l).Take(20)));
        //}
        //else if (Request.QueryString["m"] == "clani")
        //{
        //    if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(Stranke.Seznam.Where(l => l.vir == "clani").Select(l => l.naziv).OrderBy(l => l)));
        //    else Response.Write(JsonConvert.SerializeObject(Stranke.Seznam.Where(l => l.vir == "clani" && l.naziv.ToLower().Contains(_s)).Select(l => l.naziv).OrderBy(l => l).Take(20)));
        //}
        //else if (Request.QueryString["m"] == "racuni")
        //{
        //    if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(Stranke.Seznam.Where(l => l.vir == "stranke").Select(l => l.naziv).OrderBy(l => l)));
        //    else Response.Write(JsonConvert.SerializeObject(Stranke.Seznam.Where(l => l.naziv.ToLower().Contains(_s)).Select(l => l.naziv).OrderBy(l => l).Take(20)));
        //}
    }
    private void stranka()
    {
        //if (Request.QueryString["m"] == "dovolilnice") Response.Write(JsonConvert.SerializeObject(Dovolilnice.Stranke.Where(l => l.naziv.ToLower() == _s)));
        //else Response.Write(JsonConvert.SerializeObject(Stranke.Seznam.Where(l => l.naziv.ToLower() == _s)));
    }
    private void poste()
    {
        if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(Stranke.Poste.Select(l => l.Key + " | " + l.Value).OrderBy(l => l)));
        else Response.Write(JsonConvert.SerializeObject(Stranke.Poste.Where(l => l.Key.StartsWith(_s) || l.Value.ToLower().Contains(_s)).Select(l => l.Key + " | " + l.Value).OrderBy(l => l).Take(20)));
    }
    private void artikli()
    {
        //if (Request.QueryString["m"] == "prihodkiodhodki")
        //{
        //    if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Artikli.Select(l => l.naziv).OrderBy(l => l)));
        //    else Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Artikli.Where(l => l.naziv.ToLower().Contains(_s)).Select(l => l.naziv).OrderBy(l => l).Take(20)));
        //}
        //else if (Request.QueryString["m"] == "blagajne")
        //{
        //    List<Artikli.Artikel> res = Artikli.Seznam.Where(l => l.podrocje == "blagajne").ToList();
        //    if (!string.IsNullOrWhiteSpace(_s)) res = res.Where(l => l.naziv.ToLower().Contains(_s) || l.koda.ToLower().StartsWith(_s)).ToList();
        //    Response.Write(JsonConvert.SerializeObject(res.Select(l => l.naziv).OrderBy(l => l)));
        //}
        //else if (Request.QueryString["m"] == "racuni")
        //{
        //    string skupina = Request.QueryString["skupina"];
        //    List<Artikli.Artikel> res = Artikli.Seznam.Where(l => l.podrocje == "racuni").ToList();
        //    if (!string.IsNullOrWhiteSpace(_s)) res = res.Where(l => l.naziv.ToLower().Contains(_s) || l.koda.ToLower().StartsWith(_s)).ToList();
        //    if (!string.IsNullOrWhiteSpace(skupina)) res = res.Where(l => Math.Abs(l.skupina.GetHashCode()).ToString() == skupina).ToList();
        //    else res = res.Where(l => l.skupina != "zadnje" && l.skupina != "pogosto").ToList();
        //    Response.Write(JsonConvert.SerializeObject(res.Select(l => l.naziv).OrderBy(l => l)));
        //}
    }
    private void artikel()
    {
        //if (Request.QueryString["m"] == "prihodkiodhodki")
        //{
        //    Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Artikli.Where(l => l.naziv.ToLower() == _s)));
        //}
        //else if (Request.QueryString["m"] == "blagajne")
        //{
        //    Response.Write(JsonConvert.SerializeObject(Artikli.Seznam.Where(l => l.naziv.ToLower() == _s)));
        //}
        //else if (Request.QueryString["m"] == "racuni")
        //{
        //    Response.Write(JsonConvert.SerializeObject(Artikli.Seznam.Where(l => l.naziv.ToLower() == _s)));
        //}
    }
    private void enote()
    {
       // Response.Write(JsonConvert.SerializeObject(Artikli.Seznam.Select(l => l.enota).Distinct().OrderBy(l => l)));
    }
    private void glave()
    {
        //if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Glave.Select(l => l.oznaka).OrderBy(l => l)));
        //else Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Glave.Where(l => l.oznaka.ToLower().Contains(_s)).Select(l => l.oznaka).OrderBy(l => l).Take(20)));
    }
    private void glava()
    {
        //Response.Write(JsonConvert.SerializeObject(PrihodkiOdhodki.Glave.Where(l => l.oznaka.ToLower() == _s)));
    }
    private void serije()
    {
        //if (string.IsNullOrWhiteSpace(_s)) Response.Write(JsonConvert.SerializeObject(Racuni.SerijeSeti.Select(l => l.Key).OrderBy(l => l)));
        //else Response.Write(JsonConvert.SerializeObject(Racuni.SerijeSeti.Where(l => l.Key.Contains(_s)).Select(l => l.Key).OrderBy(l => l).Take(20)));
    }
    private void set()
    {
        //string serija = Request.QueryString["serija"];
        //var item = Racuni.SerijeSeti.FirstOrDefault(l => l.Key == serija);
        //if (item.Key == serija && item.Value.Count > 0)
        //{
        //    int set = Convert.ToInt32(item.Value.Max()) + 1;
        //    if (set > 0 && set <= 50) Response.Write(JsonConvert.SerializeObject(set.ToString().PadLeft(2, '0')));
        //}
    }
    private void stevilka()
    {
        //Response.Write(JsonConvert.SerializeObject(Racuni.NaslednjaVezanaKnjiga(Request.QueryString["set"], Request.QueryString["serija"])));
    }
    private void pravice()
    {
        List<string> obstojece = new List<string>(Request.QueryString["s"].Trim(',').Split(','));
        List<string> pravice = new List<string>();
        foreach (DataRow r in Uporabniki.Get().Rows)
        {
            foreach (string pravica in Convert.ToString(r["pravice"]).Split(',')) if (!obstojece.Contains(pravica) && !pravice.Contains(pravica)) pravice.Add(pravica);
        }
        pravice.Sort();
        Response.Write(JsonConvert.SerializeObject(pravice));
    }
    private void skupine()
    {
        //List<string> obstojece = new List<string>(Request.QueryString["s"].Trim(',').Split(','));
        //List<string> skupine = new List<string>();
        //foreach (DataRow r in Artikli.Get().Rows)
        //{
        //    string skupina = Convert.ToString(r["skupina"]);
        //    if (!obstojece.Contains(skupina) && !skupine.Contains(skupina)) skupine.Add(skupina);
        //}
        //skupine.Sort();
        //Response.Write(JsonConvert.SerializeObject(skupine));
    }
    private void revirji()
    {
        //List<string> obstojece = new List<string>(Request.QueryString["s"].Trim(',').Split(','));
        //List<string> revirji = new List<string>();
        //foreach (DataRow r in Revirji.Get().Rows)
        //{
        //    string revir = Convert.ToString(r["id"]);
        //    if (!obstojece.Contains(revir) && !revirji.Contains(revir)) revirji.Add(revir);
        //}
        //revirji.Sort();
        //Response.Write(JsonConvert.SerializeObject(revirji));
    }
    private void blagajne()
    {
        //List<string> obstojece = new List<string>(Request.QueryString["s"].Trim(',').Split(','));
        //List<string> blagajne = new List<string>();
        //foreach (DataRow r in PoslovniProstori.Get().Rows)
        //{
        //    string oznaka = Convert.ToString(r["oznaka"]);
        //    foreach (string elektronska_naprava in Convert.ToString(r["elektronske_naprave"]).Split(','))
        //    {
        //        string blagajna = oznaka + "-" + elektronska_naprava;
        //        if (!obstojece.Contains(blagajna) && !blagajne.Contains(blagajna)) blagajne.Add(blagajna);
        //    }
        //}
        //blagajne.Sort();
        //Response.Write(JsonConvert.SerializeObject(blagajne));
    }
    private void artikli_skupine()
    {
        //Response.Write(JsonConvert.SerializeObject(Artikli.Skupine));
    }
    private void geslo()
    {
        Odgovor res = new Odgovor("Uporabniško ime ne obstaja", "Preverite ali ste vnesli pravilno uporabniško ime.", "e");
        foreach (DataRow r in Uporabniki.Get().Rows)
        {
            string username = Convert.ToString(r["username"]);
            if (username.ToLower() == _s)
            {
                string email = Convert.ToString(r["email"]);
                string storeid = Convert.ToString(r["StoreID"]);

                if (!string.IsNullOrWhiteSpace(email))
                {
                    string hash = HttpServerUtility.UrlTokenEncode(Convert.FromBase64String(Varnost.EncryptAES256(username + "@-|@|-@" + Convert.ToString(r["password"]) + "@-|@|-@" + storeid + "@-|@|-@" + ConfigurationManager.AppSettings["EncryptKey"])));
                    string msg;
                    if (Email.Send(email, "Pozabljeno geslo", "Pozdravljeni,<br />nekdo je na spletni strani http://www.mr-avtomatika.com/" + ConfigurationManager.AppSettings["Skupina"] + "  zahteval zamenjavo pozabljenega gesla za uporabniško ime <b>" + username + "</b>. Če tega niste zahtevali vi, nam prosim to sporočite na naslov <a href='mailto:info@mr-avtomatika.com'>info@mr-avtomatika.com</a>. Vaši podatki so še vedno varni.<br />Spodaj vam pošiljamo povezavo, kjer boste lahko zamenjali svoje pozabljeno geslo.<br /><br />Kliknite <a href=\"http://www.mr-avtomatika.com/" + ConfigurationManager.AppSettings["Skupina"] + "/Geslo.aspx?hash=" + hash + "\"><b>TUKAJ</b></a> za zamenjavo pozabljenega gesla.<br /><br />LP " + Nastavitve.Naziv, true, "Pozabljeno geslo", new Dictionary<string, System.IO.Stream>(), out msg)) res = new Odgovor("Uspešno poslano", "Preverite svoj elektronski predal in sledite navodilom v sporočilu. Če elektronske pošte niste dobili, najprej preverite med sporočili, ki so označena kot vsiljena pošta. Če sporočila res niste prejeli, se obrnite po pomoč na elektronski naslov <a href='mailto:info@mr-avtomatika.com'>info@mr-avtomatika.com</a>.", "i");
                    else res = new Odgovor("Pri pošiljanju je prišlo do napake", msg, "x");
                    break;
                }
            }
        }
        Response.Write(JsonConvert.SerializeObject(res));
    }
    private void sporocila()
    {
        //Response.Write(JsonConvert.SerializeObject(Sporocila.Zadnje()));
    }
    private void persistence()
    {
        string key = Request.QueryString["key"] ?? "persistence";
        Session.Remove(key);
        Session.Add(key, Request.QueryString["s"]);
    }
    private void transakcije()
    {
        //string res = "";
        //double znesek = 0;
        //DataTable dt_clan = Clani.Get(Convert.ToInt32(_s));
        //DataTable dt_transakcije = Clani.GetTransakcije((dt_clan.Rows[0]["ime"] + " " + dt_clan.Rows[0]["priimek"]).ToLower(), _u);
        //if (dt_transakcije.Rows.Count > 0)
        //{
        //    res += "<tr>";
        //    res += "<th>Opis</th>";
        //    res += "<th style=\"width:180px;text-align:center;\">Datum</th>";
        //    res += "<th style=\"width:120px;text-align:right;\">Znesek</th>";
        //    res += "</tr>";
        //    foreach (DataRow r in dt_transakcije.Rows)
        //    {
        //        res += "<tr>";
        //        res += "<td>" + Convert.ToString(r["opis"]) + "</td>";
        //        res += "<td style=\"text-align:center;\">" + Convert.ToDateTime(r["datum"]).ToString("dd. MMMM yyyy") + "</td>";
        //        res += "<td style=\"text-align:right;\" class=\"znesek\">" + Convert.ToDouble(r["znesek"]).ToString("#,##0.00") + "</td>";
        //        res += "</tr>";
        //        znesek += Convert.ToDouble(r["znesek"]);
        //    }
        //    res += "<tr>";
        //    res += "<th></th>";
        //    res += "<th style=\"width:80px;text-align:center;\"></th>";
        //    res += "<th style=\"width:80px;text-align:right;\">" + znesek.ToString("#,##0.00") + "</th>";
        //    res += "</tr>";
        //}
        //Response.Write(res);
    }
    private void tip()
    {
        Odgovor res = new Odgovor("title", "message", "i");
        Response.Write(JsonConvert.SerializeObject(res));
    }
}