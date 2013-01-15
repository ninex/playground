using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MomentumWeb.Models
{
    public class StockQuarter
    {
        public string Ticker { get; set; }
        public string FullName { get; set; }

        public double Quarter1 { get; set; }
        public double Quarter1Move { get; set; }

        public double Quarter2 { get; set; }
        public double Quarter2Move { get; set; }

        public double Quarter3 { get; set; }
        public double Quarter3Move { get; set; }

        public double Quarter4 { get; set; }
        public double Quarter4Move { get; set; }

        public double YearMove { get; set; }

        public bool Quarter1Top5 { get; set; }
        public bool Quarter2Top5 { get; set; }
        public bool Quarter3Top5 { get; set; }
        public bool Quarter4Top5 { get; set; }
        public bool YearTop5 { get; set; }
    }
}