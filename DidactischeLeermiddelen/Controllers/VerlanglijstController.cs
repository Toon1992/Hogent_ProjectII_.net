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
        public ActionResult Index(Verlanglijst verlanglijst)
        {
            if (verlanglijst.Materialen.Count == 0)
                return PartialView("LegeVerlanglijst");
            VerlanglijstMaterialenViewModel vm = new VerlanglijstMaterialenViewModel()
            {

                Materialen = verlanglijst.Materialen.Distinct().Select(b => new VerlanglijstViewModel(b))
            };
            return View(vm.Materialen);
        }
    }
}