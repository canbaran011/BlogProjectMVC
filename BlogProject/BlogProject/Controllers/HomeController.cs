using BlogProject.Models;
using BlogProject.Models.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }


        public PartialViewResult _NavBarPartial()
        {
            NavBarPartialView model = new NavBarPartialView();
            model.Categori = db.Categories.ToList();
            return PartialView("_NavBarPartial", model);
        }

    }
}