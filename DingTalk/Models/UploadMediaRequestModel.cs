using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkServer.Models
{
    public class UploadMediaRequestModel
    {
        public string FileName { get; set; }
        public UploadMediaType MediaType { get; set; }
    }
}