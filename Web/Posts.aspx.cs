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

public partial class _Posts : System.Web.UI.Page
{
    protected string _sort;
    protected string msg = "";
    protected string _persistence;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Pošte";
            if (!Master.Uporabnik.Pravice.Contains("poste")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_posts"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "acTitle asc";
            }
            else Session["sort_posts"] = _sort;
            _persistence = Convert.ToString(Session["persistence_posts"]) ?? "";
            DataTable dt = Posts.Get_w();
            if (dt.Rows.Count > 0)
            {
                if (Request.QueryString["a"] == "csv")
                {
                    Response.Clear();
                    byte[] csv = Encoding.Default.GetBytes(Splosno.Csv(dt, "poste"));
                    Response.ContentType = "application/csv; name=Poste.csv";
                    Response.AddHeader("content-transfer-encoding", "binary");
                    Response.AddHeader("Content-Disposition", "attachment; filename=Poste.csv");
                    Response.OutputStream.Write(csv, 0, csv.Length);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    r_posts.DataSource = dt.Select("", _sort).CopyToDataTable();
                    r_posts.DataBind();
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