using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using NewsPublish.Service;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsPublish.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private BannerService _bannerService;
        private IHostingEnvironment _host;

        public BannerController(BannerService bannerService, IHostingEnvironment host) {
            _bannerService = bannerService;
            _host = host;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var banner = _bannerService.GetBannerList();
            return View(banner);
        }

        public ActionResult BannerAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AddBanner(AddBanner banner, IFormCollection collection)
        {
            var files = collection.Files;
            if (files.Count > 0)
            {
                var webRootPath = _host.WebRootPath;
                // for  windows
                //string relativeDirPath = "\\BannerPic";
                // for mac
                string relativeDirPath = "/BannerPic";
                string absolutePath = webRootPath + relativeDirPath;

                string[] fileType = new string[] { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
                string extension = Path.GetExtension(files[0].FileName);
                if (fileType.Contains(extension.ToLower()))
                {
                    if (!Directory.Exists(absolutePath)) Directory.CreateDirectory(absolutePath);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    // for windows
                    //var filePath = absolutePath + "\\" + fileName;
                    // for mac
                    var filePath = absolutePath + "/" + fileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await files[0].CopyToAsync(stream);
                    }
                    banner.Image = "/BannerPic/" + fileName;
                    return Json(_bannerService.AddBanner(banner));
                }
                return Json(new ResponseModel { code = 0, result = "Image format does not supported." });
            }
            return Json(new ResponseModel { code = 0, result = "Please upload image" });
        }

        [HttpPost]
        public JsonResult DelBanner(int id)
        {
            if (id <= 0)
                return Json(new ResponseModel { code = 0, result = "Invalid banner id" });
            return Json(_bannerService.DeleteBanner(id));

        }
    }
}
