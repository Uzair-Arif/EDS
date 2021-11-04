using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VTS.ApiModels
{
    public class BaseModel
    {
        public bool success { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }
}