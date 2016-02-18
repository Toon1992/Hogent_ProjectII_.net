using DidactischeLeermiddelen.Models.Domain;
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
        public ActionResult Index(Gebruiker gebruiker, int[] doelgroepenLijst, int[] leergebiedenLijst, string trefwoord)
        {
            List<Materiaal> materialen = new List<Materiaal>();
            List<Materiaal> materiaalDoelgroep = new List<Materiaal>();
            //Indien er geen checkboxen aangeklikt werden zulllen alle materialen getoont worden.
            if (doelgroepenLijst == null && leergebiedenLijst == null && (trefwoord == null || trefwoord.IsEmpty()))
            {
                materialen = materiaalRepository.FindAll().ToList();
            }
            else
            {
                if (doelgroepenLijst == null && leergebiedenLijst == null)
                {
                    if (!trefwoord.IsEmpty())
                    {
                        materialen = materiaalRepository.FindByTrefWoord(trefwoord).ToList();
                    }
        } 
                else
                {
                    //Per leergebied en doelgroep worden alle materialen van de desbetreffende doelgroepen en
                    // leergebieden in de lijst gestoken en doorgegeven naar het vm
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
                        //Indien er een filter op leergebied geplaatst werd (materialen is niet leeg) gaan we 
                        //De gemeenschappelijke elementen van materialen en materialenDoelgroep nemen.
                        //Indien de lijst leeg is nemen we enkel de materialenDoelgroep.
                        materialen = materialen.Any()
                            ? materiaalDoelgroep.Intersect(materialen).ToList()
                            : materiaalDoelgroep;
                    }
                }
                //DropDownlist maken
                ViewBag.Doelgroepen = GetDoelgroepenSelectedList();
                ViewBag.Leergebieden = GetLeergebiedSelectedList();
                //Opzoek gaan naar de materialen in de repository die aan het trefwoord voldoet
                //gezochteMaterialen = materiaalRepository.FindByTrefWoord(trefwoord);
                
                Materialen = materiaal.Select(b => new MateriaalViewModel(b))
            };
            return View(vm);
        }
            materialen = gebruiker.IsLector ? materialen : materialen.Where(m => m.IsReserveerBaar).ToList();
            MaterialenViewModel vm = ViewModelFactory.CreateViewModel("MaterialenViewModel", materialen,
                doelgroepRepository, leergebiedRepository) as MaterialenViewModel;

            ViewBag.Doelgroepen = GetDoelgroepenSelectedList(0);
            ViewBag.Leergebieden = GetLeergebiedSelectedList(0);

            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus", vm);
            }
            return View("Index", vm);
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
            }
            return RedirectToAction("Index");
        }

        public ActionResult DetailView(int id)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            return PartialView("Detail", new MateriaalViewModel(materiaal));
        }

        //Hulpmethode voor het aanmaken van de materialenViewModel
        //private MaterialenViewModel CreateMaterialenViewModel(IEnumerable<Materiaal> lijst, int doelgroepId = 0, int leergebiedId = 0)
        //{
        //    MaterialenViewModel vm = new MaterialenViewModel()
        //    {
        //        Materialen = lijst.Select(b => new MateriaalViewModel(b)),
        //        Doelgroepen = doelgroepRepository.FindAll().ToList(),
        //        Leergebieden = leergebiedRepository.FindAll().ToList(),
        //    };
        //    ViewBag.Doelgroepen = GetDoelgroepenSelectedList(doelgroepId);
        //    ViewBag.Leergebieden = GetLeergebiedSelectedList(leergebiedId);
        //    return vm;
        //}
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