using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventsPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace EventsPlanner.Controllers
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        public void MyEventsTest()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var u = db.Users.FirstOrDefault(f => f.UserName == "7775494@mail.ru");
            Assert.AreEqual(u.UserName, "7775494@mail.ru");

        }
    }
}