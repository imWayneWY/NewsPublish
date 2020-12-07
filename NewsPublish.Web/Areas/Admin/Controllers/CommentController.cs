using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPublish.Model.Response;
using NewsPublish.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsPublish.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommentController : Controller
    {
        private CommentService _commentService;
        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(_commentService.GetCommentList(c => true));
        }

        [HttpPost]
        public JsonResult CommentDel(int id)
        {
            if (id <= 0)
                return Json(new ResponseModel { code = 0, result = "wrong params" });
            return Json(_commentService.DeleteComment(id));
        }
    }
}
