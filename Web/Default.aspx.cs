using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using mr.bBall_Lib;

public partial class _Default : System.Web.UI.Page
{
    protected string _error = "";
    protected int _ForceLogin = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            HttpCookie c = Request.Cookies["hash"];
            using (Uporabnik user = new Uporabnik(Session))
            {
                string action = Request.QueryString["action"] ?? "";
                #region logout
                if (action == "logout")
                {
                    if (c != null) Response.Cookies["hash"].Expires = DateTime.Now.AddDays(-1);
                    user.logout(user.Id,null, Session, "");
                    Response.Redirect("Default.aspx", true);
                }
                #endregion
                if (!user.LoggedIn && (Request.HttpMethod == "POST" || c != null))
                {
                    string u = username.Text.Trim();
                    string p = password.Text.Trim();
                    bool r = remember.Checked;
                    _ForceLogin = 1;

                    if (Request.HttpMethod != "POST")
                    {
                        #region cookie
                        string c_username = "";
                        string c_password = "";
                        if (c != null && !string.IsNullOrEmpty(c.Value))
                        {
                            try
                            {
                                string[] c_hash = Varnost.DecryptAES256(c.Value).Split(new string[] { "@-|@|-@" }, StringSplitOptions.None);
                                if (c_hash.Length == 3 && c_hash[2] == ConfigurationManager.AppSettings["EncryptKey"])
                                {
                                    c_username = c_hash[0].Trim();
                                    c_password = c_hash[1].Trim();
                                }
                                else throw new Exception();
                            }
                            catch
                            {
                                Response.Cookies["hash"].Expires = DateTime.Now.AddDays(-1);
                                Response.Redirect("Default.aspx", true);
                            }
                        }
                        if (string.IsNullOrEmpty(u)) u = c_username;
                        if (string.IsNullOrEmpty(p)) p = c_password;
                        #endregion
                    }
                    if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) _error = "Uporabniško ime in geslo ne smejo biti prazni";
                    else
                    {
                        int lLoginResp = Uporabnik.login(u, p, Session, "", 1);

                        if (lLoginResp == 0)
                        {
                            if (r)
                            {
                                Response.Cookies["hash"].Value = Varnost.EncryptAES256(u + "@-|@|-@" + p + "@-|@|-@" + ConfigurationManager.AppSettings["EncryptKey"]);
                                Response.Cookies["hash"].Expires = DateTime.Now.AddDays(30);
                            }
                            else if (Request.HttpMethod == "POST" && c != null) Response.Cookies["hash"].Expires = DateTime.Now.AddDays(-1);
                            using (Uporabnik uporabnik = new Uporabnik(Session))
                            {
                                if (uporabnik.Pravice.Contains("pregled")) Response.Redirect("Pregled.aspx", true);
                                else if (uporabnik.Pravice.Contains("ostalo")) Response.Redirect("Ostalo.aspx", true);
                                else Response.Redirect("Osebno.aspx", true);
                            }
                        }
                        else _error = Splosno.GetTranslateByID(lLoginResp);
                    }
                }
                else if (user.LoggedIn) Response.Redirect("Pregled.aspx", true);
            }
        }
        catch (Exception er)
        {
            _error = "Prišlo je do programske napake<br /><i style=\"font-size:0.8em;\">" + er.Message + "</i>";
        }
    }
}