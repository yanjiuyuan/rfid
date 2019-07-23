using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class FileModel
    {
        public string FileName { get; set; }

        public string Base64String { get; set; }

        public string OldMediaId { get; set; }

        public string TaskId { get; set; }
    }
}