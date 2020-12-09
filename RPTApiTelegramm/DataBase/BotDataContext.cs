using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPTApi.DataBase
{
    public class BotDataContext : DbContext
    {
        private string dbFileName;

        public DbSet<Models.BotUser> Users { get; set; }
        public DbSet<Models.Order> Orders { get; set; }
        public DbSet<Models.Record> Records { get; set; }
        public BotDataContext(Helpers.Config config)
        {
            dbFileName = config.DataBaseFileName;
            Database.EnsureCreated();
        }
        public BotDataContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={dbFileName ?? "botData.db"}");
        }
    }
}
