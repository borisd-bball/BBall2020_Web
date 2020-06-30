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
using Spock.Davcna;
using System.Threading;
using mr.bBall_Lib;

public partial class _Uporabniki : System.Web.UI.Page
{
    protected string _sort;
    protected string msg = "";
    protected string _persistence;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Uporabniki";
            if (!Master.Uporabnik.Pravice.Contains("uporabniki")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "acLastName asc";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_uporabniki"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "acLastName asc";
            }
            else Session["sort_uporabniki"] = _sort;
            _persistence = Convert.ToString(Session["persistence_uporabniki"]) ?? "";
            DataTable dt = Uporabniki.Get();
            if (dt.Rows.Count > 0)
            {
                if (Request.QueryString["a"] == "csv")
                {
                    Response.Clear();
                    byte[] csv = Encoding.Default.GetBytes(Splosno.Csv(dt, "uporabniki"));
                    Response.ContentType = "application/csv; name=Uporabniki.csv";
                    Response.AddHeader("content-transfer-encoding", "binary");
                    Response.AddHeader("Content-Disposition", "attachment; filename=Uporabniki.csv");
                    Response.OutputStream.Write(csv, 0, csv.Length);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    r_uporabniki.DataSource = dt.Select("", _sort).CopyToDataTable();
                    r_uporabniki.DataBind();
                }
            }
            if (!string.IsNullOrWhiteSpace(msg)) Master.SetMessage(msg);
        }
        catch (Exception ee)
        {
            Master.SetMessage(ee);
        }
    }
}