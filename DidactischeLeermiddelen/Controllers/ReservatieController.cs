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
        public ActionResult Index(Gebruiker gebruiker)
        {
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeReservatielijst");
            IEnumerable<Reservatie> reservatielijst = gebruiker.Reservaties;
            IList<Materiaal> materriaallijst = new List<Materiaal>();
            foreach (Materiaal m in reservatielijst.SelectMany(r => r.Materialen))
            {
                materriaallijst.Add(m);
            }
            ReservatieMaterialenViewModel vm = ViewModelFactory.CreateViewModel("ReservatieMaterialenViewModel", null, null, materriaallijst, gebruiker) as ReservatieMaterialenViewModel;
            return View(vm);
        }
    }
}