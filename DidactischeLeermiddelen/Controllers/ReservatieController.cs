using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Controllers
{
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

            ReservatieMaterialenViewModel vm = ViewModelFactory.CreateViewModel("ReservatieMaterialenViewModel", null, null, materiaallijst,gebruiker) as ReservatieMaterialenViewModel;

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
                    gebruiker.VoegReservatieToe(materialen, aantal, week);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Reservatie werd aangemaakt";

                    VerzendMailNaReservatie(gebruiker, materialen);

                    //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    //smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
                    //smtp.EnableSsl = true;
                    //smtp.Send(m);
              
                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
        }

        private void VerzendMailNaReservatie(Gebruiker gebruiker, IList<Materiaal> materialen)
        {

            // ook nog datum erbij pakken tot wanneer uitgeleend
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", "projecten2groep6@gmail.com");// hier nog gebruiker email pakken, nu testen of het werkt
                
            m.Subject = "Bevestiging reservatie";
            m.Body = string.Format("Dag {0} <br/>", gebruiker.Naam);
            m.IsBodyHtml = true;
            m.Body += "<p>Dit zijn je reservaties: </p>";
            m.Body += "<ul>";
            foreach (var item in materialen)
            {
                m.Body += $"<li>{item.Naam}</li>";
            }

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}