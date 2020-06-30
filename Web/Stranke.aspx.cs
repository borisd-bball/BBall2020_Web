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
using System.Threading;
using mr.bBall_Lib;

public partial class _Stranke : System.Web.UI.Page
{
    protected string _sort;
    protected string msg = "";
    protected string _persistence;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Stranke";
            if (!Master.Uporabnik.Pravice.Contains("stranke")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_stranke"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "acShortTitle asc";
            }
            else Session["sort_stranke"] = _sort;
            _persistence = Convert.ToString(Session["persistence_stranke"]) ?? "";
            DataTable dt = Stranke.Get_d();
            if (dt.Rows.Count > 0)
            {
                if (Request.QueryString["a"] == "csv")
                {
                    Response.Clear();
                    byte[] csv = Encoding.Default.GetBytes(Splosno.Csv(dt, "stranke"));
                    Response.ContentType = "application/csv; name=Stranke.csv";
                    Response.AddHeader("content-transfer-encoding", "binary");
                    Response.AddHeader("Content-Disposition", "attachment; filename=Stranke.csv");
                    Response.OutputStream.Write(csv, 0, csv.Length);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    r_stranke.DataSource = dt.Select("", _sort).CopyToDataTable();
                    r_stranke.DataBind();
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