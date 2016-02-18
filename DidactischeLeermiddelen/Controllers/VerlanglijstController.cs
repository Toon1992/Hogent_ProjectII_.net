using System;
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
            VerlanglijstMaterialenViewModel vm = new VerlanglijstMaterialenViewModel()
            {
                Materialen = gebruiker.Verlanglijst.Materialen.Select(b => new VerlanglijstViewModel(b))
            };
            return View(vm.Materialen);
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
    }
}