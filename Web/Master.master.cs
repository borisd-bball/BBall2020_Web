using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using mr.bBall_Lib;


public partial class Master : System.Web.UI.MasterPage
{
    private Uporabnik uporabnik;
    protected string _title = "";
    protected string _message = "";
    public string SelectedBox = "";
    protected string _js = "";
    protected DateTime m_dt = DateTime.Today;
    protected string m_davcna = "";
    protected string m_davcne = "";
    protected string m_qs = "";
    private string m_stran;

    public string Davcna
    {
        get { return m_davcna; }
        set
        {
            m_davcna = value;
            Session["davcna"] = m_davcna;
        }
    }
    public DateTime Date
    {
        get { return m_dt; }
        set
        {
            m_dt = value;
            Session["dt"] = m_dt;
        }
    }
    public Uporabnik Uporabnik
    {
        get
        {
            return uporabnik;
        }
    }
    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
        }
    }
    public string Stran
    {
        get
        {
            return m_stran;
        }
    }

    private void AddLinkBox(string text, string href, string css, string ocena, bool selected, bool right)
    {
        linkboxes.Controls.Add(new LiteralControl("<li class=\"linkbox " + css + "\" style=\"" + (right ? "float:right;" : "") + (linkboxes.Controls.Count == 1 ? "margin-right:0px;" : "") + (selected && right ? ("margin-top:15px;") : "") + "\"><span class=\"ocena\">" + (string.IsNullOrWhiteSpace(ocena) ? "&nbsp;" : ocena) + "</span><a href=\"" + href + "\">" + text + "</a></li>"));
    }
    public void SetMessage(string title, string message, string type, string js)
    {
        if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(message)) _js += "dialog(\"" + title.Replace("\n", "<br />").Replace("\r", "") + "\",\"" + Splosno.JsSafe(message) + "\",\"" + type + "\");" + js;
    }
    public void SetMessage(string title, string message, string type)
    {
        SetMessage(title, message, type, "");
    }
    public void SetMessage(string title)
    {
        SetMessage(title, "", "i", "");
    }
    public void SetMessage(string title, string message)
    {
        SetMessage(title, message, "i", "");
    }
    public void SetMessage(Exception exception)
    {
        SetMessage("Prišlo je do programske napake", exception.Message, "x");
    }
/*
    public void EnableDavcna()
    {
        int i_davcna;
        if (int.TryParse(Request.QueryString["davcna"], out i_davcna) && uporabnik.Davcne.Contains(i_davcna.ToString()))
        {
            m_davcna = i_davcna.ToString();
            Session["davcna"] = i_davcna;
        }
        else if (Session["davcna"] != null && uporabnik.Davcne.Contains((string)Session["davcna"])) m_davcna = (string)Session["davcna"];
        else m_davcna = uporabnik.Davcna;

        string[] tab = Request.QueryString.ToString().Split('&');
        foreach (string pair in tab)
        {
            if (string.IsNullOrEmpty(pair) || pair.StartsWith("davcna=")) continue;
            m_qs += pair + '&';
        }
        string[] l_davcne = uporabnik.Davcne.Split(',');
        for (int i = 0; i < l_davcne.Length; i++) m_davcne += "<option" + (l_davcne[i] == m_davcna ? " selected=\"selected\"" : "") + " value=\"" + l_davcne[i] + "\">" + (i + 1) + "</option>";
        ph_davcna.Visible = l_davcne.Length > 1;
    }
*/
    public void EnableDatePicker()
    {
        if (DateTime.TryParse("1.1." + Request.QueryString["dt"], out m_dt)) Session["dt"] = m_dt;
        else if (Session["dt"] != null) m_dt = (DateTime)Session["dt"];
        else m_dt = new DateTime(DateTime.Today.Year, 1, 1);

        string[] tab = Request.QueryString.ToString().Split('&');
        foreach (string pair in tab)
        {
            if (string.IsNullOrEmpty(pair) || pair.StartsWith("dt=")) continue;
            m_qs += pair + '&';
        }
        ph_dt.Visible = true;
    }

    protected override void OnInit(EventArgs e)
    {
        try
        {
            uporabnik = new Uporabnik(Session);
            if (!uporabnik.AppLogedIn) Response.Redirect("Default.aspx");
            string[] parts = Request.FilePath.Split('/');
            m_stran = parts[parts.Length - 1].Split('.')[0];
        }
        catch (Exception ee)
        {
            SetMessage(ee);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        int c = 0;
        if (Uporabnik.Pravice.Contains("pregled")) AddLinkBox("Pregled", "Pregled.aspx", "nazaj", "", SelectedBox == "", c++ > 0);

        AddLinkBox("Osebno", "Osebno.aspx", "nazaj", "<span onclick=\"window.location='Default.aspx?action=logout'\">Odjava</span>", SelectedBox == "Osebno", c++ > 0);
        if (Uporabnik.Pravice.Contains("ostalo")) AddLinkBox("Ostalo", "Ostalo.aspx", "nastavitve", "", SelectedBox == "Ostalo", c++ > 0);
        if (Uporabnik.PraviceL.Contains("naprave")) AddLinkBox("Aplikacije", "Devices.aspx", "finance1", "", SelectedBox == "Aplikacije", c++ > 0);
        //if (Uporabnik.PraviceL.Contains("0")) AddLinkBox("Najave", "Najave.aspx", "stopnja", "Vse: " + Convert.ToInt32(Izdaje.Get(uporabnik.StoreID, 0).Rows.Count + Prevzemi.Get(uporabnik.StoreID, 0).Rows.Count).ToString(), SelectedBox == "Najave", c++ > 0);
        //if (Uporabnik.PraviceL.Contains("0")) AddLinkBox("Opravila", "Opravila.aspx", "alumni", "Vse: " + Convert.ToInt32(Opravila.Get(uporabnik.StoreID,0,200).Rows.Count).ToString(), SelectedBox == "Opravila", c++ > 0);
    }
    protected override void OnUnload(EventArgs e)
    {
        try
        {
            if (uporabnik != null) { uporabnik.Dispose(); }
        }
        catch (Exception ee)
        {
        }

    }


}
