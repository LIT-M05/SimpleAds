using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleAds.Data;
using System.IO;
using SimpleAds.Web.Models;

namespace SimpleAds.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SimpleAdDb db = new SimpleAdDb(Properties.Settings.Default.ConStr);
            IEnumerable<SimpleAd> ads = db.GetAds();
            List<string> ids = new List<string>();
            if (Request.Cookies["AdIds"] != null)
            {
                ids = Request.Cookies["AdIds"].Value.Split(',').ToList();
            }

            //List<AdViewModel> vms = new List<AdViewModel>();
            //foreach (SimpleAd ad in ads)
            //{
            //    vms.Add(new AdViewModel
            //    {
            //        Ad = ad,
            //        CanDelete = ids.Contains(ad.Id.ToString())
            //    });
            //}

            return View(new HomePageViewModel
            {
                Ads = ads.Select(ad =>
                {
                    return new AdViewModel
                    {
                        Ad = ad,
                        CanDelete = ids.Contains(ad.Id.ToString())
                    };
                })
            });
        }

        public ActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewAd(SimpleAd ad)
        {
            SimpleAdDb db = new SimpleAdDb(Properties.Settings.Default.ConStr);
            db.AddSimpleAd(ad);
            string ids = "";
            HttpCookie cookie = Request.Cookies["AdIds"];
            if (cookie != null)
            {
                ids = $"{cookie.Value},";
            }
            ids += ad.Id;
            Response.Cookies.Add(new HttpCookie("AdIds", ids));
            return Redirect("/");
        }

        [HttpPost]
        public ActionResult DeleteAd(int id)
        {
            SimpleAdDb db = new SimpleAdDb(Properties.Settings.Default.ConStr);
            db.Delete(id);
            return Redirect("/");
        }
    }
}