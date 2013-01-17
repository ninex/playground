using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MomentumWeb.Models;
using System.Net;
using Newtonsoft.Json;
using System.Data.Objects;

namespace MomentumWeb.Common
{
    public class Builder
    {
        private const string Fin24Link = "http://dashboard.fin24.com/ChartData.ashx?&ticker={0}&interval=Year&period={1}";
        private const string Companies = "http://dashboard.fin24.com/ajax/PriceDataService.svc/GetShareCompaniesByName";

        public static void Build()
        {
            var tickers = GetTickers();

            using (MomentumData.MomentumEntities ctx = new MomentumData.MomentumEntities())
            {
                foreach (var entry in tickers)
                {
                    var last = ctx.StockDays.Where(p => p.Ticker == entry).OrderByDescending(p => p.DateStamp).FirstOrDefault();
                    if (last == null || last.DateStamp.Date < DateTime.Now.AddDays(-3).Date)
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.Proxy = new System.Net.WebProxy("ntswhoproxy01:8080");
                            wc.Proxy.Credentials = new System.Net.NetworkCredential(@"woolworths\w7052442", "!Jan2013");
                            try
                            {
                                var data = wc.DownloadString(string.Format(Fin24Link, entry, 1));
                                var quote = JsonConvert.DeserializeObject<StockQuote>(data);
                                if (quote != null && quote.Result == 1)
                                {
                                    foreach (var item in quote.Object)
                                    {
                                        if (item.DateStamp.Date <= DateTime.Now.AddDays(-1).Date && ctx.StockDays.Count(p => p.Ticker == item.InstrumentIdentifier && p.DateStamp.Year == item.DateStamp.Year && p.DateStamp.Month == item.DateStamp.Month && p.DateStamp.Day == item.DateStamp.Day) == 0)
                                        {
                                            var newEntry = new MomentumData.StockDay();

                                            newEntry.Close = item.Close;
                                            newEntry.DateStamp = item.DateStamp;
                                            newEntry.High = item.High;
                                            newEntry.Low = item.Low;
                                            newEntry.Name = item.InstrumentName;
                                            newEntry.Open = item.Open;
                                            newEntry.Ticker = item.InstrumentIdentifier;
                                            newEntry.Volume = item.Volume;

                                            ctx.StockDays.AddObject(newEntry);
                                        }
                                    }
                                    ctx.SaveChanges();
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        public static List<StockQuarter> GetStocks()
        {
            var quarters = new List<StockQuarter>();

            using (MomentumData.MomentumEntities ctx = new MomentumData.MomentumEntities())
            {
                foreach (var ticker in GetTickers())
                {
                    var quotes = ctx.StockDays.Where(p => p.Ticker == ticker).ToList();
                    var qtr0 = quotes.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-12).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-11).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr1 = quotes.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-9).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-8).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr2 = quotes.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-6).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-5).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr3 = quotes.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-3).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-2).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr4 = quotes.Where(p => p.DateStamp.Date >= DateTime.Now.AddDays(-7).Date && p.DateStamp.Date < DateTime.Now.AddDays(-1).Date).OrderByDescending(p => p.DateStamp).FirstOrDefault();
                    if (qtr0 != null && qtr1 != null && qtr2 != null && qtr3 != null && qtr4 != null)
                    {
                        var full = ctx.StockDays.First(p => p.Ticker == ticker).Name;
                        var quarter = new StockQuarter()
                        {
                            Ticker = ticker,
                            FullName = full,
                            Quarter1 = qtr1.Close,
                            Quarter1Move = (qtr1.Close - qtr0.Close) / qtr0.Close * 100,
                            Quarter2 = qtr2.Close,
                            Quarter2Move = (qtr2.Close - qtr1.Close) / qtr1.Close * 100,
                            Quarter3 = qtr3.Close,
                            Quarter3Move = (qtr3.Close - qtr2.Close) / qtr2.Close * 100,
                            Quarter4 = qtr4.Close,
                            Quarter4Move = (qtr4.Close - qtr3.Close) / qtr3.Close * 100,
                            YearMove = (qtr4.Close - qtr0.Close) / qtr0.Close * 100,
                            Quarter1Top5 = false,
                            Quarter2Top5 = false,
                            Quarter3Top5 = false,
                            Quarter4Top5 = false,
                            YearTop5 = false
                        };
                        quarters.Add(quarter);
                    }
                }
            }

            return quarters;
        }

        private static List<string> GetTickers()
        {
            var list = new List<string>();

            using (WebClient wc = new WebClient())
            {
                wc.Proxy = new System.Net.WebProxy("ntswhoproxy01:8080");
                wc.Proxy.Credentials = new System.Net.NetworkCredential(@"woolworths\w7052442", "!Jan2013");
                var data = wc.DownloadString(Companies);
                var tickersList = JsonConvert.DeserializeObject<Tickers>(data);

                foreach (var item in tickersList.Object)
                {
                    list.Add(item.Key);
                }
            }

            return list;
        }
    }
}