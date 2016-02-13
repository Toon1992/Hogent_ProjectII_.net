using DidactischeLeermiddelen.Models.Domain;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
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
        public ActionResult Index()
        {
            List<Materiaal> materiaal = materiaalRepository.FindAll().ToList();
            
            MaterialenViewModel vm = new MaterialenViewModel()
            {
                Materialen = materiaal.Select(b => new MateriaalViewModel(b)),
                Doelgroepen = doelgroepRepository.FindAll().ToList(),
                Leergebieden = leergebiedRepository.FindAll().ToList(),
            };
            ViewBag.Doelgroepen = GetDoelgroepenSelectedList();
            ViewBag.Leergebieden = GetLeergebiedSelectedList();
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus1", vm);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult Filter(int[] doelgroepenLijst, int[] leergebiedenLijst)
        {
            List<Materiaal> materialen = new List<Materiaal>();
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
                    materialen.AddRange(materiaalRepository.FindByDoelgroep(i));
                });
            }
            
            MaterialenViewModel vm = new MaterialenViewModel()
            {
                Materialen = materialen.Distinct().Select(b => new MateriaalViewModel(b)),
                Doelgroepen = doelgroepRepository.FindAll().ToList(),
                Leergebieden = leergebiedRepository.FindAll().ToList(),
            };
            ViewBag.Doelgroepen = GetDoelgroepenSelectedList();
            ViewBag.Leergebieden = GetLeergebiedSelectedList();
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus1", vm);
            }
            return View("Catalogus1", vm);
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
            MaterialenViewModel vm = new MaterialenViewModel()
            {
                Materialen = materiaal.Select(b => new MateriaalViewModel(b)),
                Doelgroepen = doelgroepRepository.FindAll().ToList(),
                Leergebieden = leergebiedRepository.FindAll().ToList(),
            };
            ViewBag.Doelgroepen = GetDoelgroepenSelectedList(doelgroepId);
            ViewBag.Leergebieden = GetLeergebiedSelectedList(leergebiedId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus1", vm);
            }
            return View("Catalogus1", vm);
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
            else
            {
                //Opzoek gaan naar de materialen in de repository die aan het trefwoord voldoet
                //gezochteMaterialen = materiaalRepository.FindByTrefWoord(trefwoord);
                gezochteMaterialen = materiaalRepository.FindByTrefWoord(trefwoord);
            }
            //Van de gevondeMaterialen een viewmodel maken en doorsturen naar de index
            MaterialenViewModel vm = createMaterialenViewModel(gezochteMaterialen);
            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus1", vm);
            }
            return View("Index",vm);
        }

        public ActionResult VerwijderZoekResultaat()
        {
            return RedirectToAction("index");
        }

        //Hulpmethode voor het aanmaken van de materialenViewModel
        private MaterialenViewModel createMaterialenViewModel(IEnumerable<Materiaal> lijst)
        {
            MaterialenViewModel vm = new MaterialenViewModel()
            {
                Materialen = lijst.Select(b => new MateriaalViewModel(b)),
                Doelgroepen = doelgroepRepository.FindAll().ToList(),
                Leergebieden = leergebiedRepository.FindAll().ToList(),
            };

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