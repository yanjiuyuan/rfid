using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FX.Configuration;
namespace DingTalkServer
{
    public partial class DingTalkServerAddressConfig : JsonConfiguration
    {
        private static DingTalkServerAddressConfig _instance;
        static readonly string fileName = AppDomain.CurrentDomain.BaseDirectory+ "DingTalkAddressConfig.json";
        public static DingTalkServerAddressConfig GetInstance()
        {
            return _instance ?? (_instance = new DingTalkServerAddressConfig());
        }
        private DingTalkServerAddressConfig() : base(fileName)
        { 
        }

        public string  GetAccessTokenUrl { get; private set; }
        
        public string GetAttendanceListUrl { get; private set; }
        public string GetJsapiTicketUrl { get; set; }

        public string SendMessageUrl { get; set; }
        public string GetMessageListStatusUrl { get; set; }
        public string UploadMediaUrl { get; set; }
        public string DownloadFileUrl { get; set; }

        public string GetChildDeptByDeptIdUrl { get; set; }
    }
}