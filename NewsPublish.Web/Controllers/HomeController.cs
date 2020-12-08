using Microsoft.AspNetCore.Mvc;
using NewsPublish.Service;
using NewsPublish.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPublish.Web.Controllers
{
    public class HomeController : Controller
    {
        private NewsService _newsService;
        private BannerService _bannerService;
        public HomeController(NewsService newsService, BannerService bannerService)
        {
            this._newsService = newsService;
            this._bannerService = bannerService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            return View(_newsService.GetNewsClassifyList());
        }

        [HttpGet]
        public JsonResult GetBanner()
        {
            return Json(_bannerService.GetBannerList());
        }

        [HttpGet]
        public JsonResult GetTotalNews()
        {
            return Json(_newsService.GetNewsCount(c => true));
        }

        [HttpGet]
        public JsonResult GetHomeNews()
        {
            return Json(_newsService.GetNewsList(c => true, 6));
        }
    }
}
