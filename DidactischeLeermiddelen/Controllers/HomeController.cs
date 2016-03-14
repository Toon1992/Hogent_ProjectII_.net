using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IMateriaalRepository materiaalRepository;
        public HomeController() { }
        public HomeController(IMateriaalRepository materiaalRepository)
        {
            this.materiaalRepository = materiaalRepository;
        }
        public ActionResult Index()
        {
            List<Materiaal> materialen = materiaalRepository.FindAll().ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Website Didactische Leermiddelen";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}