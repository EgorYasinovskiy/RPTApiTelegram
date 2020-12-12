﻿using NUnit.Framework;
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
            File.Delete(cfg.DataBaseFileName);
        }

        [Test]
        public void Вb_Exists_False()
        {
            bool expected = false;
            bool actual = File.Exists(cfg.DataBaseFileName);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Db_Created_True()
        {
            RPTApi.DataBase.BotDataContext context = new RPTApi.DataBase.BotDataContext(cfg);
            context.Dispose();
            bool expected = true;
            bool actual = File.Exists(cfg.DataBaseFileName);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Db_ReturnUser_NotNull()
        {
            RPTApi.DataBase.BotDataContext context = new RPTApi.DataBase.BotDataContext(cfg);
            context.Users.Add(new RPTApi.DataBase.Models.BotUser() { Id=1234});
            context.SaveChanges();
            context.Dispose();
            context = new RPTApi.DataBase.BotDataContext(cfg);
            var model = context.Users.FirstOrDefault();
            Assert.IsNotNull(model);
        }
        [Test]
        public void Db_ForeighnKeyGenerated_NotNull()
        {

            RPTApi.DataBase.BotDataContext context = new RPTApi.DataBase.BotDataContext(cfg);
            context.Users.Add(new RPTApi.DataBase.Models.BotUser { Id = 5050 });
            context.Orders.Add(new RPTApi.DataBase.Models.Order
            {
                Barcode = "123",
                LastQuerry = DateTime.Now,
                Name = "Name",
                StartTracking = DateTime.Now,
                UserId = 5050
            });
            context.SaveChanges();
            for(int i=0;i<10;i++)
            {
                context.Records.Add(new RPTApi.DataBase.Models.Record { DateTime = DateTime.Now.AddDays(i), Location = "SomeLocation", OrderBarcode = "123" });
            }
            context.SaveChanges();
            var recs = context.Orders.First().Records;
            Assert.IsNotNull(recs);
            Assert.IsTrue(recs.Count > 0);
        }
    }
}
