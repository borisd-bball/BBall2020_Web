using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;
using System.Threading;
using mr.bBall_Lib;

public partial class _Ostalo : System.Web.UI.Page
{
    protected double _prihodkiodhodki_znesek = 0;
    protected int _uporabniki_stetje = 0;
    protected int _countrys_stetje = 0;
    protected int _posts_stetje = 0;
    protected int _stranke_stetje = 0;
    protected int _artikli_stetje = 0;
    protected int _subtypes_stetje = 0;
    protected int _sporocila_stetje = 0;
    protected int _posi_stetje = 0;
    protected int poslovni_prostori_stetje = 0;
    protected double _osnovnasredstva_znesek = 0;
    protected string msg = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Ostalo";
            if (!Master.Uporabnik.Pravice.Contains("ostalo")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            //_prihodkiodhodki_znesek = Convert.ToDouble(PrihodkiOdhodki.GetSkupaj(Master.Date).Rows[0]["znesek"]);
            //_osnovnasredstva_znesek = OsnovnaSredstva.GetSkupaj(Master.Date);
            _uporabniki_stetje = Uporabniki.Get().Rows.Count;
            _countrys_stetje = Countrys.Get_d().Rows.Count;
            _posts_stetje = Posts.Get_d().Rows.Count;
            _stranke_stetje = Stranke.Get_d().Rows.Count;
            //_artikli_stetje = Artikli.Get("").Rows.Count;
            _subtypes_stetje = SubTypes.Get_d().Rows.Count;
            //_sporocila_stetje = Sporocila.Get().Rows.Count;
            //_posi_stetje = Posi.Get().Rows.Count;
            //poslovni_prostori_stetje = PoslovniProstori.Get().Rows.Count;
            if (!string.IsNullOrWhiteSpace(msg)) Master.SetMessage(msg);
        }
        catch (Exception ee)
        {
            Master.SetMessage(ee);
        }
    }
}