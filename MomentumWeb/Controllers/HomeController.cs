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

        //
        // GET: /Home/

        public ActionResult Index()
        {
            Common.Builder.Build();
            var quarters = Common.Builder.GetStocks();

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
    }
}
