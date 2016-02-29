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
using DidactischeLeermiddelen.Models.DAL;
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
        private IReservatieRepository reservatieRepository;

        public ReservatieController(IMateriaalRepository materiaalRepository, IGebruikerRepository gebruikerRepository, IReservatieRepository reservatieRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.gebruikerRepository = gebruikerRepository;
            this.reservatieRepository = reservatieRepository;
        }

        public ActionResult Index(Gebruiker gebruiker)
        {
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeReservatielijst");

            ICollection<Reservatie> reservatielijst = gebruiker.Reservaties;
            IList<Materiaal> materiaallijst = new List<Materiaal>();

            foreach (Materiaal materiaal in reservatielijst.Select(r => r.Materiaal))
            {
                materiaallijst.Add(materiaal);
            }
            ViewBag.Gebruikersnaam = gebruiker.Naam;
            ViewBag.AantalReservaties = reservatielijst.Count();

            ReservatieMaterialenViewModel vm = ViewModelFactory.CreateViewModel("ReservatieMaterialenViewModel", null, null, null,DateTime.Now,gebruiker) as ReservatieMaterialenViewModel;

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

            IList<Reservatie> reservaties = reservatieRepository.FindAll().ToList();

            if (materialen != null)
            {
                try
                {
                    gebruiker.VoegReservatieToe(materialen, aantal, week);
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