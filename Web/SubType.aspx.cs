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

public partial class _SubType : System.Web.UI.Page
{
    protected string _id;
    protected string msg = "";
    protected string _sort;
    protected string _sel_leto = "";
    protected string _persistence;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Tip Zavezanca";
            if (!Master.Uporabnik.Pravice.Contains("tipi_zavezancev")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_subtype"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "datum desc";
            }
            else Session["sort_subtype"] = _sort;
            _persistence = Convert.ToString(Session["persistence_subtype"]) ?? "";
            //int.TryParse(Request.QueryString["id"], out _id);
            _id = Request.QueryString["id"];

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

                var lc = new List<SubTypes.SubType>();
                if (!String.IsNullOrEmpty(_id)) { lc = SubTypes.Get(0,_id); }
                
                if (lc.Count > 0)
                {
                    var lData = lc[0]; 
                    naziv.Text = lData.acTitle;
                    TypeID.Text = (lData.acTypeID);
                    drzava.SelectedValue = Convert.ToString(lData.anCountryID);

                    brisi.Visible = true;
                    TypeID.ReadOnly = true;


                }
                else
                {
                    TypeID.ReadOnly = false;
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
            TypeID.Text = TypeID.Text.Trim();

            if (string.IsNullOrWhiteSpace(naziv.Text)) throw new Exception("Polje Naziv ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(TypeID.Text)) throw new Exception("Polje ID ne sme biti prazno!");
            if (string.IsNullOrWhiteSpace(drzava.Text)) throw new Exception("Polje Država ne sme biti prazno!");

            if (String.IsNullOrEmpty(_id))
            {
                var lData = new SubTypes.SubType(0,
                    TypeID.Text,
                    Convert.ToInt32(drzava.SelectedValue),
                    naziv.Text,
                    DateTime.Now,
                    Master.Uporabnik.Id,
                    0);

                SubTypes.Insert(lData);
                _id = lData.acTypeID;
            }
            else
            {
                var lData = SubTypes.Get(0,_id)[0];
                lData.acTitle = naziv.Text;
                lData.anCountryID = Convert.ToInt32(drzava.SelectedValue);
                lData.adModificationDate = DateTime.Now;
                lData.anUserMod = Master.Uporabnik.Id;

                SubTypes.Update(lData);
            }

            Response.Redirect("SubType.aspx?id=" + _id + "&msg=" + HttpUtility.UrlEncode("Podatki shranjeni"));
        }
        catch (Exception er)
        {
            Master.SetMessage(er.Message);
        }
    }
    protected void brisi_Click(object sender, EventArgs e)
    {
        var lData = SubTypes.Get(0, _id)[0];
        SubTypes.Delete(lData);
        Response.Redirect("SubTypes.aspx?msg=" + HttpUtility.UrlEncode("Izbrisano"));
    }



}