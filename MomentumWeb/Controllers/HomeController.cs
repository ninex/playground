using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
using MomentumWeb.Models;

namespace MomentumWeb.Controllers
{
    public class HomeController : Controller
    {
        private const string Fin24Link = "http://dashboard.fin24.com/ChartData.ashx?&ticker={0}&interval=Year&period={1}";

        //
        // GET: /Home/

        public ActionResult Index()
        {
            var tickers = GetTickers();
            var quotes = new List<StockQuote>();
            var quarters = new List<StockQuarter>();

            foreach (var entry in tickers)
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = new System.Net.WebProxy("ntswhoproxy01:8080");
                    wc.Proxy.Credentials = new System.Net.NetworkCredential(@"woolworths\w7052442", "!Jan2013");
                    var data = wc.DownloadString(string.Format(Fin24Link, entry, 1));
                    var quote = JsonConvert.DeserializeObject<StockQuote>(data);
                    quotes.Add(quote);
                }
            }

            foreach (var quote in quotes)
            {
                var qtr0 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-12).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-11).Date).OrderBy(p => p.DateStamp).First();
                var qtr1 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-9).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-8).Date).OrderBy(p => p.DateStamp).First();
                var qtr2 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-6).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-5).Date).OrderBy(p => p.DateStamp).First();
                var qtr3 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-3).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-2).Date).OrderBy(p => p.DateStamp).First();
                var qtr4 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddDays(-7).Date && p.DateStamp.Date < DateTime.Now.AddDays(-1).Date).OrderByDescending(p => p.DateStamp).First();

                var quarter = new StockQuarter()
                {
                    Ticker = quote.Object[0].InstrumentIdentifier,
                    Quarter1 = qtr1.Close,
                    Quarter1Move = (qtr1.Close - qtr0.Close) / qtr0.Close * 100,
                    Quarter2 = qtr2.Close,
                    Quarter2Move = (qtr2.Close - qtr1.Close) / qtr1.Close * 100,
                    Quarter3 = qtr3.Close,
                    Quarter3Move = (qtr3.Close - qtr2.Close) / qtr2.Close * 100,
                    Quarter4 = qtr4.Close,
                    Quarter4Move = (qtr4.Close - qtr3.Close) / qtr3.Close * 100,
                    YearMove = (qtr4.Close - qtr0.Close) / qtr0.Close * 100
                };
                quarters.Add(quarter);
            }

            return View(quarters);
        }

        private List<string> GetTickers()
        {
            var list = new List<string>();

            list.Add("LON");
            list.Add("WHL");
            list.Add("VOD");

            return list;
        }

    }
}
