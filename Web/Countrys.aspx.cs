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

public partial class _Countrys : System.Web.UI.Page
{
    protected string _sort;
    protected string msg = "";
    protected string _persistence;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Države";
            if (!Master.Uporabnik.Pravice.Contains("drzave")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_countrys"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "acTitle asc";
            }
            else Session["sort_countrys"] = _sort;
            _persistence = Convert.ToString(Session["persistence_countrys"]) ?? "";
            DataTable dt = Countrys.Get_d();
            if (dt.Rows.Count > 0)
            {
                if (Request.QueryString["a"] == "csv")
                {
                    Response.Clear();
                    byte[] csv = Encoding.Default.GetBytes(Splosno.Csv(dt, "drzave"));
                    Response.ContentType = "application/csv; name=Stranke.csv";
                    Response.AddHeader("content-transfer-encoding", "binary");
                    Response.AddHeader("Content-Disposition", "attachment; filename=Drzave.csv");
                    Response.OutputStream.Write(csv, 0, csv.Length);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    r_countrys.DataSource = dt.Select("", _sort).CopyToDataTable();
                    r_countrys.DataBind();
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