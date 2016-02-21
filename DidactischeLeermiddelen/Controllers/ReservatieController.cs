using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Controllers
{
    public class ReservatieController : Controller
    {
        // GET: Reservatie
        public ActionResult Index(IGebruiker gebruiker)
        {
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeReservatielijst");

            IEnumerable<Reservatie> reservatielijst = gebruiker.Reservaties;
            IList<Materiaal> materiaallijst = new List<Materiaal>();

            foreach (Materiaal m in reservatielijst.Select(r => r.Materiaal))
            {
                materiaallijst.Add(m);
            }

            ReservatieMaterialenViewModel vm = ViewModelFactory.CreateViewModel("ReservatieMaterialenViewModel", null, null, materiaallijst,gebruiker) as ReservatieMaterialenViewModel;

            return View(vm);
        }
    }
}