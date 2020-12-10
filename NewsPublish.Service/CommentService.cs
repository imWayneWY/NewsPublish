using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NewsPublish.Model;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Response;

namespace NewsPublish.Service
{
    public class CommentService
    {
        private Db _db;
        private NewsService _newsService;
        public CommentService(Db db, NewsService newsService)
        {
            this._db = db;
            this._newsService = newsService;
        }

        /// <summary>
        /// Add comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public ResponseModel AddComment(AddComment comment)
        {
            var news = _newsService.GetOneNews(comment.NewsId);
            if (news.code == 0)
                return new ResponseModel { code = 0, result = "News does not existed." };
            var com = new NewsComment
            {
                AddTime = DateTime.Now,
                NewsId = comment.NewsId,
                Contents = comment.Contents,
            };
            _db.NewsComment.Add(com);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "News comment add succ",
                    data = new
                    {
                        contents = comment.Contents,
                        floor = "#" + (news.data.CommentCount + 1),
                        addTime = DateTime.Now,
                    }
                };
            }
            return new ResponseModel { code = 0, result = "News comment add fail" };
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteComment(int id)
        {
            var comment = _db.NewsComment.Find(id);
            if (comment == null)
                return new ResponseModel { code = 0, result = "Comment does not exist" };

            _db.NewsComment.Remove(comment);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Comment delete succ" };
            return new ResponseModel { code = 0, result = "Comment delete fail" };
        }

        /// <summary>
        /// Get comments list
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetCommentList(Expression<Func<NewsComment, bool>> where)
        {
            var comments = _db.NewsComment.Include("News").Where(where).OrderBy(c => c.AddTime).ToList();
            var response = new ResponseModel();
            response.code = 200;
            response.result = "Comment list get succ";
            response.data = new List<CommentModel>();

            int floor = 1;
            foreach (var comment in comments)
            {
                response.data.Add(new CommentModel
                {
                    Id=comment.Id,
                    Contents = comment.Contents,
                    NewsName = comment.News.Title,
                    Remark = comment.Remark,
                    AddTime = comment.AddTime,
                    Floor = "#"+floor,
                });
                floor++;
            }

            response.data.Reverse();
            return response;
        }
    }
}
