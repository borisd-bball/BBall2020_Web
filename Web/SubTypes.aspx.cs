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

public partial class _SubTypes : System.Web.UI.Page
{
    protected string _sort;
    protected string msg = "";
    protected string _persistence;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Ostalo";
            Master.Title = "Tipi zavezancev";
            if (!Master.Uporabnik.Pravice.Contains("tipi_zavezancev")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_subtypes"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "acTitle asc";
            }
            else Session["sort_subtypes"] = _sort;
            _persistence = Convert.ToString(Session["persistence_subtypes"]) ?? "";
            DataTable dt = SubTypes.Get_w();
            if (dt.Rows.Count > 0)
            {
                if (Request.QueryString["a"] == "csv")
                {
                    Response.Clear();
                    byte[] csv = Encoding.Default.GetBytes(Splosno.Csv(dt, "tipi_zavezancev"));
                    Response.ContentType = "application/csv; name=tipi_zavezancev.csv";
                    Response.AddHeader("content-transfer-encoding", "binary");
                    Response.AddHeader("Content-Disposition", "attachment; filename=tipi_zavezancev.csv");
                    Response.OutputStream.Write(csv, 0, csv.Length);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    r_subtypes.DataSource = dt.Select("", _sort).CopyToDataTable();
                    r_subtypes.DataBind();
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