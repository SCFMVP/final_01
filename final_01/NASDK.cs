using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace NADemo
{
    public class NASDK
    {
        public bool isHttp = false;
        string m_logfile = "wk_na.txt";
        string m_pltip;
        string m_appid;
        string m_appsecret;
        int m_pltPort;
        string m_p12certfile;
        string m_certpassword;
        public NASDK(string platformIP, int port, string appId, string appSecret, string p12cert, string certpassword)
        {
            this.m_pltip = platformIP;
            this.m_appid = appId;
            this.m_appsecret = appSecret;
            this.m_pltPort = port;
            this.m_p12certfile = p12cert;
            this.m_certpassword = certpassword;

        }
        //获取token
        public TokenResult getToken()
        {
            TokenResult result = null;

            string apiPath = "/iocm/app/sec/v1.1.0/login";
            string body = "appId=" + this.m_appid + "&secret=" + this.m_appsecret;
            string method = "POST";
            string contenttype = "application/x-www-form-urlencoded";
            WebHeaderCollection headers = new WebHeaderCollection();
            try
            {
                ApiResult apiresult = PostUrl(apiPath, body, headers, method, contenttype, this.m_p12certfile, this.m_certpassword);
                log(apiresult.statusCode.ToString() + apiresult.result);
                TokenResult tr = JsonConvert.DeserializeObject<TokenResult>(apiresult.result);
                result = tr;
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);

                result = null;
            }
            return result;

        }
        //注册设备
        public RegDeviceResult regDevice(string token, string verifycode)
        {
            return regDevice(token, verifycode, verifycode, null, null, 0);
        }
        public RegDeviceResult regDevice(string token, string verifycode, string nodeid, string enduserid, string psk, int timeout)
        {
            RegDeviceResult result = null;
            string apiPath = "/iocm/app/reg/v1.2.0/devices?appId=" + this.m_appid;
            RegDeviceRequest rdr = new RegDeviceRequest();
            rdr.nodeId = nodeid;
            rdr.verifyCode = verifycode;
            rdr.timeout = timeout;
            string body = JsonConvert.SerializeObject(rdr);
            string method = "POST";
            string contenttype = "application/json";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("app_key", this.m_appid);
            headers.Add("Authorization", "Bearer " + token);


            try
            {
                ApiResult apiresult = PostUrl(apiPath, body, headers, method, contenttype, this.m_p12certfile, this.m_certpassword);
                log(apiresult.statusCode.ToString() + apiresult.result);
                RegDeviceResult tr = JsonConvert.DeserializeObject<RegDeviceResult>(apiresult.result);
                if (tr.deviceId.Trim() != "")
                {
                    result = tr;
                }
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);

                result = null;
            }
            return result;
        }

        //修改设备
        public string modifyDevice(string token, string deviceid, string mid, string devmodel)
        {
            string result = null;
            string apiPath = "/iocm/app/dm/v1.2.0/devices/" + deviceid + "?appId=" + this.m_appid;
            ModDeviceRequest mdr = new ModDeviceRequest();
            mdr.protocolType = "CoAP";
            mdr.manufacturerId = mid;
            mdr.model = devmodel;
            string body = JsonConvert.SerializeObject(mdr);
            string method = "PUT";
            string contenttype = "application/json";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("app_key", this.m_appid);
            headers.Add("Authorization", "Bearer " + token);


            try
            {
                ApiResult apiresult = PostUrl(apiPath, body, headers, method, contenttype, this.m_p12certfile, this.m_certpassword);
                log(apiresult.statusCode.ToString() + apiresult.result);
                result = apiresult.statusCode.ToString();

            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);


                result = null;
            }
            return result;
        }
        //订阅消息
        public string subscribe(string token, string notifytype, string callbackurl)
        {
            string result = null;
            string apiPath = "/iocm/app/sub/v1.2.0/subscribe";

            string body = "{\"notifyType\":\"" + notifytype + "\",\"callbackurl\":\"" + callbackurl + "\"}";
            string method = "POST";
            string contenttype = "application/json";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("app_key", this.m_appid);
            headers.Add("Authorization", "Bearer " + token);


            try
            {
                ApiResult apiresult = PostUrl(apiPath, body, headers, method, contenttype, this.m_p12certfile, this.m_certpassword);
                log(apiresult.statusCode.ToString() + apiresult.result);
                result = apiresult.statusCode.ToString();

            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);

                result = null;
            }
            return result;
        }
        //查询历史数据
        public HistoryDataResult queryHistoryData(string token, string deviceId, int count)
        {
            return queryHistoryData(token, deviceId, deviceId, "", "", 0, count, "", "");
        }
        public HistoryDataResult queryHistoryData(string token, string deviceId,string serviceid,int pageno,int pagesize,string starttime,string endtime)
        {
            return queryHistoryData(token, deviceId, deviceId,serviceid,"",pageno,pagesize,starttime,endtime);
        }

        public HistoryDataResult queryHistoryData(string token, string deviceId, string gatewayid, string serviceId, string property, int pageno, int pagesize, string starttime, string endtime)
        {
            HistoryDataResult result = null;
            string apiPath = string.Format("/iocm/app/data/v1.1.0/deviceDataHistory?deviceId={0}&gatewayId={1}&serviceId={2}&pageNo={3}&pageSize={4}&startTime={5}&endTime={6}&property={7}&appId={8}", deviceId, gatewayid, serviceId, pageno, pagesize, starttime, endtime, property, this.m_appid);

            string body = "";
            string method = "GET";
            string contenttype = "application/json";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("app_key", this.m_appid);
            headers.Add("Authorization", "Bearer " + token);


            try
            {
                ApiResult apiresult = PostUrl(apiPath, body, headers, method, contenttype, this.m_p12certfile, this.m_certpassword);
                log(apiresult.statusCode.ToString() + apiresult.result);
                HistoryDataResult tr = JsonConvert.DeserializeObject<HistoryDataResult>(apiresult.result);

                result = tr;

            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);

                result = null;
            }
            return result;
        }
        //下发命令
        public string sendCommand(string token, string deviceId, string callbackurl, string serviceId, string commandId, List<CommandPara> lsParas)
        {
            string result = null;
            string apiPath = string.Format("/iocm/app/cmd/v1.4.0/deviceCommands?appId={0}", this.m_appid);

            SendCommandRequest scr = new SendCommandRequest();
            scr.deviceId = deviceId;
            scr.callbackUrl = callbackurl;
            scr.command = new Command();
            scr.command.method = commandId;
            scr.command.serviceId = serviceId;
            scr.command.paras = "#commandparas#";
            string body = JsonConvert.SerializeObject(scr);
            string parabody = "{";
            foreach (CommandPara item in lsParas)
            {
                if (item.isNum) { parabody += string.Format("\"{0}\":{1},", item.paraName,item.paraValue); } else {
                    parabody += string.Format("\"{0}\":\"{1}\",", item.paraName, item.paraValue);
                }
             
            }
            parabody= parabody.TrimEnd(',')+"}";
           

            body = body.Replace("\"#commandparas#\"", parabody);

            string method = "POST";
            string contenttype = "application/json";
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("app_key", this.m_appid);
            headers.Add("Authorization", "Bearer " + token);


            try
            {
                ApiResult apiresult = PostUrl(apiPath, body, headers, method, contenttype, this.m_p12certfile, this.m_certpassword);
                log(apiresult.statusCode.ToString() + apiresult.result);
               
                result = apiresult.statusCode.ToString();

            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);

                result = null;
            }
            return result;
        }
























        private void log(string msg)
        {
            File.AppendAllText(this.m_logfile, msg);
        }
        public class ApiResult
        {
            public int statusCode;
            public string result;
            public string errcode;
            public string memo;
        }
        private ApiResult PostUrl(string apiPath, string postData, WebHeaderCollection headers, string method, string contenttype, string p12certfile, string cerpassword)
        {
            string url = string.Format("https://{0}:{1}{2}", this.m_pltip, this.m_pltPort, apiPath);
            if (isHttp)
            {
                url = string.Format("http://{0}:{1}{2}", this.m_pltip, this.m_pltPort, apiPath);
            }
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            X509Certificate2 cerCaiShang = new X509Certificate2(p12certfile, cerpassword);
            if (!isHttp)
            {
                httpRequest.ClientCertificates.Add(cerCaiShang);
            }
            httpRequest.Method = method;
            httpRequest.ContentType = contenttype;
            httpRequest.Referer = null;
            httpRequest.AllowAutoRedirect = true;
            httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            httpRequest.Accept = "*/*";
            for (int i = 0; i < headers.Count; i++)
            {
                for (int j = 0; j < headers.GetValues(i).Length; j++)
                {
                    httpRequest.Headers.Add(headers.Keys[i], headers.GetValues(i)[j]);
                }

            }

            if (isHttp) { httpRequest.ServicePoint.Expect100Continue = false; }
            //
            if (method != "GET")
            {
                Stream requestStem = httpRequest.GetRequestStream();
                StreamWriter sw = new StreamWriter(requestStem);
                sw.Write(postData);
                sw.Close();
            }


            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            Stream receiveStream = httpResponse.GetResponseStream();

            string result = string.Empty;
            using (StreamReader sr = new StreamReader(receiveStream))
            {
                result = sr.ReadToEnd();
            }
            ApiResult r = new ApiResult();
            r.result = result;
            r.statusCode = (int)httpResponse.StatusCode;

            return r;
        }
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)

        {

            if (sslPolicyErrors == SslPolicyErrors.None)

                return true;

            return true;

        }



    }
}
