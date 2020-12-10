using Microsoft.EntityFrameworkCore;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NewsPublish.Service
{
    public class NewsService
    {
        private Db _db;
        public NewsService(Db db)
        {
            this._db = db;
        }
        
        /// <summary>
        /// Add news category
        /// </summary>
        /// <param name="newsClassify"></param>
        /// <returns></returns>
        public ResponseModel AddNewsClassify(AddNewsClassify newsClassify)
        {
            var exist = _db.NewsClassify.FirstOrDefault(c => c.Name == newsClassify.Name) != null;
            if (exist)
                return new ResponseModel { code = 0, result = "This category has existed!" };
            var classify = new NewsClassify { Name = newsClassify.Name, Sort = newsClassify.Sort, Remark = newsClassify.Remark };
            _db.NewsClassify.Add(classify);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "News category add succ!" };
            return new ResponseModel { code = 0, result = "News category delete fail!" };
        }

        /// <summary>
        /// Get a news category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel GetOneNewsClassify(int id)
        {
            var classify = _db.NewsClassify.Find(id);
            if (classify == null)
                return new ResponseModel { code = 0, result = "This category does not existed" };
            return new ResponseModel
            {
                code = 200,
                result = "News category get succ!",
                data = new NewsClassifyModel
                {
                    Id = classify.Id,
                    Sort = classify.Sort,
                    Remark = classify.Remark,
                    Name = classify.Name,
                }
            };
        }

        /// <summary>
        /// Get a category
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        private NewsClassify GetOneNewsClassify(Expression<Func<NewsClassify, bool>> where)
        {
            return _db.NewsClassify.FirstOrDefault(where);
        }
        /// <summary>
        /// Update a news category
        /// </summary>
        /// <param name="newsClassify"></param>
        /// <returns></returns>
        public ResponseModel EditNewsClassify(EditNewsClassify newsClassify)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == newsClassify.Id);
            if (classify == null)
                return new ResponseModel { code = 0, result = "This category does not existed" };
            classify.Name = newsClassify.Name;
            classify.Sort = newsClassify.Sort;
            classify.Remark = newsClassify.Remark;
            _db.NewsClassify.Update(classify);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Category update succ!" };
            return new ResponseModel { code = 0, result = "Category update fail" };
        }


        /// <summary>
        /// Get category list
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetNewsClassifyList()
        {
            var classifys = _db.NewsClassify.OrderByDescending(c => c.Sort).ToList();
            var response = new ResponseModel { code = 200, result = "Category list get succ!" };
            response.data = new List<NewsClassifyModel>();
            foreach(var classify in classifys)
            {
                response.data.Add(new NewsClassifyModel { 
                    Id = classify.Id,
                    Name = classify.Name,
                    Sort = classify.Sort,
                    Remark = classify.Remark,
                });
            }
            return response;
        }
    
        
        //---------------------------------------------------------------------------------------------------------------
        // News Services
        //---------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Add news
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        public ResponseModel AddNews(AddNews news)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == news.NewsClassifyId);
            if (classify == null)
                return new ResponseModel { code = 0, result = "Category does not exist!" };
            var n = new News {
                NewsClassifyId = news.NewsClassifyId,
                Title = news.Title,
                Image = news.Image,
                Contents = news.Contents,
                PublishDate = DateTime.Now,
                Remark = news.Remark,
            };
            _db.News.Add(n);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Add news succ!" };
            return new ResponseModel { code = 0, result = "Add news fail!" };
        }

        /// <summary>
        /// Get a news
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel GetOneNews(int id)
        {
            var news = _db.News.Include("NewsClassify").Include("NewsComment").FirstOrDefault(c => c.Id == id);
            if (news == null)
                return new ResponseModel { code = 0, result = "News does not exist!" };
            return new ResponseModel
            {
                code = 200,
                result = "Get news succ!",
                data = new NewsModel
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark,
                    CommentCount = news.NewsComment.Count(),
                    ClassifyName = news.NewsClassify.Name,
                }
            };
        }

        /// <summary>
        /// Delete a news
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DelOneNews(int id)
        {
            var news = _db.News.Include("NewsClassify").Include("NewsComment").FirstOrDefault(c => c.Id == id);
            if (news == null)
                return new ResponseModel { code = 0, result = "News does not exist!" };

            _db.News.Remove(news);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Delete news succ!" };
            return new ResponseModel { code = 0, result = "Delete news fail!" };

        }


        /// <summary>
        /// Query News by pages
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public ResponseModel NewsPageQuery(int pageSize, int pageIndex, out int total, List<Expression<Func<News, bool>>> where)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment");
            foreach (var item in where)
            {
                list = list.Where(item);
            }
            total = list.Count();

            var pageData = list.OrderByDescending(c => c.PublishDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            var response = new ResponseModel
            {
                code = 200,
                result = "News pages query succ",
            };
            response.data = new List<NewsModel>();
            foreach (var news in pageData)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents.Length > 50 ? news.Contents.Substring(0,50)+"..." : news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark,
                    CommentCount = news.NewsComment.Count(),
                    ClassifyName = news.NewsClassify.Name,
                });
            }
            return response;
        }

        /// <summary>
        /// Query news list by count
        /// </summary>
        /// <param name="where"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetNewsList(Expression<Func<News, bool>> where, int topCount) {
            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(where).OrderByDescending(c =>
                c.PublishDate).Take(topCount);

            var response = new ResponseModel
            {
                code = 200,
                result = "News list query succ",
            };
            response.data = new List<NewsModel>();
            foreach (var news in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents.Length > 50 ? news.Contents.Substring(0,50) + "..." : news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark,
                    CommentCount = news.NewsComment.Count(),
                    ClassifyName = news.NewsClassify.Name,
                });
            }
            return response;
        }


        /// <summary>
        /// Get latest commented news list
        /// </summary>
        /// <param name="where"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetNewsCommentNewsList(Expression<Func<News, bool>> where, int topCount) {
            // key对应着newsId的value和key
            var newsIds = _db.NewsComment.OrderByDescending(c => c.AddTime).GroupBy(c => c.NewsId).Select(c => c.Key).Take(topCount);

            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(c => newsIds.Contains(c.Id)).Where(where).OrderByDescending(c =>
                c.PublishDate);

            var response = new ResponseModel
            {
                code = 200,
                result = "News list query succ",
            };
            response.data = new List<NewsModel>();
            foreach (var news in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents.Length > 50 ? news.Contents.Substring(0, 50) + "..." : news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark,
                    CommentCount = news.NewsComment.Count(),
                    ClassifyName = news.NewsClassify.Name,
                });
            }
            return response;
        }

        /// <summary>
        /// Search One News
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetSearchOneNews(Expression<Func<News, bool>> where)
        {
            var news = _db.News.Where(where).FirstOrDefault();
            if (news == null)
                return new ResponseModel { code = 0, result = "No news has been found" };
            return new ResponseModel { code = 200, result = "Search news succ!", data = news.Id };
        }


        /// <summary>
        /// Get counts for news
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetNewsCount(Expression<Func<News, bool>> where)
        {
            var count = _db.News.Where(where).Count();
            return new ResponseModel { code = 200, result = "Get news count succ", data = count };
        }

        /// <summary>
        /// Get recommend news by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel GetRecommendNewsList (int newsId)
        {
            var n = _db.News.FirstOrDefault(c => c.Id == newsId);
            if (n == null)
                return new ResponseModel { code = 0, result = "News does not exist." };
            var newsList = _db.News.Include("NewsComment").Where(c => c.NewsClassifyId == n.NewsClassifyId && c.Id != newsId)
                .OrderByDescending(c => c.PublishDate).OrderByDescending(c => c.NewsComment.Count).Take(6).ToList();

            var response = new ResponseModel
            {
                code = 200,
                result = "Recommend News list query succ",
            };
            response.data = new List<NewsModel>();
            foreach (var news in newsList)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents.Length > 50 ? news.Contents.Substring(0, 50) + "..." : news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark,
                    CommentCount = news.NewsComment.Count(),
                    ClassifyName = news.NewsClassify.Name,
                });
            }
            return response;
        }
    }
}
