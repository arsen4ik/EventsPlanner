using EventsPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace EventsPlanner.Controllers
{
    public class UserController : Controller
    {
        public ActionResult newController()
        {

            return View();
        }
        
        public ActionResult Index(int? page)
        {
            UserManager<ApplicationUser> dbUsers = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationDbContext db = new ApplicationDbContext();
            var list = db.Events.Where(w => w.eventDate >= DateTime.Now).OrderBy(o => o.eventDate).ToList();
            if (User.Identity.IsAuthenticated)
            {
                var u = db.Users.FirstOrDefault(f => f.UserName == User.Identity.Name);
                list.ForEach(f => { f.isSubscribed = u.EventsSubscribed.Contains(f); });
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        public ActionResult MyEvents(int? page)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var u = db.Users.FirstOrDefault(f => f.UserName == User.Identity.Name);

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(u.EventsCreated.OrderByDescending(o => o.createdDate).ToPagedList(pageNumber, pageSize));

        }

        [Authorize]
        public ActionResult SubscribedEvents(int? page)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var u = db.Users.FirstOrDefault(f => f.UserName == User.Identity.Name);

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(u.EventsSubscribed.OrderByDescending(o => o.eventDate).ToPagedList(pageNumber, pageSize));

        }


        [Authorize]
        public ActionResult CreateEvent()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateEvent(CreateEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                if( model.Fields.Count >0 
                    && model.Fields.FirstOrDefault().Description== null
                    && model.Fields.FirstOrDefault().Name == null)
                {
                    ModelState.AddModelError("", "Additional fields should not be null.");
                    return View(model);
                }
                ApplicationDbContext db = new ApplicationDbContext();
                var u = db.Users.FirstOrDefault(f => f.UserName == User.Identity.Name);
                if (u == null)
                    return View();
                var ev = db.Events.Add(new Event
                {
                    name = model.EventName,
                    eventDate = model.EventDate,
                    createdDate = DateTime.Now,
                    maxSubscribedUsers = model.MaxSubscribedUsers,
                    EventFields = model.Fields
                        .Where(w => w.Name != null && w.Description != null)
                        .Select(s => new EventField { name = s.Name, value = s.Description }).ToList()
                });
                if (ev == null)
                    return View();

                u.EventsCreated.Add(ev);
                db.SaveChanges();
                return RedirectToAction("MyEvents", "User");

            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SubscribeToEvent(int eventId)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var u = db.Users.FirstOrDefault(f => f.UserName == User.Identity.Name);
            if (u == null)
                return new HttpStatusCodeResult(400, "User not exist.");

            var ev = db.Events.FirstOrDefault(f => f.id == eventId);
            if (ev == null)
                return new HttpStatusCodeResult(400, "Event not exist.");

            bool _subscribed = false;
            if (ev.SubscribedUsers.Count < ev.maxSubscribedUsers)
            {
                if ((_subscribed = !u.EventsSubscribed.Contains(ev)))
                    u.EventsSubscribed.Add(ev);
                else
                    u.EventsSubscribed.Remove(ev);
                await db.SaveChangesAsync();
            }
            return Json(new { subscribed = _subscribed });
        }



    }
}