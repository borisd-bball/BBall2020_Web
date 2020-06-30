using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.SessionState;
using mr.bBall_Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Client_AccController : ApiController
{
    // GET api/<controller>
    public HttpResponseMessage Get(HttpRequestMessage request)
    {
        var response = Request.CreateResponse(HttpStatusCode.OK);

        return response;
    }

    // GET api/<controller>/5
    public HttpResponseMessage Get(int id, HttpRequestMessage request)
    {
        var response = Request.CreateResponse(HttpStatusCode.OK);

        return response;
    }

    // POST api/<controller>
    public HttpResponseMessage Post(HttpRequestMessage request)
    {
        var response = Request.CreateResponse(HttpStatusCode.OK);
        string lresponse = "";

        HttpSessionState s = null;

        try
        {
            string lx = request.Content.ReadAsStringAsync().Result;
            byte[] data = Convert.FromBase64String(lx);

            JObject j_req = JObject.Parse(Encoding.UTF8.GetString(data));

            string _req_type = j_req["req_type"].ToString().ToUpper();

            if (_req_type == "LOGIN")
            {
                string _req_data = "";
                string lUserName = "";
                string lPassword = "";
                string lDeviceID = "";
                int lForceLogin = 0;

                try
                {
                    _req_data = j_req["req_data"].ToString();

                    if (!String.IsNullOrEmpty(_req_data))
                    {
                        JObject jo = JObject.Parse(_req_data);

                        lUserName = Convert.ToString(jo["acUserName"]);
                        lPassword = Convert.ToString(jo["acPassword"]);
                        lDeviceID = Convert.ToString(jo["acDevID"]);
                        lForceLogin = Convert.ToInt32(jo["anForceLogin"]);
                    }

                    Uporabnik lUporabnik = new Uporabnik();

                    string lRsp = lUporabnik.login(lUserName, lPassword, lDeviceID, lForceLogin,0,"");
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lRsp));

                }
                catch (Exception exception)
                {
                    lresponse = Splosno.AddHeadDataToResponseData(0, 99999, exception.ToString(), lresponse);
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
                }

                response.Content = new StringContent(lresponse, Encoding.UTF8, "application/json");

            }
            else if (_req_type == "LOGOUT")
            {
                string _req_data = j_req["req_data"].ToString();
                int lUserID = 0;
                string lSessionID = "";
                string lDeviceID = "";
                int lForceLogin = 0;

                if (!String.IsNullOrEmpty(_req_data))
                {
                    JObject jo = JObject.Parse(_req_data);

                    lUserID = Convert.ToInt32(jo["anUserID"]);
                    lSessionID = Convert.ToString(jo["acSessionID"]);
                    lDeviceID = Convert.ToString(jo["acDevID"]);
                }

                Uporabnik lUporabnik = new Uporabnik();

                lUporabnik.logout(lUserID, lSessionID, s, lDeviceID);
                lUporabnik.Dispose();
                String lRsp = Splosno.AddHeadDataToResponseData(0, 0, "", "");

                string ldata = Convert.ToBase64String(Encoding.UTF8.GetBytes(lRsp));

                response.Content = new StringContent(ldata, Encoding.UTF8, "application/json");

            }
            else if (_req_type == "REGISTRATION")
            {
                string _req_data = "";
                string lUserName = "";
                string lPassword = "";
                string lDeviceID = "";
                string lName = "";
                string lEmail = "";
                string lUserRights = "";

                try
                {
                    _req_data = j_req["req_data"].ToString();

                    if (!String.IsNullOrEmpty(_req_data))
                    {
                        JObject jo = JObject.Parse(_req_data);

                        lUserName = Convert.ToString(jo["acUserName"]);
                        lPassword = Convert.ToString(jo["acPassword"]);
                        lDeviceID = Convert.ToString(jo["acDevID"]);
                        lName = Convert.ToString(jo["acName"]);
                        lEmail = Convert.ToString(jo["acEmail"]);
                        lUserRights = Convert.ToString(jo["acUserRights"]);
                    }

                    Uporabnik lUporabnik = new Uporabnik();

                    string lRsp = lUporabnik.Registration(lUserName, lPassword, lDeviceID, lName, lEmail, lUserRights, 0, "");
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lRsp));

                }
                catch (Exception exception)
                {
                    lresponse = Splosno.AddHeadDataToResponseData(0, 99999, exception.ToString(), lresponse);
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
                }

                response.Content = new StringContent(lresponse, Encoding.UTF8, "application/json");

            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotImplemented);
            }

        }
        catch (Exception exception)
        {
            response = Request.CreateResponse(HttpStatusCode.ExpectationFailed);
        }
        return response;
    }

    // PUT api/<controller>/5
    public HttpResponseMessage Post(int id, HttpRequestMessage request)
    {
        var response = Request.CreateResponse(HttpStatusCode.OK);

        return response;
    }

    // DELETE api/<controller>/5
    public HttpResponseMessage Delete(int id, HttpRequestMessage request)
    {
        var response = Request.CreateResponse(HttpStatusCode.OK);

        return response;

    }

}
