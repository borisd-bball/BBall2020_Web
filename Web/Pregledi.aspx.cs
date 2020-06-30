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

public partial class _Pregledi : System.Web.UI.Page
{
    protected int _mat_kartica_stetje = 0;
    protected int _zal_lokacije_stetje = 0;
    protected int _mat_kartica_fifo_stetje = 0;
    protected string msg = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Pregledi";
            Master.Title = "Pregledi";
            if (!Master.Uporabnik.Pravice.Contains("pregled")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            //_mat_kartica_stetje = Opravila.Get(Master.Uporabnik.StoreID,0,101).Rows.Count;
            if (!string.IsNullOrWhiteSpace(msg)) Master.SetMessage(msg);
        }
        catch (Exception ee)
        {
            Master.SetMessage(ee);
        }
    }
}