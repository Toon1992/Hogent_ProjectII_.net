﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
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
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeVerlanglijst");

            VerlanglijstMaterialenViewModel vm = ViewModelFactory.CreateViewModel("VerlanglijstMaterialenViewModel",null,null,null,gebruiker) as VerlanglijstMaterialenViewModel;
            return View(vm);
        }

        [HttpPost]
        public ActionResult VerwijderUitVerlanglijst(int id, Gebruiker gebruiker)
        {
            Materiaal materiaal = materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id);
            if (materiaal != null)
            {
                try
                {
                    gebruiker.VerwijderMateriaalUitVerlanglijst(materiaal);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Item {materiaal.Naam} werd verwijderd uit uw verlanglijst";
                    MailMessage m = new MailMessage("projecten2groep6@gmail.com", "projecten2groep6@gmail.com"); // hier nog gebruiker email pakken, nu testen of het werkt
                    m.Subject = "Bevestiging reservatie";
                    m.Body = string.Format("Dear {0} <br/>" +
                                           "Bedankt voor je bestelling van volgende materialen" + "<p>{1}</p>"
                                           , gebruiker.Email,materiaal.Naam);
                    m.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com",587);
                    smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
                    smtp.EnableSsl = true;
                    smtp.Send(m);

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
            VerlanglijstMaterialenViewModel vm;
            List<Materiaal> materiaalVerlanglijst = gebruiker.Verlanglijst.Materialen;
            List<Materiaal> materialen = new List<Materiaal>();
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            int aantalBeschikbaar;
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
                    GeselecteerdeWeek = FirstDateOfWeekISO8601(2016, week),
                    TotaalGeselecteerd = totaalGeselecteerd
                };
                return PartialView("Confirmatie", vm);
            }         
            vm = new VerlanglijstMaterialenViewModel
            {
                Materialen = materiaalVerlanglijst.Select(m => new VerlanglijstViewModel
                {
                    AantalBeschikbaar = aantalBeschikbaar = m.AantalInCatalogus - (m.Stuks.Count(s => s.StatusData.FirstOrDefault(sd => sd.Week.Equals(week)).Status.Equals(Status.Reserveerbaar))),
                    Beschikbaar = true,
                    Firma = m.Firma,
                    Foto = m.Foto,
                    AantalGeselecteerd = materiaalAantal.ContainsKey(m.MateriaalId) ? materiaalAantal[m.MateriaalId] : 0,
                    Geselecteerd = aantalBeschikbaar > 0 ? materialen.Any(k => k.MateriaalId.Equals(m.MateriaalId)) : false,
                    Leergebieden = m.Leergebieden,
                    AantalInCatalogus = m.AantalInCatalogus,
                    MateriaalId = m.MateriaalId,
                    Naam = m.Naam,
                    Omschrijving = m.Omschrijving,
                }),
                GeselecteerdeWeek = FirstDateOfWeekISO8601(2016, week),
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
                    var reservatieData = materialen[i].Stuks.Count(s => s.StatusData.FirstOrDefault(sd => sd.Week.Equals(week)).Status.Equals(Status.Reserveerbaar));
                    if (reservatieData != null)
                    {
                        aantalBeschikbaar = materialen[i].AantalInCatalogus - reservatieData;
                        if (aantalBeschikbaar == 0)
                        {
                            ModelState.AddModelError("",
                                string.Format("Materiaal {0} is niet meer beschikbaar in beschikbaar van {1} tot {2}",
                                    materialen[i].Naam, FirstDateOfWeekISO8601(2016, week).ToString("d"),
                                    FirstDateOfWeekISO8601(2016, week).AddDays(5).ToString("d")));
                        }
                        else if (aantalBeschikbaar < aantal[i])
                        {
                            ModelState.AddModelError("",
                                string.Format("Slechts {0} stuks van materiaal {1} beschikbaar", aantalBeschikbaar,
                                    materialen[i].Naam));
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
        [HttpPost]
        public void MaakReservatie(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week)
        {
            List<Materiaal> materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
            if (materialen != null)
            {
                try
                {
                    //gebruiker.VoegReservatieToe(materialen, startDatum);
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
        }
        private DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
    }
}