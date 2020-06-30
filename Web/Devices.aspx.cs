﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using mr.bBall_Lib;
using System.Data;
using System.Text;

public partial class _Devices : System.Web.UI.Page
{
    protected string _sort;
    protected int _id;
    protected string msg = "";
    protected string _DevID = "";
    protected string _sel_tip = "";
    protected string _filter_aktiven;
    protected string _persistence;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.SelectedBox = "Aplikacije";
            Master.Title = "Aplikacije";
            if (!Master.Uporabnik.Pravice.Contains("naprave")) throw new Exception("Nimate pravice!");
            msg = Request.QueryString["msg"] ?? "";
            _sort = Request.QueryString["sort"] ?? "";
            _id = Convert.ToInt32(Request.QueryString["id"] ?? "0");
            _DevID = Request.QueryString["DevID"] ?? "";

            if (string.IsNullOrWhiteSpace(_sort))
            {
                _sort = Convert.ToString(Session["sort_naprave"]);
                if (string.IsNullOrWhiteSpace(_sort)) _sort = "acTitle asc";
            }
            else Session["sort_naprave"] = _sort;
            _persistence = Convert.ToString(Session["persistence_naprave"]) ?? "";

            DataTable dt = Devices.Get_d();
            if (dt.Rows.Count > 0)
            {
                if (Request.QueryString["a"] == "csv")
                {
                    Response.Clear();
                    byte[] csv = Encoding.Default.GetBytes(Splosno.Csv(dt, "Aplikacije"));
                    Response.ContentType = "application/csv; name=Aplikacije.csv";
                    Response.AddHeader("content-transfer-encoding", "binary");
                    Response.AddHeader("Content-Disposition", "attachment; filename=Aplikacije.csv");
                    Response.OutputStream.Write(csv, 0, csv.Length);
                    Response.Flush();
                    Response.End();
                }
                if (Request.QueryString["a"] == "batch")
                {
                    //new Thread(Skulpture.PosljiPodatke).Start();
                }
                if (Request.QueryString["a"] == "pdf")
                {
                    //DateTime datum_racuna = DateTime.Now;
                    //List<byte[]> pdfs = new List<byte[]>();
                    //int c = 0;
                    //foreach (DataRow r in dt.Select("", "priimek,ime"))
                    //{
                    //    if (c++ > 10) break;
                    //    if (Convert.ToBoolean(r["aktiven"]))
                    //    {
                    //        Response furs_response = new Response();
                    //        string stranka_naziv = Convert.ToString(r["ime"]) + " " + Convert.ToString(r["priimek"]);
                    //        string stranka_naslov = Convert.ToString(r["naslov"]);
                    //        string stranka_posta = Convert.ToString(r["posta"]);
                    //        string stranka_kraj = Convert.ToString(r["kraj"]);
                    //        string stranka_email = Convert.ToString(r["email"]);
                    //        int id_racun = Racuni.Izdaj(0, "gotovinski", "elektronski_racun", datum_racuna, datum_racuna, datum_racuna, "INT", "ONL", "", "", "", Master.Uporabnik.Username, Master.Uporabnik.Davcna, stranka_naziv, stranka_naslov, stranka_kraj, stranka_posta, stranka_email, "", "", new List<Racuni.Vrstica>(new Racuni.Vrstica[] { new Racuni.Vrstica("Letna dovolilnica " + Master.Date.Year, 22, 40.98, 0, "0", 1, "") }), 0, false, false, out furs_response);
                    //        pdfs.Add(Racuni.GetPdf(id_racun, false));
                    //    }
                    //}
                    //pdfs.Reverse();
                    //Response.Clear();
                    //byte[] pdf = (byte[])Pdf.GeneratePdf(pdfs);
                    //Response.ContentType = "application/csv; name=LetneDovolilnice.pdf";
                    //Response.AddHeader("content-transfer-encoding", "binary");
                    //Response.AddHeader("Content-Disposition", "attachment; filename=LetneDovolilnice.pdf");
                    //Response.OutputStream.Write(pdf, 0, pdf.Length);
                    //Response.Flush();
                    //Response.End();
                }
                else
                {
                    r_naprave.DataSource = dt.Select("", _sort).CopyToDataTable();
                    r_naprave.DataBind();
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