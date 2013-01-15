using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MomentumWeb.Models
{
    public class StockQuote
    {
        public List<StockDay> Object { get; set; }
        public int Result { get; set; }
        public object ErrorMessage { get; set; }
    }
}