using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using mr.bBall_Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DevicesController : ApiController
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

        try
        {
            string lx = request.Content.ReadAsStringAsync().Result;
            byte[] data = Convert.FromBase64String(lx);

            JObject j_req = JObject.Parse(Encoding.UTF8.GetString(data));

            string _req_type = j_req["req_type"].ToString().ToUpper();

            if (_req_type == "GET")
            {
                string _req_data = "";
                int anID = 0;
                string lDeviceID = "";

                try
                {
                    _req_data = j_req["req_data"].ToString();

                    if (!String.IsNullOrEmpty(_req_data))
                    {
                        JObject jo = JObject.Parse(_req_data);

                        anID = Convert.ToInt32(jo["anID"]);
                        lDeviceID = Convert.ToString(jo["acDevID"]);

                    }

                    List<Devices.Device> lDevices;
                    if (anID > 0) { lDevices = Devices.Get(anID, ""); }
                    else
                    { lDevices = Devices.Get(0, lDeviceID); }

                    string ldata = JsonConvert.SerializeObject(lDevices);
                    lresponse = Splosno.AddHeadDataToResponseData(0, 0, "", ldata);
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));

                }
                catch (Exception exception)
                {
                    lresponse = Splosno.AddHeadDataToResponseData(0, 99999, exception.ToString(), lresponse);
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
                }



                response.Content = new StringContent(lresponse, Encoding.UTF8, "application/json");

            }
            else if (_req_type == "POST")
            {
                string _req_data = "";

                try
                {
                    JObject jo = null;
                    _req_data = j_req["req_data"].ToString();

                    if (!String.IsNullOrEmpty(_req_data))
                    {
                        jo = JObject.Parse(_req_data);


                        Devices.Device lDevice = new Devices.Device
                        {
                            Id = 0,
                            acDevID = (string)jo["acDevID"],
                            acTitle = (string)jo["acTitle"],
                            adInsetDate = DateTime.Now,
                            anUserIns = 99999,
                            acBT_Name = (string)jo["acBT_Name"],
                            acEmail = Convert.ToString(jo["acEmail"]),
                            adModificationDate = DateTime.Now,
                            anUserMod = 99999

                        };

                        Devices.Upload(lDevice);

                        lresponse = Splosno.AddHeadDataToResponseData(0, 0, "", "");
                        lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));

                    }
                    else
                    {
                        lresponse = Splosno.AddHeadDataToResponseData(0, Convert.ToInt32(HttpStatusCode.BadRequest), "BadRequest", "");
                        lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
                    }

                }
                catch (Exception exception)
                {
                    lresponse = Splosno.AddHeadDataToResponseData(0, 99999, exception.ToString(), lresponse);
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
                }

                response.Content = new StringContent(lresponse, Encoding.UTF8, "application/json");

            }
            else if (_req_type == "DELETE")
            {
                string _req_data = "";
                int anID = 0;

                try
                {
                    _req_data = j_req["req_data"].ToString();

                    if (!String.IsNullOrEmpty(_req_data))
                    {
                        JObject jo = JObject.Parse(_req_data);

                        anID = Convert.ToInt32(jo["anID"]);

                        List<Devices.Device> lDevices = Devices.Get(anID, "");
                        if (lDevices.Count > 0)
                        {
                            Devices.Delete(lDevices[0]);
                        }

                        lresponse = Splosno.AddHeadDataToResponseData(0, 0, "", "");
                        lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));

                    }
                    else
                    {
                        lresponse = Splosno.AddHeadDataToResponseData(0, Convert.ToInt32(HttpStatusCode.BadRequest), "BadRequest", "");
                        lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
                    }
                }
                catch (Exception exception)
                {
                    lresponse = Splosno.AddHeadDataToResponseData(0, 99999, exception.ToString(), lresponse);
                    lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
                }
            }
            else
            {
                lresponse = Splosno.AddHeadDataToResponseData(0, Convert.ToInt32(HttpStatusCode.NotImplemented), "NotImplemented", "");
                lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
            }
        }
        catch (Exception exception)
        {
            lresponse = Splosno.AddHeadDataToResponseData(0, Convert.ToInt32(HttpStatusCode.ExpectationFailed), exception.ToString(), lresponse);
            lresponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(lresponse));
        }
        return response;
    }

    // PUT api/<controller>/5
    public HttpResponseMessage Put(int id, HttpRequestMessage request)
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
