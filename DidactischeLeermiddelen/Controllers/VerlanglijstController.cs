using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.ViewModels;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Controllers
{
    [Authorize]
    public class VerlanglijstController : Controller
    {
        private IMateriaalRepository materiaalRepository;
        private IGebruikerRepository gebruikerRepository;

        public VerlanglijstController(IMateriaalRepository materiaalRepository, IGebruikerRepository gebruikerRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.gebruikerRepository = gebruikerRepository;
        }

        // GET: Verlanglijst
        public ActionResult Index(Gebruiker gebruiker)
        {
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeVerlanglijst");

            VerlanglijstMaterialenViewModel vm = ViewModelFactory.CreateViewModel("VerlanglijstMaterialenViewModel",null,null,null,gebruiker) as VerlanglijstMaterialenViewModel;
            vm.GeselecteerdeWeek = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, (HulpMethode.GetIso8601WeekOfYear(DateTime.Now)+1)%53).ToString("d",dtfi);
            return View(vm);
        }

        [HttpPost]
        public ActionResult VerwijderUitVerlanglijst(int id, Gebruiker gebruiker)
        {
            Materiaal materiaal = materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id);
            List<String> lijst=new List<String>() { "bla", "sdsfsfd", "ésdsdsf" };

            if (materiaal != null)
            {
                try
                {
                    gebruiker.VerwijderMateriaalUitVerlanglijst(materiaal);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Item {materiaal.Naam} werd verwijderd uit uw verlanglijst";

                    ////StreamReader reader = new StreamReader(Server.MapPath("~/Views/EmailReservatie.html"));
                    //MailMessage m = new MailMessage("projecten2groep6@gmail.com", "projecten2groep6@gmail.com"); // hier nog gebruiker email pakken, nu testen of het werkt
                    //m.Subject = "Bevestiging reservatie";

                    //m.IsBodyHtml = true;
                    //m.Body += "<p>Dit zijn je reservaties: </p>";
                    //m.Body += "<ul>";
                    //foreach (var item in lijst)
                    //{
                    //    m.Body += $"<li>{item}</li>";
                    //}
                    //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    //smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
                    //smtp.EnableSsl = true;
                    //smtp.Send(m);

                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation(
                                  "Class: {0}, Property: {1}, Error: {2}",
                                  validationErrors.Entry.Entity.GetType().FullName,
                                  validationError.PropertyName,
                                  validationError.ErrorMessage);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Controle(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week, bool knop)
        {
            //Variabelen
            VerlanglijstMaterialenViewModel vm = null;
            List<Materiaal> materiaalVerlanglijst = gebruiker.Verlanglijst.Materialen;
            List<Materiaal> materialen = new List<Materiaal>();
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
            int aantalBeschikbaar, aantalGeselecteerd = 0;
            int totaalGeselecteerd = 0;

            if (materiaal != null)
            {
                materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
                //Map maken die de geselecteerde materialen met hun aantallen verbind.

                for (int i = 0; i < materiaal.Length; i++)
                {
                    materiaalAantal.Add(materiaal[i], aantal[i]);
                    totaalGeselecteerd += aantal[i];
                }
            }
            //Wanneer op "Ga naar reservatie werd geklikt wordt eerst gekeken of de gekozen materialen met voldoende
            //aantal stuks beschikbaar zijn, zoniet wordt het verlanglijstscherm terug getoont.
            bool allesBeschikbaar = ControleSelecteerdMateriaal(gebruiker, materiaal, aantal, week);
            if (knop && allesBeschikbaar)
            {
                vm = new VerlanglijstMaterialenViewModel
                {
                    Materialen = materialen.Select(m => new VerlanglijstViewModel
                    {
                        AantalGeselecteerd = materiaalAantal[m.MateriaalId],
                        Naam = m.Naam,                  
                    }),
                    GeselecteerdeWeek = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d",dtfi),
                    TotaalGeselecteerd = totaalGeselecteerd
                };
                return PartialView("Confirmatie", vm);
            }         
            vm = new VerlanglijstMaterialenViewModel
            {
                Materialen = materiaalVerlanglijst.Select(m => new VerlanglijstViewModel
                {
                    AantalBeschikbaar = aantalBeschikbaar = m.AantalInCatalogus - (m.Stuks.Count(s => s.StatusData.FirstOrDefault(sd => sd.Week.Equals(week)).Status.Equals(Status.Gereserveerd))),
                    Beschikbaar = aantalBeschikbaar == 0,
                   
                    Firma = m.Firma,
                    Foto = m.Foto,
                    AantalGeselecteerd = aantalGeselecteerd = materiaalAantal.ContainsKey(m.MateriaalId) ? materiaalAantal[m.MateriaalId] : (aantalGeselecteerd == 0 ? aantalGeselecteerd == aantalBeschikbaar? 0 : 1: aantalGeselecteerd > aantalBeschikbaar ? aantalBeschikbaar : aantalGeselecteerd),
                    Geselecteerd = aantalBeschikbaar > 0 ? materialen.Any(k => k.MateriaalId.Equals(m.MateriaalId)) : false,
                    Leergebieden = m.Leergebieden as List<Leergebied>,
                    AantalInCatalogus = m.AantalInCatalogus,
                    MateriaalId = m.MateriaalId,
                    Beschikbaarheid = aantalBeschikbaar == 0 ? 
                                    string.Format("Niet meer beschikbaar van {0} tot {1}",HulpMethode.FirstDateOfWeekISO8601(2016, week).ToString("d"),HulpMethode.FirstDateOfWeekISO8601(2016, week).AddDays(5).ToString("d")) :
                                    aantalBeschikbaar < aantalGeselecteerd ? string.Format("Slechts {0} stuks beschikbaar", aantalBeschikbaar ): "",
                    Naam = m.Naam,
                    Omschrijving = m.Omschrijving,
                }),
                GeselecteerdeWeek = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d",dtfi),
            };
            return PartialView("Verlanglijst", vm);
        }

        private bool ControleSelecteerdMateriaal(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week)
        {
            //Variabelen
            List<Materiaal> materialen = new List<Materiaal>();
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            int aantalBeschikbaar = 0;

            if (materiaal != null)
            {
                materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
                //Map maken die de geselecteerde materialen met hun aantallen verbind.

                for (int i = 0; i < materiaal.Length; i++)
                {
                    materiaalAantal.Add(materiaal[i], aantal[i]);
                }
            }
            //Melding geven indien niet alle gewenste materialen beschikbaar zijn.
            if (materiaal != null)
            {
                for (int i = 0; i < materiaal.Length; i++)
                {
                    //Kijken of er voor de opgegeven week al reservatiedata beschikbaar is voor het geselecteerde materiaal
                    var reservatieData = materialen[i].Stuks.Count(s => s.StatusData.FirstOrDefault(sd => sd.Week.Equals(week)).Status.Equals(Status.Gereserveerd));
                    if (reservatieData != null)
                    {
                        aantalBeschikbaar = materialen[i].AantalInCatalogus - reservatieData;
                        if (aantalBeschikbaar == 0)
                        {
                            ModelState.AddModelError("","error");
                        }
                        else if (aantalBeschikbaar < aantal[i])
                        {
                            ModelState.AddModelError("","error");
                        }
                    }
                }
            }
            if (ModelState.IsValid)
            {
                return true;
            }
            return false;
        }
       
    }
}