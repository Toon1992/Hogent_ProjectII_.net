using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Controllers
{
    [CustomAuthorize]
    public class ReservatieController : Controller
    {
        // GET: Reservatie
        private IMateriaalRepository materiaalRepository;
        private IGebruikerRepository gebruikerRepository;

        public ReservatieController(IMateriaalRepository materiaalRepository, IGebruikerRepository gebruikerRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.gebruikerRepository = gebruikerRepository;
        }

        public ActionResult Index(Gebruiker gebruiker)
        {
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeReservatielijst");

            IEnumerable<Reservatie> reservatielijst = gebruiker.Reservaties;
            IList<Materiaal> materiaallijst = new List<Materiaal>();

            foreach (Materiaal m in reservatielijst.Select(r => r.Materiaal))
            {
                materiaallijst.Add(m);
            }
            ViewBag.Gebruikersnaam = gebruiker.Naam;
            ViewBag.AantalReservaties = reservatielijst.Count();

            ReservatieMaterialenViewModel vm = ViewModelFactory.CreateViewModel("ReservatieMaterialenViewModel", null, null, null,gebruiker) as ReservatieMaterialenViewModel;

            return View(vm);
        }

        public ActionResult MaakReservatie()
        {
            return View("Index");
        }

        [HttpPost]
        public void MaakReservatie(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week)
        {
            IList<Materiaal> materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();

            if (materialen != null)
            {
                try
                {
                    gebruiker.VoegReservatieToe(materialen, aantal, week,gebruiker);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Reservatie werd aangemaakt";
                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
        }


    }
}