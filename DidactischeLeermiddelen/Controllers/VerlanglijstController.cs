using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Controllers
{
    public class VerlanglijstController : Controller
    {
        // GET: Verlanglijst
        public ActionResult Index(Gebruiker gebruiker)
        {
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeVerlanglijst");
            VerlanglijstMaterialenViewModel vm = new VerlanglijstMaterialenViewModel()
            {
                Materialen = gebruiker.Verlanglijst.Materialen.Select(b => new VerlanglijstViewModel(b))
            };
            return View(vm.Materialen);
        }
    }
}