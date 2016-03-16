using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.InterfaceRepositories;
using DidactischeLeermiddelen.Models.Domain.Mail;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Controllers
{
    [Authorize]
    public class ReservatieController : Controller
    {
        // GET: Reservatie
        private IMateriaalRepository materiaalRepository;
        private IGebruikerRepository gebruikerRepository;
        private IReservatieRepository reservatieRepository;
        private IMailServiceRepository mailServiceRepository;

        public ReservatieController(IMateriaalRepository materiaalRepository, IGebruikerRepository gebruikerRepository, IReservatieRepository reservatieRepository, IMailServiceRepository mailServiceRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.gebruikerRepository = gebruikerRepository;
            this.reservatieRepository = reservatieRepository;
            this.mailServiceRepository = mailServiceRepository;
        }

        public ActionResult Index(Gebruiker gebruiker)
        {
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeReservatielijst");

            ICollection<Reservatie> reservatielijst = gebruiker.Reservaties;

            //IList<Materiaal> materiaallijst = VulMateriaalLijstIn(reservatielijst); 

            ViewBag.Gebruikersnaam = gebruiker.Naam;
            ViewBag.AantalReservaties = reservatielijst.Count();

            ReservatieMaterialenViewModelFactory rvmf = new ReservatieMaterialenViewModelFactory();
            ReservatieMaterialenViewModel vm = rvmf.CreateReservatieMaterialenViewModel(gebruiker) as ReservatieMaterialenViewModel;

            return View(vm);
        }

        public ActionResult MaakReservatie()
        {
            return View("Index");
        }

        [HttpPost]
        public void MaakReservatie(Gebruiker gebruiker, int[] materiaal, int[] aantal, string startDatum, string[] dagen)
        {
            IList<Materiaal> materialen = GeefMaterialenVanId(materiaal);

            string eersteDag = HulpMethode.GetStartDatum(startDatum).ToShortDateString();

            if (materialen.Count > 0)
            {
                IDictionary<Materiaal, int> potentieleReservaties = InvullenVanMapMetPotentieleReservaties(aantal, materialen);

                try
                {
                    if (gebruiker is Student)
                    {
                        Student student = gebruiker as Student;

                        student.MaakReservaties(potentieleReservaties, eersteDag);
                        VerstuurMailStudent(potentieleReservaties, eersteDag, student);

                        TempData["Info"] = $"Reservatie werd aangemaakt";
                    }
                    else
                    {
                        Lector lector = gebruiker as Lector;

                        lector.MaakBlokkeringen(potentieleReservaties, eersteDag,dagen);

                        List<Reservatie> materialenOverruled=new List<Reservatie>();
                        //materialenOverruled =
                        //    potentieleReservaties.Keys.Where(
                        //        m => m.Reservaties.Where(p => p.ReservatieStateEnum == ReservatieStateEnum.Overruled)).ToList();
                        VerstuurMailBlokkeringLector(potentieleReservaties, dagen, lector);
                        //VerstuurMailBlokkeringStudent(null,eersteDag,null);
                        TempData["Info"] = $"Blokkering werd aangemaakt";
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
        public ActionResult VerwijderReservatie(int id, Gebruiker gebruiker)
        {
            Reservatie r = reservatieRepository.FindById(id);
            try
            {
                gebruiker.VerwijderReservatie(r);
                reservatieRepository.Remove(r);
                reservatieRepository.SaveChanges();
                gebruikerRepository.SaveChanges();
                
                TempData["Info"] = (gebruiker is Lector ? "Reservatie" : "Blokkering") + "is succesvol verwijderd";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private IList<Materiaal> GeefMaterialenVanId(int[] materiaal)
        {
            return materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
        }

        private IDictionary<Materiaal, int> InvullenVanMapMetPotentieleReservaties(int[] aantal, IList<Materiaal> materialen)
        {
            IDictionary<Materiaal, int> potentieleReservaties = new Dictionary<Materiaal, int>();

            for (int index = 0; index < materialen.Count; index++)
            {
                potentieleReservaties.Add(materialen[index], aantal[index]);
            }

            return potentieleReservaties;
        }

        private void VerstuurMailStudent(IDictionary<Materiaal, int> potentieleReservaties, string startDatum, Gebruiker gebruiker)
        {
            MailTemplate mail = mailServiceRepository.GeefMailTemplate("Bevestiging reservatie");
            mail.VerzendMail(potentieleReservaties, startDatum, HulpMethode.GetEindDatum(startDatum).ToShortDateString(),null, gebruiker);
        }

        private void VerstuurMailBlokkeringLector(IDictionary<Materiaal, int> blokkeringen, string[] dagen,
            Gebruiker gebruiker)
        {
            MailTemplate mail = mailServiceRepository.GeefMailTemplate("Blokkering");
            //mail.VerzendMail(blokkeringen,"","",dagen,gebruiker);

        }

        private void VerstuurMailBlokkeringStudent(IDictionary<Materiaal, int> blokkeringen, string startDatum,
            Gebruiker gebruiker)
        {
            MailTemplate mail = mailServiceRepository.GeefMailTemplate("Reservatie gewijzigd");
            mail.VerzendMail(blokkeringen,startDatum,"",null,gebruiker);
        }     

        //private IList<Materiaal> VulMateriaalLijstIn(ICollection<Reservatie> reservatielijst)
        //{
        //    IList<Materiaal> materiaallijst = new List<Materiaal>();

        //    foreach (Materiaal materiaal in reservatielijst.Select(r => r.Materiaal))
        //    {
        //        materiaallijst.Add(materiaal);
        //    }

        //    return materiaallijst;
        //}
    }
}