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

            ReservatieMaterialenViewModelFactory rvmf = new ReservatieMaterialenViewModelFactory();
            ReservatieMaterialenViewModel vm = rvmf.CreateViewModel(null, null, null, DateTime.Now, gebruiker) as ReservatieMaterialenViewModel;

            return View(vm);
        }

        public ActionResult MaakReservatie()
        {
            return View("Index");
        }

        [HttpPost]
        public void MaakReservatie(Gebruiker gebruiker, int[] materiaal, int[] aantal, string startDatum, string eindDatum)
        {
            IList<Materiaal> materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();

            if (materialen.Count > 0)
            {
                IDictionary<Materiaal, int> potentieleReservaties = new Dictionary<Materiaal, int>();
                for (int index = 0; index < materialen.Count; index++)
                {
                    potentieleReservaties.Add(materialen[index], aantal[index]);
                }

                try
                {
                    if (gebruiker is Student)
                    {
                        Student student = gebruiker as Student;

                        if (student != null)
                            student.maakReservaties(potentieleReservaties, startDatum, eindDatum);
                                             
                       
                        TempData["Info"] = $"Reservatie werd aangemaakt";
                    }
                    else
                    {
                      
                        Lector lector = gebruiker as Lector;

                        if (lector != null)
                            lector.MaakBlokkeringen(potentieleReservaties, startDatum, eindDatum);

                        TempData["Info"] = $"Reservatie werd aangemaakt";
                    }

                    gebruikerRepository.SaveChanges();
                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
        }

        [HttpPost]
        public void VerwijderReservatie(int id, Gebruiker gebruiker)
        {
            Reservatie r = reservatieRepository.FindById(id);
            try
            {
                gebruiker.VerwijderReservatie(r);
                gebruikerRepository.SaveChanges();
                TempData["Info"] = "Reservatie is succesvol verwijderd";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
        }


    }
}