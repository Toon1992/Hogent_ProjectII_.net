using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController() { }
        public ActionResult Index()
        {
            return View();
        }

        public string GetType(Gebruiker gebruiker)
        {
            string type = gebruiker.GetType().BaseType.Name;
            return type;
        }
    }
}