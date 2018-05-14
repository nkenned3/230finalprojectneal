using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NealsLearningCenter.Models;

namespace NealsLearningCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager userManager;

        public HomeController()
        {
            this.userManager = new UserManager();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ClassList()
        {
            var entities = new Entities1();
            entities.Database.Connection.Open();

            var classes = new List<Class>();
            foreach (var cl in entities.Classes)
            {
                classes.Add(cl);
            }
            return View(classes);
        }
        [Authorize]
        //authorize tag seems to not work if there are old cookies from local host
        public ActionResult StudentClasses()
        {
            var retrieved = new List<Class>();
            var entities = new Entities1();
            entities.Database.Connection.Open();

            if (Session["User"] == null)
            {
                //Authorize attribute should prevent this, but sometimes does not due to old cookies
                return Redirect("~/Error");
            }
            var loggedInUser = (User)Session["User"];
            var user = entities.Users.FirstOrDefault(t => t.UserEmail.ToLower() == loggedInUser.UserEmail.ToLower()
                                      && t.UserPassword == loggedInUser.UserPassword);
            foreach(var cls in user.Classes)
            {
                retrieved.Add(cls);
            }
            return View(retrieved);
        }
        [Authorize]
        //authorize tag seems to not work if there are old cookies from local host
        public ActionResult EnrollInClasses()
        {
            if (Session["User"] == null)
            {
                //Authorize attribute should prevent this, but sometimes does not due to old cookies
                return Redirect("~/Error");
            }
            var entities = new Entities1();
            entities.Database.Connection.Open();

            var classes = new List<SelectListItem>();
            foreach (var cl in entities.Classes)
            {
                classes.Add(new SelectListItem { Text = cl.ClassDescription, Value = cl.ClassId.ToString()});
            }
            return View(classes);
        }
        [HttpPost]
        public ActionResult EnrollInClasses(List<SelectListItem> list)
        {
            string value = Request.Form["EnrollInClasses"].ToString();
            var entities = new Entities1();
            entities.Database.Connection.Open();
            var loggedInUser = (User)Session["User"];
            var user = entities.Users.FirstOrDefault(t => t.UserEmail.ToLower() == loggedInUser.UserEmail.ToLower()
                                      && t.UserPassword == loggedInUser.UserPassword);
            var val = Int32.Parse(value);
            var cls = entities.Classes.FirstOrDefault(t => t.ClassId == val);
            user.Classes.Add(cls);
            entities.SaveChanges();
            return View();
        }
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(LoginModel loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = userManager.Login(loginModel.UserName, loginModel.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "User name and password do not match.");
                }
                else
                {
                    //lets us track any info about the user
                    Session["User"] = user;
                    //sets the cookie so theyre logged in
                    System.Web.Security.FormsAuthentication.SetAuthCookie(loginModel.UserName, false);

                    return Redirect(returnUrl ?? "~/");
                }
            }
            return View(loginModel);
        }

        public ActionResult LogOff()
        {
            Session["User"] = null;
            System.Web.Security.FormsAuthentication.SignOut();

            return Redirect("~/");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel registerModel, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                var user = userManager.Register(registerModel.Email, registerModel.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Account not registered.");
                }
                else
                {
                    //lets us track any info about the user
                    Session["User"] = user;
                    //sets the cookie so theyre logged in
                    System.Web.Security.FormsAuthentication.SetAuthCookie(registerModel.Email, false);
                    //Redirect("~/");
                    return Redirect(returnUrl ?? "~/");
                }
            }
            ModelState.AddModelError("", "Account not registered.");
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}