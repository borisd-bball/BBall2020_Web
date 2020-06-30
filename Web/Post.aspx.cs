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

public partial class _Post : System.Web.UI.Page
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
            Master.Title = "Pošta";
            if (!Master.Uporabnik.Pravice.Contains("poste")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_post"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "datum desc";
            }
            else Session["sort_post"] = _sort;
            _persistence = Convert.ToString(Session["persistence_post"]) ?? "";
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

                var lc = new List<Posts.Post>();
                if (_id > 0) { lc = Posts.Get(0,_id); }
                
                if (lc.Count > 0)
                {
                    var lData = lc[0]; 
                    naziv.Text = lData.acTitle;
                    postCode.Text = Convert.ToString(lData.anPostID);
                    iso_code.Text = lData.acISOCode;
                    code.Text = lData.acCode;
                    drzava.SelectedValue = Convert.ToString(lData.anCountryID);

                    brisi.Visible = true;

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
            code.Text = code.Text.Trim();

            if (string.IsNullOrWhiteSpace(naziv.Text)) throw new Exception("Polje Naziv ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(iso_code.Text)) throw new Exception("Polje ISO koda ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(code.Text)) throw new Exception("Polje Koda ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(drzava.Text)) throw new Exception("Polje Država ne sme biti prazno!");

            if (_id == 0)
            {
                var lData = new Posts.Post(0,
                    Posts.Get_MaxID() + 1,
                    Convert.ToInt32(drzava.SelectedValue),
                    naziv.Text,
                    iso_code.Text,
                    code.Text,
                    DateTime.Now,
                    Master.Uporabnik.Id,
                    0);

                Posts.Insert(lData);
                _id = lData.anPostID;
            }
            else
            {
                var lData = Posts.Get(0,_id)[0];
                lData.acTitle = naziv.Text;
                lData.acCode = code.Text;
                lData.acISOCode = iso_code.Text;
                lData.anCountryID = Convert.ToInt32(drzava.SelectedValue);
                lData.adModificationDate = DateTime.Now;
                lData.anUserMod = Master.Uporabnik.Id;

                Posts.Update(lData);
            }

            Response.Redirect("Post.aspx?id=" + _id + "&msg=" + HttpUtility.UrlEncode("Podatki shranjeni"));
        }
        catch (Exception er)
        {
            Master.SetMessage(er.Message);
        }
    }
    protected void brisi_Click(object sender, EventArgs e)
    {
        var lData = Posts.Get(0, _id)[0];
        Posts.Delete(lData);
        Response.Redirect("Posts.aspx?msg=" + HttpUtility.UrlEncode("Izbrisano"));
    }

    protected void drzava_SelectedIndexChanged(object sender, EventArgs e)
    {
        set_iso_code(((DropDownList)sender).SelectedValue);
    }

    private void set_iso_code(string anCountryID)
    {
        var lc = Countrys.Get(Convert.ToInt32(anCountryID))[0];
        iso_code.Text = lc.acISOCode;
    }


}