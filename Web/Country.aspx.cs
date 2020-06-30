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
using mr.bBall_Lib;

public partial class _Country : System.Web.UI.Page
{
    protected int _id;
    protected string msg = "";
    protected string _sort;
    protected string _sel_leto = "";
    protected string _persistence;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Država";
            if (!Master.Uporabnik.Pravice.Contains("drzave")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_country"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "datum desc";
            }
            else Session["sort_country"] = _sort;
            _persistence = Convert.ToString(Session["persistence_country"]) ?? "";
            int.TryParse(Request.QueryString["id"], out _id);
            if (IsPostBack)
            {
                #region
                msg = "";
                Master.SetMessage(msg);
                #endregion
            }
            else
            {
                #region
                var lc = new List<Countrys.Country>();
                if (_id > 0) { lc = Countrys.Get(_id); }
                
                if (lc.Count > 0)
                {
                    var lData = lc[0]; 
                    naziv.Text = lData.acTitle;
                    countryCode.Text = Convert.ToString(lData.anCountryID);
                    iso_code.Text = lData.acISOCode;
                    currency.Text = lData.acCurrency;
                    VATCodePrefix.Text = lData.acVATCodePrefix;
                    isEU.Checked = Convert.ToInt32(lData.anIsEU) == 1 ? true : false;

                    //brisi.Visible = true;

                }
                else
                {

                }
                #endregion
            }
            if (!string.IsNullOrWhiteSpace(msg)) Master.SetMessage(msg);
        }
        catch (Exception ee)
        {
            Master.SetMessage(ee);
        }
    }
    protected void shrani_Click(object sender, EventArgs e)
    {
        try
        {
            naziv.Text = naziv.Text.Trim();
            iso_code.Text = iso_code.Text.Trim();
            currency.Text = currency.Text.Trim();
            VATCodePrefix.Text = VATCodePrefix.Text.Trim();

            if (string.IsNullOrWhiteSpace(naziv.Text)) throw new Exception("Polje Naziv ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(iso_code.Text)) throw new Exception("Polje ISO koda ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(currency.Text)) throw new Exception("Polje Valuta ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(VATCodePrefix.Text)) throw new Exception("Polje Davčna predpona ne sme biti prazno!");

            if (_id == 0)
            {
                var lData = new Countrys.Country(0,
                    Countrys.Get_MaxID() + 1,
                    naziv.Text,
                    iso_code.Text,
                    currency.Text,
                    VATCodePrefix.Text,
                    (isEU.Checked) ? 1 : 0,
                    DateTime.Now,
                    Master.Uporabnik.Id,
                    0);

                Countrys.Insert(lData);
                _id = lData.anCountryID;
            }
            else
            {
                var lData = Countrys.Get(_id)[0];
                lData.acTitle = naziv.Text;
                lData.acCurrency = currency.Text;
                lData.acISOCode = iso_code.Text;
                lData.acVATCodePrefix = VATCodePrefix.Text;
                lData.adModificationDate = DateTime.Now;
                lData.anIsEU = (isEU.Checked) ? 1 : 0;
                lData.anUserMod = Master.Uporabnik.Id;

                Countrys.Update(lData);
            }

            Response.Redirect("Country.aspx?id=" + _id + "&msg=" + HttpUtility.UrlEncode("Podatki shranjeni"));
        }
        catch (Exception er)
        {
            Master.SetMessage(er.Message);
        }
    }
    protected void brisi_Click(object sender, EventArgs e)
    {
        var lData = Countrys.Get(_id)[0];
        Countrys.Delete(lData);
        Response.Redirect("Countrys.aspx?msg=" + HttpUtility.UrlEncode("Izbrisano"));
    }


}