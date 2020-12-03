﻿using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewsPublish.Service
{
    /// <summary>
    /// Banner Services
    /// </summary>
    public class BannerService
    {
        private Db _db;
        public BannerService(Db db)
        {
            this._db = db;
        }
        /// <summary>
        /// Add Banner
        /// </summary>
        /// <param name="banner"></param>
        /// <returns></returns>
        public ResponseModel AddBanner(AddBanner banner)
        {
            var ba = new Banner { AddTime = DateTime.Now, Image = banner.Image, Url = banner.Url, Remark = banner.Remark };
            _db.Banner.Add(ba);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Banner add succ!" };
            return new ResponseModel { code = 0, result = "Banner add fail!" };
        }

        /// <summary>
        /// Get Banner Collection
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetBannerList()
        {
            var banners = _db.Banner.ToList().OrderByDescending(c => c.AddTime);
            var response = new ResponseModel();
            response.code = 200;
            response.result = "Banner collection get succ!";
            response.data = new List<BannerModel>();
            foreach (var banner in banners)
            {
                response.data.Add(new BannerModel { 
                    Id = banner.Id,
                    Image = banner.Image,
                    Url = banner.Url,
                    Remark = banner.Remark
                });
            }
            return response;
        }

        /// <summary>
        /// Delete Banner
        /// </summary>
        /// <param name="bannerId"></param>
        /// <returns></returns>
        public ResponseModel DeleteBanner(int bannerId)
        {
            var banner = _db.Banner.Find(bannerId);
            if (banner==null)
            {
                return new ResponseModel { code = 0, result = "Banner does not exist!" };
            }
            _db.Banner.Remove(banner);
            int i = _db.SaveChanges();
            if (i > 0)
                return new ResponseModel { code = 200, result = "Banner delete succ!" };
            return new ResponseModel { code = 0, result = "Banner delete fail!" };
        }
    }
}

