using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsPublish.Web.Controllers
{
    public class NewsController : Controller
    {
        private NewsService _newsService;
        public NewsController(NewsService newsService)
        {
            this._newsService = newsService;
        }
        // GET: /<controller>/
        public IActionResult Classify(int id)
        {
            if (id < 0)
                Response.Redirect("/Home/Index");
            var classify = _newsService.GetOneNewsClassify(id);
            if (classify.code == 0)
                Response.Redirect("/Home/Index");
            ViewData["ClassifyName"] = classify.data.Name;
            ViewData["Title"] = classify.data.Name;

            var newsList = _newsService.GetNewsList(c => c.NewsClassifyId == id, 6);
            ViewData["NewsList"] = newsList;

            var newCommentNews = _newsService.GetNewsCommentNewsList(c => c.NewsClassifyId == id, 5);
            ViewData["NewCommentNews"] = newCommentNews;

            return View(_newsService.GetNewsClassifyList());
        }
    }
}
