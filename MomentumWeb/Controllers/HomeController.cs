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
                    //wc.Proxy = new System.Net.WebProxy("ntswhoproxy01:8080");
                    //wc.Proxy.Credentials = new System.Net.NetworkCredential(@"woolworths\w7052442", "!Jan2013");
                    var data = wc.DownloadString(string.Format(Fin24Link, entry, 1));
                    var quote = JsonConvert.DeserializeObject<StockQuote>(data);
                    quotes.Add(quote);
                }
            }

            foreach (var quote in quotes)
            {
                if (quote.Result == 1)
                {
                    var qtr0 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-12).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-11).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr1 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-9).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-8).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr2 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-6).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-5).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr3 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddMonths(-3).Date && p.DateStamp.Date < DateTime.Now.AddMonths(-2).Date).OrderBy(p => p.DateStamp).FirstOrDefault();
                    var qtr4 = quote.Object.Where(p => p.DateStamp.Date >= DateTime.Now.AddDays(-7).Date && p.DateStamp.Date < DateTime.Now.AddDays(-1).Date).OrderByDescending(p => p.DateStamp).FirstOrDefault();
                    if (qtr0 != null && qtr1 != null && qtr2 != null && qtr3 != null && qtr4 != null)
                    {
                        var quarter = new StockQuarter()
                        {
                            Ticker = quote.Object[0].InstrumentIdentifier,
                            FullName = quote.Object[0].InstrumentName,
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

            foreach (var entry in quarters.OrderByDescending(p => p.Quarter1Move).Take(5))
            {
                entry.Quarter1Top5 = true;
            }

            foreach (var entry in quarters.OrderByDescending(p => p.Quarter2Move).Take(5))
            {
                entry.Quarter2Top5 = true;
            }

            foreach (var entry in quarters.OrderByDescending(p => p.Quarter3Move).Take(5))
            {
                entry.Quarter3Top5 = true;
            }

            foreach (var entry in quarters.OrderByDescending(p => p.Quarter4Move).Take(5))
            {
                entry.Quarter4Top5 = true;
            }

            foreach (var entry in quarters.OrderByDescending(p => p.YearMove).Take(5))
            {
                entry.YearTop5 = true;
            }

            return View(quarters);
        }

        private List<string> GetTickers()
        {
            var list = new List<string>();
            
            list.Add("ASA");
            list.Add("ABO");
            list.Add("ACP");
            list.Add("AIP");
            list.Add("ADR");
            list.Add("ADH");
            list.Add("AFE");
            list.Add("AFR");
            list.Add("AFO");
            list.Add("AOO");
            list.Add("ABL");
            list.Add("ADW");
            list.Add("AME");
            list.Add("AFX");
            list.Add("ARIM");
            list.Add("AFT");
            list.Add("ACT");
            list.Add("AGI");
            list.Add("AET");
            list.Add("AFB");
            list.Add("ACD");
            list.Add("ATN");
            list.Add("ALT");
            list.Add("AMA");
            list.Add("AER");
            list.Add("ABP");
            list.Add("AEC");
            list.Add("AGL");
            list.Add("AMS");
            list.Add("ANG");
            list.Add("ARHP");
            list.Add("ARQ");
            list.Add("APA");
            list.Add("APE");
            list.Add("AQP");
            list.Add("ART");
            list.Add("APN");
            list.Add("ASR");
            list.Add("ARL");
            list.Add("APK");
            list.Add("ATS");
            list.Add("AEG");
            list.Add("AVI");
            list.Add("AWT");
            list.Add("BAW");
            list.Add("BJM");
            list.Add("BSR");
            list.Add("BEE");
            list.Add("BEG");
            list.Add("BEL");
            list.Add("BIL");
            list.Add("BVT");
            list.Add("BCF");
            list.Add("BAT");
            list.Add("BRC");
            list.Add("BRT");
            list.Add("BDM");
            list.Add("BCX");
            list.Add("BTG");
            list.Add("CBS");
            list.Add("CDZ");
            list.Add("CAE");
            list.Add("CPL");
            list.Add("CCO");
            list.Add("CPI");
            list.Add("CRG");
            list.Add("CSB");
            list.Add("CAT");
            list.Add("CRM");
            list.Add("CLM");
            list.Add("CLE");
            list.Add("CLR");
            list.Add("COM");
            list.Add("CMH");
            list.Add("CMA");
            list.Add("CCL");
            list.Add("CNX");
            list.Add("CND");
            list.Add("CNL");
            list.Add("CML");
            list.Add("CVS");
            list.Add("CKS");
            list.Add("CUL");
            list.Add("DCT");
            list.Add("DTC");
            list.Add("DEC");
            list.Add("DEL");
            list.Add("DMR");
            list.Add("DGC");
            list.Add("DIDDT");
            list.Add("DSY");
            list.Add("DST");
            list.Add("DAW");
            list.Add("DIV");
            list.Add("DLV");
            list.Add("DRDD");
            list.Add("ECO");
            list.Add("ELD");
            list.Add("ELB");
            list.Add("ELH");
            list.Add("EMI");
            list.Add("ENL");
            list.Add("ERM");
            list.Add("ENV");
            list.Add("EOH");
            list.Add("EUR");
            list.Add("EXL");
            list.Add("EXX");
            list.Add("EXO");
            list.Add("FVT");
            list.Add("FBR");
            list.Add("FRT");
            list.Add("FSR");
            list.Add("FIU");
            list.Add("FCPD");
            list.Add("FOS");
            list.Add("GIJ");
            list.Add("GMB");
            list.Add("GLL");
            list.Add("GOGOF");
            list.Add("GDF");
            list.Add("GDH");
            list.Add("GBG");
            list.Add("GND");
            list.Add("GRF");
            list.Add("GRT");
            list.Add("GVGVM");
            list.Add("HAL");
            list.Add("HAR");
            list.Add("HVL");
            list.Add("HCI");
            list.Add("HPA");
            list.Add("HWN");
            list.Add("HDC");
            list.Add("HWHWA");
            list.Add("HTP");
            list.Add("IFH");
            list.Add("IFR");
            list.Add("ILA");
            list.Add("ILV");
            list.Add("IMPO");
            list.Add("IBPL");
            list.Add("IFW");
            list.Add("ILT");
            list.Add("ITR");
            list.Add("INL");
            list.Add("INP");
            list.Add("IVT");
            list.Add("IPS");
            list.Add("ISA");
            list.Add("ITE");
            list.Add("ITXUK");
            list.Add("JSC");
            list.Add("JCD");
            list.Add("JDG");
            list.Add("JDH");
            list.Add("JCM");
            list.Add("JNC");
            list.Add("JSE");
            list.Add("JNC");
            list.Add("KGM");
            list.Add("KIR");
            list.Add("KAP");
            list.Add("KLG");
            list.Add("KEL");
            list.Add("KNG");
            list.Add("KIO");
            list.Add("KWV");
            list.Add("LAB");
            list.Add("LMID");
            list.Add("LEW");
            list.Add("LBH");
            list.Add("LHC");
            list.Add("LNF");
            list.Add("LON");
            list.Add("LAF");
            list.Add("MCU");
            list.Add("MDN");
            list.Add("MKL");
            list.Add("MSS");
            list.Add("MAS");
            list.Add("MSM");
            list.Add("MTZ");
            list.Add("MDC");
            list.Add("MRF");
            list.Add("MTL");
            list.Add("MTA");
            list.Add("MML");
            list.Add("MEMTX");
            list.Add("MFL");
            list.Add("MET");
            list.Add("MMG");
            list.Add("MMH");
            list.Add("MLA");
            list.Add("MMI");
            list.Add("MOB");
            list.Add("MNY");
            list.Add("MTE");
            list.Add("MPC");
            list.Add("MTN");
            list.Add("MUR");
            list.Add("MST");
            list.Add("MVG");
            list.Add("MVL");
            list.Add("MVS");
            list.Add("NPK");
            list.Add("NPN");
            list.Add("NED");
            list.Add("NBKP");
            list.Add("NTC");
            list.Add("NAI");
            list.Add("NCA");
            list.Add("NCS");
            list.Add("NHM");
            list.Add("NWL");
            list.Add("OAO");
            list.Add("OAS");
            list.Add("OCE");
            list.Add("OCT");
            list.Add("OML");
            list.Add("OMN");
            list.Add("OLG");
            list.Add("OPT");
            list.Add("PAM");
            list.Add("PAP");
            list.Add("PBT");
            list.Add("PCN");
            list.Add("PSC");
            list.Add("PGR");
            list.Add("PET");
            list.Add("PHM");
            list.Add("PWK");
            list.Add("PIK");
            list.Add("PNG");
            list.Add("PNC");
            list.Add("PLL");
            list.Add("PMM");
            list.Add("PPC");
            list.Add("PMA");
            list.Add("PMV");
            list.Add("PGF");
            list.Add("PSG");
            list.Add("PPE");
            list.Add("PPR");
            list.Add("QPG");
            list.Add("QUY");
            list.Add("RACP");
            list.Add("RBW");
            list.Add("RBX");
            list.Add("RAH");
            list.Add("RDF");
            list.Add("RIN");
            list.Add("REM");
            list.Add("RES");
            list.Add("RSG");
            list.Add("RLO");
            list.Add("RTN");
            list.Add("CFR");
            list.Add("RMH");
            list.Add("RBP");
            list.Add("SBL");
            list.Add("SAB");
            list.Add("SBV");
            list.Add("SAL");
            list.Add("SLM");
            list.Add("SNT");
            list.Add("SAP");
            list.Add("SFN");
            list.Add("SOL");
            list.Add("SCN");
            list.Add("SER");
            list.Add("SDH");
            list.Add("SKJ");
            list.Add("SNU");
            list.Add("STO");
            list.Add("SHP");
            list.Add("SBG");
            list.Add("SIM");
            list.Add("SYA");
            list.Add("SLO");
            list.Add("SOV");
            list.Add("SPA");
            list.Add("SUM");
            list.Add("SPS");
            list.Add("SUR");
            list.Add("SQE");
            list.Add("SBK");
            list.Add("SHF");
            list.Add("SHFF");
            list.Add("SLL");
            list.Add("STA");
            list.Add("SUI");
            list.Add("SPG");
            list.Add("SYC");
            list.Add("TAS");
            list.Add("TAW");
            list.Add("TKG");
            list.Add("TBX");
            list.Add("BVT");
            list.Add("DON");
            list.Add("TFG");
            list.Add("BSB");
            list.Add("SPP");
            list.Add("TBS");
            list.Add("TON");
            list.Add("TDH");
            list.Add("TPC");
            list.Add("TMT");
            list.Add("TRE");
            list.Add("TTO");
            list.Add("TRU");
            list.Add("UCS");
            list.Add("UTR");
            list.Add("VLE");
            list.Add("VMK");
            list.Add("VOD");
            list.Add("VST");
            list.Add("VIL");
            list.Add("VIF");
            list.Add("VKE");
            list.Add("VUN");
            list.Add("WEA");
            list.Add("WLL");
            list.Add("WSL");
            list.Add("WEZ");
            list.Add("WBO");
            list.Add("WNH");
            list.Add("WHL");
            list.Add("YRK");
            list.Add("ZCI");
            list.Add("ZSA");
            list.Add("ZPT");

            return list;
        }

    }
}
