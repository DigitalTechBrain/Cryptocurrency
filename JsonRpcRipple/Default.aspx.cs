using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonRpcRipple
{
    public partial class Default : System.Web.UI.Page
    {
        public JObject InvokeMethod(string a_sMethod, params object[] a_params)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://s1.ripple.com:51234/");
            // webRequest.Credentials = Credentials;

            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            JObject joe = new JObject();
            joe["command"] = a_sMethod;
            //joe["id"] = "1";

            joe["account"] = "r9cZA1mLK5R5Am25ArfXFmqgNwjZgnfk59";
            joe["strict"] = "true";
            joe["ledger_index"] = "validated";

            if (a_params != null)
            {
                if (a_params.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (var p in a_params)
                    {
                        props.Add(p);
                    }
                    joe.Add(new JProperty("params", props));
                }
            }

            string s = JsonConvert.SerializeObject(joe);
            // serialize json for the requestf
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;

            try
            {
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (WebException we)
            {
                //inner exception is socket
                //{"A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 23.23.246.5:8332"}
                throw we;
            }
            WebResponse webResponse = null;
            try
            {
                using (webResponse = webRequest.GetResponse())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException webex)
            {

                using (Stream str = webex.Response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        var tempRet = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        return tempRet;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var ret = InvokeMethod("account_info", "index");
        }
    }
}