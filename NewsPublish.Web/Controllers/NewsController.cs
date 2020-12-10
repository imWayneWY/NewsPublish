using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Response;
using NewsPublish.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsPublish.Web.Controllers
{
    public class NewsController : Controller
    {
        private NewsService _newsService;
        private CommentService _commentService;
        public NewsController(NewsService newsService, CommentService commentService)
        {
            this._newsService = newsService;
            this._commentService = commentService;
        }
        // GET: /<controller>/
        public IActionResult Classify(int id)
        {
            if (id < 0)
                Response.Redirect("/Home/Index");
            var classify = _newsService.GetOneNewsClassify(id);
            ViewData["ClassifyName"] = "News";
            ViewData["Title"] = "News";
            ViewData["NewsList"] = new ResponseModel();
            ViewData["NewCommentNews"] = new ResponseModel();
            if (classify.code == 0)
            {
                Response.Redirect("/Home/Index");
            } else
            {
                ViewData["ClassifyName"] = classify.data.Name;
                ViewData["Title"] = classify.data.Name;

                var newsList = _newsService.GetNewsList(c => c.NewsClassifyId == id, 6);
                ViewData["NewsList"] = newsList;

                var newCommentNews = _newsService.GetNewsCommentNewsList(c => c.NewsClassifyId == id, 5);
                ViewData["NewCommentNews"] = newCommentNews;

            }
            return View(_newsService.GetNewsClassifyList());
        }

        public IActionResult Detail(int id)
        {
            if (id<0)
                Response.Redirect("/Home/Index");

            var news = _newsService.GetOneNews(id);
            ViewData["Title"] = "Detail Page";
            ViewData["News"] = new ResponseModel();
            ViewData["RecommendNews"] = new ResponseModel();
            ViewData["Comments"] = new ResponseModel();

            if (news.code == 0)
            {
                Response.Redirect("/Home/Index");
            }
            else
            {
                ViewData["Title"] = news.data.Title + " - Detail Page";
                ViewData["News"] = news.data;
                ViewData["RecommendNews"] = _newsService.GetRecommendNewsList(id);
                ViewData["Comments"] = _commentService.GetCommentList(c => c.NewsId == id);
            }
            return View(_newsService.GetNewsClassifyList());
        }
    }
}
