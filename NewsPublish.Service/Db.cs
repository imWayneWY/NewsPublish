using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using NewsPublish.Model.Entity;

namespace NewsPublish.Service
{
    /// <summary>
    /// 数据库访问上下文
    /// </summary>
    public class Db : DbContext
    {
        public Db() { }
        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // For my windows env
            //optionsBuilder.UseSqlServer("Data source=LAPTOP-IS0OAAEB;Initial Catalog=NewsPublish;User ID=weiyan;Password=weiyan", b => b.UseRowNumberForPaging());
            // For my mac env
            optionsBuilder.UseSqlServer("Data source=127.0.0.1,1433;Initial Catalog=NewsPublish;User Id = SA;Password =<YourStrong@Passw0rd>", b => b.UseRowNumberForPaging());


            /*
             run this on mac:
            sudo docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YourStrong@Passw0rd>" \
   -p 1433:1433 --name sql1 \
   -d mcr.microsoft.com/mssql/server:2017-latest
             */
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public virtual DbSet<Banner> Banner { get; set; }
        public virtual DbSet<NewsClassify> NewsClassify { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsComment> NewsComment { get; set; }
    }
}
