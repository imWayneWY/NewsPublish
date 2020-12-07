using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using NewsPublish.Service;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.IO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsPublish.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : Controller
    {
        private NewsService _newsService;
        private IHostingEnvironment _host;

        public NewsController(NewsService newsService, IHostingEnvironment host)
        {
            _newsService = newsService;
            _host = host;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var newsClassify = _newsService.GetNewsClassifyList();
            return View(newsClassify);
        }

        [HttpGet]
        public JsonResult GetNews(int pageIndex, int pageSize, int classifyId, string keyword)
        {
            List<Expression<Func<News, bool>>> wheres = new List<Expression<Func<News, bool>>>();
            if (classifyId > 0)
            {
                wheres.Add(c => c.NewsClassifyId==classifyId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                wheres.Add(c => c.Title.Contains(keyword));
            }
            int total = 0;
            var news = _newsService.NewsPageQuery(pageSize, pageIndex, out total, wheres);
            return Json(new { total = total, data = news.data });
        }

        public ActionResult NewsAdd()
        {
            var newsClassifys = _newsService.GetNewsClassifyList();
            return View(newsClassifys);
        }

        [HttpPost]
        public async Task<JsonResult> NewsAdd(AddNews news, IFormCollection collection)
        {
            if (news.NewsClassifyId <= 0 || string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Contents))
                return Json(new ResponseModel { code = 0, result = "wrong params" });
            var files = collection.Files;
            if (files.Count > 0)
            {
                string webRootPath = _host.WebRootPath;
                string relativeDir = "/NewsPic";
                string absolutePath = webRootPath + relativeDir;

                string[] fileTypes = new string[] { ".gif", ".jpg", ".jpeg", ".png", ".bmp", };
                string extension = Path.GetExtension(files[0].FileName);
                if (fileTypes.Contains(extension))
                {
                    if (!Directory.Exists(absolutePath)) Directory.CreateDirectory(absolutePath);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    var filePath = absolutePath + '/' + fileName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await files[0].CopyToAsync(stream);
                    }
                    news.Image = "/NewsPic/" + fileName;
                    return Json(_newsService.AddNews(news));
                }
                return Json(new ResponseModel { code = 0, result = "Invalid image format" });
            }
            return Json(new ResponseModel { code = 0, result = "Please upload image" });
        }

        [HttpPost]
        public JsonResult NewsDel(int id)
        {
            if (id < 0)
                return Json(new ResponseModel { code = 0, result = "News does not exist" });
            return Json(_newsService.DelOneNews(id));
        }

        #region Category
        public IActionResult NewsClassify()
        {
            var newsClassify = _newsService.GetNewsClassifyList();
            return View(newsClassify);
        }

        public IActionResult NewsClassifyAdd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult NewsClassifyAdd(AddNewsClassify newsClassify)
        {
            if (string.IsNullOrEmpty(newsClassify.Name))
                return Json(new ResponseModel { code = 0, result="Category can not be empty" });
            return Json(_newsService.AddNewsClassify(newsClassify));
        }

        public IActionResult NewsClassifyEdit(int id)
        {
            return View(_newsService.GetOneNewsClassify(id));
        }

        [HttpPost]
        public JsonResult NewsClassifyEdit(EditNewsClassify newsClassify)
        {
            if (string.IsNullOrEmpty(newsClassify.Name))
                return Json(new ResponseModel { code = 0, result = "Category name is invalid" });
            return Json(_newsService.EditNewsClassify(newsClassify));
        }
        #endregion
    }
}
