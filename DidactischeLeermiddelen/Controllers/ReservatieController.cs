using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
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
        public ActionResult MaakReservatie(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week)
        {
            IList<Materiaal> materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();

            if (materialen != null)
            {
                try
                {
                    gebruiker.VoegReservatieToe(materialen, aantal, week);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Reservatie werd aangemaakt";

                    //System.Net.Mail.MailMessage m =new System.Net.Mail.MailMessage("projecten2groep6@gmail.com","projecten2groep6@gmail.com"); // hier nog gebruiker email pakken, nu testen of het werkt
                    //m.Subject = "Bevestiging reservatie";
                    //m.Body = string.Format("Dear {0} <br/>" +
                    //                       "Bedankt voor je bestelling van volgende materialen" + 
                    //                       "<p>{1}</p>",gebruiker.Email,materialen);
                    //m.IsBodyHtml = true;

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

            return View("LegeReservatieLijst");
        }
    }
}