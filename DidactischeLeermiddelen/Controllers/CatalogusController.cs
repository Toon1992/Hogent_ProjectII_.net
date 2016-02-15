﻿using DidactischeLeermiddelen.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using DidactischeLeermiddelen.ViewModels;
using WebGrease.Css.Extensions;
namespace DidactischeLeermiddelen.Controllers
{
    [Authorize]
    public class CatalogusController : Controller
    {
        private IMateriaalRepository materiaalRepository;
        private IDoelgroepRepository doelgroepRepository;
        private ILeergebiedRepository leergebiedRepository;
        private IGebruikerRepository gebruikerRepository;
        public CatalogusController(IMateriaalRepository materiaalRepository, IDoelgroepRepository doelgroepRepository, ILeergebiedRepository leergebiedRepository, IGebruikerRepository gebruikerRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.doelgroepRepository = doelgroepRepository;
            this.leergebiedRepository = leergebiedRepository;
            this.gebruikerRepository = gebruikerRepository;
        }
        public ActionResult Index()
        {
            List<Materiaal> materiaal = materiaalRepository.FindAll().ToList();
            MaterialenViewModel vm = CreateMaterialenViewModel(materiaal);
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus", vm);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult Filter(int[] doelgroepenLijst, int[] leergebiedenLijst)
        {
            List<Materiaal> materialen = new List<Materiaal>();
            List<Materiaal> materiaalDoelgroep = new List<Materiaal>();
            //Indien er geen checkboxen aangeklikt werden zulllen alle materialen getoont worden.
            if (doelgroepenLijst == null && leergebiedenLijst == null)
            {
                materialen = materiaalRepository.FindAll().ToList();
            }
            //Per leergebied en doelgroep worden alle materialen van de desbetreffende doelgroepen en
            // leergebieden in de lijst gestoken en doorgegeven naar het vm
            else
            {
                leergebiedenLijst.ForEach(i =>
                {
                    materialen.AddRange(materiaalRepository.FindByLeergebied(i));
                });
                doelgroepenLijst.ForEach(i =>
                {
                    materiaalDoelgroep.AddRange(materiaalRepository.FindByDoelgroep(i));
                });
                //Als de lijst van doelgroepen niet leeg is wordt het gemeenschappelijke eruit gehaald.
                if (materiaalDoelgroep.Any())
                {
                    materialen = materiaalDoelgroep.Intersect(materialen).ToList();
                }
                
            }

            MaterialenViewModel vm = CreateMaterialenViewModel(materialen.Distinct());
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus", vm);
            }
            return View("Catalogus", vm);
        }

        public ActionResult FilterMobile(int doelgroepId = 0, int leergebiedId = 0)
        {
            List<Materiaal> materiaal;
            if (doelgroepId == 0 && leergebiedId == 0)
            {
                materiaal = materiaalRepository.FindAll().ToList();
            }
            else if (doelgroepId == 0)
            {
                materiaal = materiaalRepository.FindByLeergebied(leergebiedId).ToList();
            }
            else if (leergebiedId == 0)
            {
                materiaal = materiaalRepository.FindByDoelgroep(doelgroepId).ToList();
            }
            else
            {
                var materiaalDoelgroep = materiaalRepository.FindByLeergebied(leergebiedId).ToList();
                var materiaalLeergebied = materiaalRepository.FindByDoelgroep(doelgroepId).ToList();
                materiaal = materiaalDoelgroep.Intersect(materiaalLeergebied).ToList();
            }
            MaterialenViewModel vm = CreateMaterialenViewModel(materiaal, doelgroepId, leergebiedId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus", vm);
            }
            return View("Catalogus", vm);
        }

        [HttpPost]
        public ActionResult VoegAanVerlanglijstToe(int id, Gebruiker gebruiker)
        {
            Materiaal materiaal = materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id);
            if (materiaal != null)
            {
                try
                {
                    gebruiker.VoegMateriaalAanVerlanglijstToe(materiaal);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Item {materiaal.Naam} werd toegevoegd aan verlanglijst";
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

        public ActionResult Zoek(string trefwoord)
        {

            //LijstMaken waar we het gezochte materiaal vinden
            IEnumerable<Materiaal> gezochteMaterialen = new List<Materiaal>();
            //DropDownlist maken
            ViewBag.Doelgroepen = GetDoelgroepenSelectedList();
            ViewBag.Leergebieden = GetLeergebiedSelectedList();
            //Als er niks bevind in de textbox veranderd er niks
            if (trefwoord == null || trefwoord.IsEmpty())
            {
                return RedirectToAction("Index");
            }
            //Opzoek gaan naar de materialen in de repository die aan het trefwoord voldoet
            //gezochteMaterialen = materiaalRepository.FindByTrefWoord(trefwoord);
            gezochteMaterialen = materiaalRepository.FindByTrefWoord(trefwoord);
            //Van de gevondeMaterialen een viewmodel maken en doorsturen naar de index
            MaterialenViewModel vm = CreateMaterialenViewModel(gezochteMaterialen);
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus", vm);
            }
            return View("Index", vm);
        }

        public ActionResult VerwijderZoekResultaat()
        {
            return RedirectToAction("index");
        }

        //Hulpmethode voor het aanmaken van de materialenViewModel
        private MaterialenViewModel CreateMaterialenViewModel(IEnumerable<Materiaal> lijst, int doelgroepId = 0, int leergebiedId = 0)
        {
            MaterialenViewModel vm = new MaterialenViewModel()
            {
                Materialen = lijst.Select(b => new MateriaalViewModel(b)),
                Doelgroepen = doelgroepRepository.FindAll().ToList(),
                Leergebieden = leergebiedRepository.FindAll().ToList(),
            };
            ViewBag.Doelgroepen = GetDoelgroepenSelectedList(doelgroepId);
            ViewBag.Leergebieden = GetLeergebiedSelectedList(leergebiedId);
            return vm;
        }
        private SelectList GetDoelgroepenSelectedList(int doelgroepId = 0)
        {
            return new SelectList(doelgroepRepository.FindAll().OrderBy(d => d.Naam),
                "DoelgroepId", "Naam", doelgroepId);
        }

        private SelectList GetLeergebiedSelectedList(int leergebiedId = 0)
        {
            return new SelectList(leergebiedRepository.FindAll().OrderBy(d => d.Naam),
                "LeergebiedId", "Naam", leergebiedId);
        }
    }
}