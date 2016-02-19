using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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

        public ActionResult Confirmatie(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week)
        {
            List<Materiaal> materiaalVerlanglijst = gebruiker.Verlanglijst.Materialen;
            List<Materiaal> materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
            int aantalBeschikbaar = 0;
            //VerlanglijstMaterialenViewModel vm = ViewModelFactory.CreateViewModel("VerlanglijstMaterialenViewModel",null, null, materiaalVerlanglijst, gebruiker) as VerlanglijstMaterialenViewModel;
            VerlanglijstMaterialenViewModel vm = new VerlanglijstMaterialenViewModel
            {
                Materialen = materiaalVerlanglijst.Select(m => new VerlanglijstViewModel
                {
                    AantalBeschikbaar = aantalBeschikbaar = m.AantalInCatalogus - m.ReservatieData.FirstOrDefault(r => r.Week.Equals(week)).Aantal,         
                    Beschikbaar = true,
                    Firma = m.Firma,
                    Foto = m.Foto,
                    //AANTALGESELECTEERD IMPLEMENTERN
                    Geselecteerd =  aantalBeschikbaar > 0? materialen.Any(k => k.MateriaalId.Equals(m.MateriaalId)):false,
                    Leergebieden = m.Leergebieden,
                    AantalInCatalogus = 1,
                    MateriaalId = m.MateriaalId,
                    Naam = m.Naam,
                    Omschrijving = m.Omschrijving,       
                })
            };
            //Melding geven indien niet alle gewenste materialen beschikbaar zijn.
            for (int i = 0; i < materiaal.Length; i++)
            {
                //NULLPOINTERS FIXEN
                aantalBeschikbaar = materialen[i].AantalInCatalogus - materialen[i].ReservatieData.FirstOrDefault(r => r.Week.Equals(week)).Aantal;
                if (aantalBeschikbaar == 0)
                {
                    ModelState.AddModelError("", string.Format("Materiaal {0} is niet meer beschikbaar in beschikbaar van {1} tot {2}", materialen[i].Naam, FirstDateOfWeekISO8601(2016, week), FirstDateOfWeekISO8601(2016, week).AddDays(5)));
                }
                else if (aantalBeschikbaar < aantal[i])
                {
                    ModelState.AddModelError("",string.Format("Slechts {0} stuks van materiaal {1} beschikbaar", aantalBeschikbaar, materialen[i].Naam));
                }   
            }
            return PartialView("Verlanglijst", vm);
        }
        [HttpPost]
        public void MaakReservatie(List<int> ids, DateTime startDatum, Gebruiker gebruiker)
        {
            List<Materiaal> materialen = ids.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
            if (materialen != null)
            {
                try
                {
                    gebruiker.VoegReservatieToe(materialen, startDatum);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Reservatie werd aangemaakt";
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