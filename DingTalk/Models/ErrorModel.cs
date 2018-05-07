using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models
{
    public class ErrorModel
    {
        //errorCode 0 正常  
        public int errorCode { get; set; }

        public string errorMessage { get; set; }

        public string Content { get; set; }

        public bool IsError { get; set; }
    }
}