using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NADemo
{
    /// <summary>
    /// 设备profile：data1 int   data2 string(8,32)  ret1 int ret2 string(8,32) para1 para2 para12 para22  
    /// 0xf0上报0xff命令相应0x91 cmd1 0x92 cmd2
    /// </summary>
    public class TokenResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string accessToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tokenType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string refreshToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expiresIn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string scope { get; set; }
    }

    public class RegDeviceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string verifyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nodeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int timeout { get; set; }
    }

    public class RegDeviceResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string deviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string verifyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int timeout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string psk { get; set; }
    }


    public class ModDeviceRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string endUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string manufacturerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string protocolType { get; set; }
    }




    public class DeviceDataHistoryDTOsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string deviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string gatewayId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string serviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> data { get; set; }
        public string DataString {
              get  {
                string result = "";
                foreach (KeyValuePair<string,string> item in data)
                {
                    result += item.Key + ":" + item.Value + "\r\n";
                }
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string timestamp { get; set; }
    }

    public class HistoryDataResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int totalCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pageNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DeviceDataHistoryDTOsItem> deviceDataHistoryDTOs { get; set; }
    }

    
    public class CommandPara
    {
        public string paraName { get; set; }
        public string paraValue { get; set; }
        public bool isNum { get; set; }
    }
    public class Command
    {
        /// <summary>
        /// 
        /// </summary>
        public string serviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paras { get; set; }
    }

    public class SendCommandRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string deviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Command command { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string callbackUrl { get; set; }
    }


}
