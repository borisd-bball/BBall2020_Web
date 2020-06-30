using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mr.bBall_Lib;
using System.Configuration;

public partial class _Geslo : System.Web.UI.Page
{
    protected string _js = "";
    protected string _error = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            using (Uporabnik up = new Uporabnik(Session))
            {
                up.logout(Session);
            }
            string u = "";
            string p = "";
            try
            {
                string[] hash = Varnost.DecryptAES256(Convert.ToBase64String(HttpServerUtility.UrlTokenDecode(Request.QueryString["hash"]))).Split(new string[] { "@-|@|-@" }, StringSplitOptions.None);
                if (hash.Length == 3 && hash[2] == ConfigurationManager.AppSettings["EncryptKey"])
                {
                    u = hash[0].Trim();
                    p = hash[1].Trim();
                }
                else throw new Exception();
            }
            catch
            {
                Response.Redirect("Default.aspx", true);
            }

            if (Uporabnik.login(u, p, Session, "", 1) == 0)
            {
                using (Uporabnik uporabnik = new Uporabnik(Session))
                {
                    try
                    {
                        username.Text = uporabnik.Username;
                        if (IsPostBack)
                        {
                            password_new1.Text = password_new1.Text.Trim();
                            password_new2.Text = password_new2.Text.Trim();
                            if (!string.IsNullOrWhiteSpace(password_new1.Text) && password_new1.Text.Length < 6) throw new Exception("Polje Novo geslo ne sme biti krajše od 6 znakov");
                            if (!string.IsNullOrWhiteSpace(password_new2.Text) && password_new2.Text.Length < 6) throw new Exception("Polje Novo geslo (ponovi) ne sme biti krajše od 6 znakov");
                            if (password_new1.Text != password_new2.Text) throw new Exception("Polji Novo geslo in Novo geslo (ponovi) nista enaki");

                            Uporabniki.Edit(uporabnik.Id, uporabnik.Username, password_new1.Text, uporabnik.Ime, uporabnik.Priimek, uporabnik.Active, uporabnik.Admin, string.Join(",", uporabnik.Pravice), Session, uporabnik.Email, uporabnik.Gsm, uporabnik.Id);

                            _js = "dialog('Geslo uspešno spremenjeno', 'Sedaj se lahko prijavite z novim geslom.', 'i');";
                            _js += "$('#dialog').dialog('option', 'buttons', { Ok: function () { $(this).dialog('close'); window.location='Default.aspx';}});";
                        }
                    }
                    catch (Exception er)
                    {
                        throw er;
                    }
                    finally
                    {
                        uporabnik.logout(Session);
                    }
                }
            }
            else _error = "Podatki v povezavi so nepravilni";
        }
        catch (Exception er)
        {
            _error = "Prišlo je do programske napake<br /><i style=\"font-size:0.8em;\">" + er.Message + "</i>";
        }
    }
}