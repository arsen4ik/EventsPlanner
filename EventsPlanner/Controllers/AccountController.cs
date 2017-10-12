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

namespace EventsPlanner.Controllers
{
    public class AccountController : Controller
    {

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        public ActionResult Registration()
        {
            return PartialView("_PartialRegForm", new RegistrationViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserManager<ApplicationUser> dbUsers = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                var isEmailExist = await dbUsers.FindByNameAsync(model.Email);
                if (isEmailExist != null)
                {
                    ModelState.AddModelError("", "This Email already exist");
                    return PartialView("_PartialRegForm", model);
                }

                var user = new ApplicationUser() { UserName = model.Email };
                user.Email = model.Email;
                user.EmailConfirmed = false;
                
                var result = await dbUsers.CreateAsync(user);
                if (result.Succeeded)
                {
                    if (model.EventId != -1) // if Subcribe
                    {
                        ApplicationDbContext db = new ApplicationDbContext();
                        user = db.Users.FirstOrDefault(f => f.UserName == user.UserName);
                        if (user == null)
                            return PartialView("_MessageOk", "Error: User not register");

                        var ev = db.Events.FirstOrDefault(f => f.id == model.EventId);
                        if (ev == null)
                            return PartialView("_MessageOk", "Error: Not find Event");

                        user.EventsSubscribed.Add(ev);
                        await db.SaveChangesAsync();

                    }
                    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                      new System.Net.Mail.MailAddress("event_subscriber@mail.ru", "Event Subscriber"),
                      new System.Net.Mail.MailAddress(user.Email));
                    m.Subject = "E-mail confirmation";
                    m.Body = string.Format("For confirm your e-mail go to: <a href=\"{0}\" title=\"Confirm\">{0}</a>", Url.Action("ConfirmEmail", "Account", new { Token = user.Id, Email = user.Email }, Request.Url.Scheme));
                    m.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.mail.ru", 587);
                    smtp.Credentials = new System.Net.NetworkCredential("event_subscriber@mail.ru", "event1234");
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(m);
                        return PartialView("_MessageOk", "On your email send message for confirmation.");
                    }
                    catch (Exception ex)
                    {
                        result = await dbUsers.DeleteAsync(user);
                        return PartialView("_MessageOk", "Error sending message " + user.Email );
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.FirstOrDefault());
                }

            }
            return PartialView("_PartialRegForm", model);
        }



        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {

            UserManager<ApplicationUser> dbUsers = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            ApplicationUser user = dbUsers.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email && !dbUsers.HasPassword(user.Id))
                {
                    ViewBag.Token = Token;
                    if (user.EmailConfirmed == false)
                    {
                        user.EmailConfirmed = true;
                        await dbUsers.UpdateAsync(user);
                        ViewBag.EmailConfirmed = true;
                    }
                    return View(new ConfirmEmailViewModel { TokenUser = Token });
                }
            }
            return RedirectToAction("Index", "User");

        }

        [HttpPost]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (ModelState.IsValid)
            {

  
                UserManager<ApplicationUser> dbUsers = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                ApplicationUser user = await  dbUsers.FindByIdAsync(model.TokenUser);
                if (user == null || dbUsers.HasPassword(user.Id))
                {
                    ModelState.AddModelError("", "Error.");
                    return View(model);
                }
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                var result = await dbUsers.UpdateAsync(user);
                if (result.Succeeded)
                {
                    result = await dbUsers.AddPasswordAsync(user.Id, model.Password);


                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Error passowrd not set.");
                        return View(model);
                    }
                    result = await dbUsers.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Error save password.");
                        return View(model);
                    }
                    var identity = await dbUsers.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(identity);
                    return RedirectToAction("Index", "User");

                }

            }
            return View(model);

        }


        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserManager<ApplicationUser> dbUsers = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                var user = await dbUsers.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    if (user.EmailConfirmed == true)
                    {

                        var identity = await dbUsers.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(identity);
                        return Json(new { isReload = true });
                    }
                    else
                        ModelState.AddModelError("", "Confirm email.");

                }
                else
                {
                    ModelState.AddModelError("", "The login or password is wrong.");
                }
            }
            return PartialView("_PartialLoginForm", model);
        }

        [Authorize]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut();

            return Redirect(Request.UrlReferrer.ToString());

        }

        [Authorize]
        public ActionResult MyProfile()
        {
            UserManager<ApplicationUser> dbUsers = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            ApplicationUser user = dbUsers.FindByName(User.Identity.Name);

            ProfileViewModel u = new ProfileViewModel() {FirstName = user.FirstName, LastName= user.LastName};

            return View(u);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> MyProfile(ProfileViewModel model)
        {
            UserManager<ApplicationUser> dbUsers = new UserManager<Models.ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = await dbUsers.FindByNameAsync(User.Identity.Name);


            if (ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                var result = await dbUsers.UpdateAsync(user);
                if (result.Succeeded)
                {
                    ViewBag.isSaved = true;

                    if (model.OldPassword != null && model.NewPassword != null && model.ConfirmPassword != null)
                    {

                        result = await dbUsers.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return View(model);

                        }
                        else
                            ModelState.AddModelError("", "Current password is wrong.");
                    }
                }
            }
            return View(model);
        }

    }
}