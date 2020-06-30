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
using Newtonsoft.Json;

public partial class _Uporabnik : System.Web.UI.Page
{
    protected int _id;
    protected string msg = "";
    protected string s_vrstice = "";
    private List<string> vrstice = new List<string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Uporabnik";
            if (!Master.Uporabnik.Pravice.Contains("uporabniki")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            int.TryParse(Request.QueryString["id"], out _id);
            if (IsPostBack)
            {
                #region
                msg = "";
                Master.SetMessage(msg);
                string vrstica = Request.Form["vrstica"];
                if (!string.IsNullOrWhiteSpace(vrstica))
                {
                    string[] vs = vrstica.Split('-');
                    if (vs.Length == 2)
                    {
                        int idx = Convert.ToInt32(vs[1]);
                        if (ViewState["vrstice"] != null) vrstice = JsonConvert.DeserializeObject<List<string>>(ViewState["vrstice"].ToString());
                        if (vs[0] == "u") u_roles.Text = vrstice[idx];
                        vrstice.RemoveAt(idx);
                        ViewState["vrstice"] = JsonConvert.SerializeObject(vrstice);
                    }
                }
                #endregion
            }
            else
            {
                #region
                if (u_roles.Items.Count == 0)
                {
                    foreach (DataRow r in Uporabniki.Get_URoles().Select("anActive = 1")) u_roles.Items.Add(new ListItem(r["acRoleName"].ToString() , Convert.ToString(r["acRoleID"])));
                }

                DataTable dt = Uporabniki.Get(_id);
                if (dt.Rows.Count > 0)
                {
                    username.Text = Convert.ToString(dt.Rows[0]["acUserName"]);
                    email.Text = Convert.ToString(dt.Rows[0]["acEmail"]);
                    ime.Text = Convert.ToString(dt.Rows[0]["acFirstName"]);
                    priimek.Text = Convert.ToString(dt.Rows[0]["acLastName"]);
                    anAdmin.Checked = Convert.ToInt32(dt.Rows[0]["anAdmin"]) == 1 ? true : false; 
                    brisi.Visible = true;

                    vrstice = new List<string>();
                    foreach (DataRow r in Uporabniki.Get_UserRoles(Convert.ToString(dt.Rows[0]["acUserName"])).Rows) { vrstice.Add(r["acRoleID"].ToString()); }
                    ViewState["vrstice"] = JsonConvert.SerializeObject(vrstice);
                }
                else
                {
                    password.CssClass = "req";
                    //elektronske_naprave(poslovni_prostor.SelectedValue);
                }
                password.Text = "";
                #endregion
            }
            vrstice_izpis();
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
            username.Text = username.Text.Trim();
            password.Text = password.Text.Trim();
            email.Text = email.Text.Trim();
            ime.Text = ime.Text.Trim();
            priimek.Text = priimek.Text.Trim();
            if (string.IsNullOrWhiteSpace(username.Text)) throw new Exception("Polje Uporabniško ime ne sme biti prazno");
            if ((!string.IsNullOrWhiteSpace(password.Text) || _id == 0) && password.Text.Length < 4) throw new Exception("Polje Geslo ne sme biti krajše od 4 znakov");
            if (ViewState["vrstice"] != null) vrstice = JsonConvert.DeserializeObject<List<string>>(ViewState["vrstice"].ToString());


            //List<int> l_davcne = new List<int>();
            //int i_davcna = Convert.ToInt32(davcna.Text);
            //l_davcne.Add(i_davcna);
            //foreach (var item in davcne.Text.Split(','))
            //{
            //    i_davcna = Convert.ToInt32(item);
            //    if (!l_davcne.Contains(i_davcna)) l_davcne.Add(i_davcna);
            //}
            //if (_id == 0) _id = Uporabniki.Dodaj(username.Text, password.Text, ime.Text, priimek.Text, email.Text, davcna.Text, string.Join(",", vrstice), prodajalec.Checked, poslovni_prostor.SelectedValue, elektronska_naprava.SelectedValue, skupine.Text, oznaka.Text, racuni.Checked, popravljanje.Checked, revirji.Text, reprezentanca.Checked, string.Join(",", l_davcne), tiskalnik.Text);
            //else Uporabniki.Popravi(_id, username.Text, password.Text, ime.Text, priimek.Text, email.Text, davcna.Text, string.Join(",", vrstice), prodajalec.Checked, s_poslovni_prostor, s_elektronska_naprava, skupine.Text, oznaka.Text, racuni.Checked, popravljanje.Checked, revirji.Text, reprezentanca.Checked, string.Join(",", l_davcne), tiskalnik.Text, null);

            if (_id == 0) _id = Uporabniki.Add(username.Text, password.Text, ime.Text, priimek.Text, true, string.Join(",", vrstice), email.Text, "", anAdmin.Checked, Master.Uporabnik.Id);
            else Uporabniki.Edit(_id, username.Text, password.Text, ime.Text, priimek.Text, true, anAdmin.Checked, string.Join(",", vrstice), null, email.Text, "", Master.Uporabnik.Id);
            Response.Redirect("Uporabnik.aspx?id=" + _id + "&msg=" + HttpUtility.UrlEncode("Podatki shranjeni"));
        }
        catch (Exception er)
        {
            Master.SetMessage(er.Message);
        }
    }
    protected void brisi_Click(object sender, EventArgs e)
    {
        Uporabniki.Delete(_id);
        Response.Redirect("Uporabniki.aspx?msg=" + HttpUtility.UrlEncode("Izbrisano"));
    }
    protected void dodaj_Click(object sender, EventArgs e)
    {
        try
        {
            if (ViewState["vrstice"] != null) vrstice = JsonConvert.DeserializeObject<List<string>>(ViewState["vrstice"].ToString());
            string uRole = u_roles.SelectedValue;
            if (string.IsNullOrWhiteSpace(uRole)) throw new Exception("Izbrana ni nobena pravica.");
            if (vrstice.Contains(uRole)) throw new Exception("Izbrana pravica je že dodeljena.");
            vrstice.Add(uRole);
            ViewState["vrstice"] = JsonConvert.SerializeObject(vrstice);
            vrstice_izpis();
        }
        catch (Exception ee)
        {
            Master.SetMessage(ee);
        }
    }
    protected void vrstice_izpis()
    {
        s_vrstice = "";
        if (ViewState["vrstice"] != null) vrstice = JsonConvert.DeserializeObject<List<string>>(ViewState["vrstice"].ToString());
        int c = 0;
        foreach (var item in vrstice)
        {
            s_vrstice += "<tr>";
            s_vrstice += "<td style=\"text-weight:bold;" + (c == 0 ? "border-top:solid 1px #DDD;" : "") + "\" class=\"pravica\">&nbsp;" + item + "</td>";
            s_vrstice += "<td style=\"text-align:center;vertical-align:middle;\"><i class=\"icon-edit pt\" onclick=\"$('#vrstica').val('u-" + c + "');__doPostBack('vrstica_uredi', '');\"></i>&nbsp;<i class=\"icon-remove pt\" onclick=\"$('#vrstica').val('i-" + c + "');__doPostBack('vrstica_izbriši', '');\"></i></td>";
            s_vrstice += "</tr>";
            c++;
        }
    }
    protected void poslovni_prostor_SelectedIndexChanged(object sender, EventArgs e)
    {
        elektronske_naprave(((DropDownList)sender).SelectedValue);
    }
    private void elektronske_naprave(string oznaka)
    {
        //elektronska_naprava.Items.Clear();
        //DataRow[] r = PoslovniProstori.Get().Select("oznaka='" + oznaka + "'");
        //if (r.Length > 0)
        //{
        //    foreach (var item in Convert.ToString(r[0]["elektronske_naprave"]).Split(',')) elektronska_naprava.Items.Add(item);
        //}
    }
}