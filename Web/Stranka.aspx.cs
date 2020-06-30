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

public partial class _Stranka : System.Web.UI.Page
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
            Master.Title = "Stranka";
            if (!Master.Uporabnik.Pravice.Contains("stranke")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_stranka"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "datum desc";
            }
            else Session["sort_stranka"] = _sort;
            _persistence = Convert.ToString(Session["persistence_stranka"]) ?? "";
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
                foreach (var c in Countrys.Get(0)) drzava.Items.Add(new ListItem(c.acTitle, Convert.ToString(c.anCountryID)));

                var ls = new List<Stranke.Customer>();
                if (_id > 0) { ls = Stranke.Get(_id); }
                
                if (ls.Count > 0)
                {
                    var lStranka = ls[0]; 
                    naziv.Text = lStranka.acTitle;
                    k_naziv.Text = lStranka.acShortTitle;
                    email.Text = lStranka.aceMaile;
                    naslov.Text = lStranka.acAddress;
                    drzava.SelectedValue = Convert.ToString(lStranka.anCountryID);
                    drzava_SelectedIndexChanged(drzava, null);

                    posta.SelectedValue = Convert.ToString(lStranka.anPostID);
                    customerCode.Text = Convert.ToString(lStranka.anCustomerID);
                    tip_zavezanca.SelectedValue = lStranka.acVATTypeID;
                    id_za_ddv.Text = lStranka.acVATPrefix; 
                    davcna.Text = lStranka.acVATNumber;
                    contact.Text = lStranka.acContactName; 
                    telefon.Text = lStranka.acTelephone;
                    note.Text = lStranka.acNote;
                    active.Checked = Convert.ToInt32(lStranka.anActive) == 1 ? true : false;
                    brisi.Visible = true;

                    //string stranka_naziv = naziv.Text;
                    //DataTable dt_transakcije = Stranke.GetTransakcije(stranka_naziv.ToLower());
                    if (Request.QueryString["a"] == "csv")
                    {
                        //Response.Clear();
                        //byte[] csv = Encoding.Default.GetBytes(Splosno.Csv(dt_transakcije, "stranka"));
                        //Response.ContentType = "application/csv; name=Stranka.csv";
                        //Response.AddHeader("content-transfer-encoding", "binary");
                        //Response.AddHeader("Content-Disposition", "attachment; filename=Stranka.csv");
                        //Response.OutputStream.Write(csv, 0, csv.Length);
                        //Response.Flush();
                        //Response.End();
                    }
                    else
                    {
                        for (int i = 2014; i <= DateTime.Today.Year; i++) _sel_leto += "<option value=\".le" + i + "\">" + i + "</option>";
                       // r_transakcije.DataSource = dt_transakcije;
                       // r_transakcije.DataBind();
                    }
                }
                else
                {
                    // For new customer
                    drzava.SelectedValue = "206";
                    drzava_SelectedIndexChanged(drzava, null);
                    active.Checked = true;
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
            email.Text = email.Text.Trim();
            naslov.Text = naslov.Text.Trim();
            davcna.Text = davcna.Text.Trim();

            if (string.IsNullOrWhiteSpace(naziv.Text)) throw new Exception("Polje Naziv ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(k_naziv.Text)) throw new Exception("Polje Kratek naziv ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(davcna.Text)) throw new Exception("Polje ID za DDV ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(drzava.Text)) throw new Exception("Polje Država ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(posta.Text)) throw new Exception("Polje Pošta ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(tip_zavezanca.Text)) throw new Exception("Polje Tip zavezanca ne sme biti prazno!");

            if (_id == 0)
            {
                var lStranka = new Stranke.Customer(0, 
                    Stranke.Get_MaxID() + 1,
                    Convert.ToInt32(posta.SelectedValue),
                    Convert.ToInt32(drzava.SelectedValue),
                    naziv.Text,
                    k_naziv.Text,
                    naslov.Text,
                    posta.SelectedItem.Text,
                    id_za_ddv.Text,
                    davcna.Text,
                    tip_zavezanca.SelectedValue,
                    telefon.Text,
                    note.Text,
                    email.Text,
                    contact.Text,
                    (active.Checked) ? 1 : 0,
                    DateTime.Now,
                    Master.Uporabnik.Id,
                    0);

                Stranke.Insert(lStranka);
                _id = lStranka.anCustomerID;
            }
            else
            {
                var lStranka = Stranke.Get(_id)[0];
                lStranka.acAddress = naslov.Text;
                lStranka.acContactName = contact.Text;
                lStranka.aceMaile = email.Text;
                lStranka.acNote = note.Text;
                lStranka.acPostTitle = posta.SelectedItem.Text;
                lStranka.acShortTitle = k_naziv.Text;
                lStranka.acTelephone = telefon.Text;
                lStranka.acTitle = naziv.Text;
                lStranka.acVATNumber = davcna.Text;
                lStranka.acVATPrefix = id_za_ddv.Text;
                lStranka.acVATTypeID = tip_zavezanca.SelectedValue;
                lStranka.adModificationDate = DateTime.Now;
                lStranka.anActive = (active.Checked) ? 1 : 0;
                lStranka.anCountryID = Convert.ToInt32(drzava.SelectedValue);
                lStranka.anPostID = Convert.ToInt32(posta.SelectedValue);
                lStranka.anUserMod = Master.Uporabnik.Id;

                Stranke.Update(lStranka);
            }

            Response.Redirect("Stranka.aspx?id=" + _id + "&msg=" + HttpUtility.UrlEncode("Podatki shranjeni"));
        }
        catch (Exception er)
        {
            Master.SetMessage(er.Message);
        }
    }
    protected void brisi_Click(object sender, EventArgs e)
    {
        var lStranka = Stranke.Get(_id)[0];
        Stranke.Delete(lStranka);
        Response.Redirect("Stranke.aspx?msg=" + HttpUtility.UrlEncode("Izbrisano"));
    }
    protected string tip_transakcije(string tip_transakcije)
    {
        if (tip_transakcije == "Racun") return "Račun";
        else if (tip_transakcije == "Blagajna") return "Blagajna";
        else return "";
    }

    protected void drzava_SelectedIndexChanged(object sender, EventArgs e)
    {
        sifrant_post(((DropDownList)sender).SelectedValue);
    }

    protected void tip_zavezanca_SelectedIndexChanged(object sender, EventArgs e)
    {
        sifrant_tip_zavezanca(((DropDownList)sender).SelectedValue);
    }

    private void sifrant_post(string anCountryID)
    {
        posta.Items.Clear();
        tip_zavezanca.Items.Clear();

        foreach (var c in Posts.Get(Convert.ToInt32(anCountryID),0)) posta.Items.Add(new ListItem(c.acTitle + " (" + c.acISOCode + "-" + c.acCode + ")"  , Convert.ToString(c.anPostID)));
        foreach (var t in SubTypes.Get(Convert.ToInt32(anCountryID), "")) tip_zavezanca.Items.Add(new ListItem(t.acTitle + " (" + t.acTypeID + ")", Convert.ToString(t.acTypeID)));

        sifrant_tip_zavezanca(tip_zavezanca.SelectedValue);

        if (Convert.ToInt32(anCountryID) == 206) { iskanje.ReadOnly = false; } else { iskanje.ReadOnly = true; }

    }

    private void sifrant_tip_zavezanca(string acTypeID)
    {

        if ((acTypeID == "K") || (String.IsNullOrEmpty(acTypeID)))
        {
            id_za_ddv.Text = "";
        }
        else
        {
            var lc = Countrys.Get(Convert.ToInt32(drzava.SelectedValue))[0];
            id_za_ddv.Text = lc.acISOCode;
        }

    }


}