using DidactischeLeermiddelen.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private ViewModelFactory factory;
        public CatalogusController(IMateriaalRepository materiaalRepository, IDoelgroepRepository doelgroepRepository, ILeergebiedRepository leergebiedRepository, IGebruikerRepository gebruikerRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.doelgroepRepository = doelgroepRepository;
            this.leergebiedRepository = leergebiedRepository;
            this.gebruikerRepository = gebruikerRepository;
        }

        public CatalogusController()
        {
            
        }

        public ActionResult Index(Gebruiker gebruiker, int[] doelgroepenLijst, int[] leergebiedenLijst, string trefwoord)
        {
            IList<Materiaal> materialen = new List<Materiaal>();

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
                    materialen = GeefGefilterdeMaterialen(doelgroepenLijst, leergebiedenLijst, materialen);
                }
            }

            if (gebruiker is Student)
            {
                materialen = materialen.Where(m => m.IsReserveerBaar != null && (bool) m.IsReserveerBaar).ToList();
            }  

            materialen.ForEach(m =>
            {
                m.InVerlanglijst = gebruiker.Verlanglijst.BevatMateriaal(m);
            });
            factory = new MaterialenViewModelFactory();
            MaterialenViewModel vm = factory.CreateMaterialenViewModel(GetDoelgroepenSelectedList(0), GetLeergebiedSelectedList(0), materialen) as MaterialenViewModel;

            if (Request.IsAjaxRequest())
            {
                return PartialView("Catalogus", vm);
            }
            return View("Index", vm);
        }


        private IList<Materiaal> GeefGefilterdeMaterialen(int[] doelgroepenLijst, int[] leergebiedenLijst, IList<Materiaal> materialen)
        {
            IList<Materiaal> materiaalDoelgroep = new List<Materiaal>();
                    leergebiedenLijst.ForEach(i =>
                    {
                materiaalRepository.FindByLeergebied(i).ForEach(m => materialen.Add(m));
                    });

                    doelgroepenLijst.ForEach(i =>
                    {
                materiaalRepository.FindByDoelgroep(i).ForEach(m => materiaalDoelgroep.Add(m));
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
            return materialen;
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
            factory = new MaterialenViewModelFactory();
            return PartialView("Detail", factory.CreateMateriaalViewModel(materiaal) as MateriaalViewModel);
        }

        public ActionResult DetailViewFirma(int id)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            factory = new MaterialenViewModelFactory();
            return PartialView("DetailFirma", factory.CreateFirmaViewModel(materiaal) as FirmaViewModel);
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