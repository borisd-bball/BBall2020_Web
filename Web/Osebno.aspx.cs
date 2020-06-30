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

public partial class _Osebno : System.Web.UI.Page
{
    protected int _id;
    protected string msg = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Osebno";
            Master.Title = "Osebno";
            msg = Request.QueryString["msg"] ?? "";
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
                email.Text = Master.Uporabnik.Email;
                ime.Text = Master.Uporabnik.Ime;
                priimek.Text = Master.Uporabnik.Priimek;
                password.Text = "";
                password_new1.Text = "";
                password_new2.Text = "";
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
            password.Text = password.Text.Trim();
            password_new1.Text = password_new1.Text.Trim();
            password_new2.Text = password_new2.Text.Trim();
            email.Text = email.Text.Trim();
            ime.Text = ime.Text.Trim();
            priimek.Text = priimek.Text.Trim();
            if (string.IsNullOrWhiteSpace(ime.Text)) throw new Exception("Polje Ime ne sme biti prazno");
            if (string.IsNullOrWhiteSpace(priimek.Text)) throw new Exception("Polje Priimek ne sme biti prazno");
            if (string.IsNullOrWhiteSpace(email.Text)) throw new Exception("Polje Email ne sme biti prazno");
            if (!string.IsNullOrWhiteSpace(password.Text) && password.Text.Length < 4) throw new Exception("Polje Geslo ne sme biti krajše od 4 znakov");
            if (!string.IsNullOrWhiteSpace(password_new1.Text) && password_new1.Text.Length < 4) throw new Exception("Polje Novo geslo ne sme biti krajše od 4 znakov");
            if (!string.IsNullOrWhiteSpace(password_new2.Text) && password_new2.Text.Length < 4) throw new Exception("Polje Novo geslo (ponovi) ne sme biti krajše od 4 znakov");
            if (password_new1.Text != password_new2.Text) throw new Exception("Polji Novo geslo in Novo geslo (ponovi) nista enaki");

            Uporabniki.Edit(Master.Uporabnik.Id, Master.Uporabnik.Username, password_new1.Text, ime.Text, priimek.Text, true, true, string.Join(",", Master.Uporabnik.Pravice), Session, email.Text, "", Master.Uporabnik.Id);
            Response.Redirect("Osebno.aspx?msg=" + HttpUtility.UrlEncode("Podatki shranjeni"));
        }
        catch (Exception er)
        {
            Master.SetMessage(er.Message);
        }
    }
}