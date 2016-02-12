﻿using DidactischeLeermiddelen.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using DidactischeLeermiddelen.ViewModels;
using WebGrease.Css.Extensions;
namespace DidactischeLeermiddelen.Controllers
{
    public class CatalogusController : Controller
    {
        private IMateriaalRepository materiaalRepository;
        private IDoelgroepRepository doelgroepRepository;
        private ILeergebiedRepository leergebiedRepository;
        public CatalogusController(IMateriaalRepository materiaalRepository, IDoelgroepRepository doelgroepRepository, ILeergebiedRepository leergebiedRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.doelgroepRepository = doelgroepRepository;
            this.leergebiedRepository = leergebiedRepository;
        }
        public ActionResult Index(int doelgroepId = 0, int leergebiedId = 0)
        {
            List<Materiaal> materiaal;
            Leergebied leergebied;
            Doelgroep doelgroep;
            if (doelgroepId == 0 && leergebiedId == 0)
            {
                materiaal = materiaalRepository.FindAll().ToList();
            }
            else if (doelgroepId == 0)
            {
                leergebied = leergebiedRepository.FindById(leergebiedId);
                materiaal = leergebied.Materialen;
            }
            else if (leergebiedId == 0)
            {
                doelgroep = doelgroepRepository.FindById(doelgroepId);
                materiaal = doelgroep.Materialen;
            }
            else
            {
                doelgroep = doelgroepRepository.FindById(doelgroepId);
                leergebied = leergebiedRepository.FindById(leergebiedId);
                materiaal = doelgroep.Materialen.Intersect(leergebied.Materialen).ToList();
            }
            MaterialenViewModel vm = new MaterialenViewModel()
            {
                Materialen = materiaal.Select(b => new MateriaalViewModel(b)),
            };
            ViewBag.Doelgroepen = GetDoelgroepenSelectedList(doelgroepId);
            ViewBag.Leergebieden = GetLeergebiedSelectedList(leergebiedId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus", vm.Materialen);
            }
            return View(vm);
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
        public ActionResult VoegAanVerlanglijstToe(int id, int aantal, Verlanglijst verlanglijst)
        {
            Materiaal materiaal = materiaalRepository.FindAll().FirstOrDefault(m => m.ArtikelNr == id);
            if (materiaal != null)
            {
                try
                {
                    verlanglijst.VoegMateriaalToe(materiaal, aantal);
                    TempData["Info"] = $"Item {materiaal.Naam} werd toegevoegd aan verlanglijst";
                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Zoek(String trefwoord)
        {
            //LijstMaken waar we het gezochte materiaal vinden
            IEnumerable<Materiaal> gezochteMaterialen = new List<Materiaal>();
            //DropDownlist maken
            ViewBag.Doelgroepen = GetDoelgroepenSelectedList();
            ViewBag.Leergebieden = GetLeergebiedSelectedList();
            //Als er niks bevind in de textbox veranderd er niks
            if (trefwoord == null || trefwoord.IsEmpty())
                return View("Index");
            //Opzoek gaan naar de materialen in de repository die aan het trefwoord voldoet
            gezochteMaterialen = materiaalRepository.FindByTrefWoord(trefwoord);
            //Van de gevondeMaterialen een viewmodel maken en doorsturen naar de index
            MaterialenViewModel vm = new MaterialenViewModel()
            {
                Materialen = gezochteMaterialen.Select(b => new MateriaalViewModel(b)),
            };
            return View("Index", vm);
        }
    }
}