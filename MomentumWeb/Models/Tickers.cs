using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MomentumWeb.Models
{
    public class Tickers
    {
        public object ErrorMessage { get; set; }
        public List<TickerObject> Object { get; set; }
        public int Result { get; set; }
    }
    public class TickerObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}