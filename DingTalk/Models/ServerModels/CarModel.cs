using DingTalk.Models.DingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalk.Models.ServerModels
{
    public class CarModel
    {
       public string name { get; set; }
       public List<Car> cars { get; set; }
    }
}