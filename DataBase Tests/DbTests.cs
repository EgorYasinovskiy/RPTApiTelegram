using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace DataBase_Tests
{
    class DbTests
    {
        private RPTApi.Helpers.Config cfg;
        [SetUp]
        public void Setup()
        {
            cfg = new RPTApi.Helpers.Config() { DataBaseFileName = "botDataBase.db" };
            //File.Delete(cfg.DataBaseFileName);
        }

        [Test]
        public void DB_EXISTS_FALSE_FALSE()
        {
            bool expected = false;
            bool actual = File.Exists(cfg.DataBaseFileName);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void DB_CREATED_TRUE_TRUE()
        {
            RPTApi.DataBase.BotDataContext context = new RPTApi.DataBase.BotDataContext(cfg);
            context.Dispose();
            bool expected = true;
            bool actual = File.Exists(cfg.DataBaseFileName);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void DB_RETURNS_DATES_TRUE()
        {
            var dt = DateTime.Now;
            RPTApi.DataBase.BotDataContext context = new RPTApi.DataBase.BotDataContext(cfg);
            context.Users.Add(new RPTApi.DataBase.Models.BotUser() { Id=1234,LastQuerry=dt, Orders = new System.Collections.Generic.List<RPTApi.DataBase.Models.Order>()});
            context.SaveChanges();
            context.Dispose();
            context = new RPTApi.DataBase.BotDataContext(cfg);
            var model = context.Users.FirstOrDefault();
            bool expected = true;
            bool actual = dt == model.LastQuerry;
            Assert.AreEqual(expected, actual);
        }
    }
}
