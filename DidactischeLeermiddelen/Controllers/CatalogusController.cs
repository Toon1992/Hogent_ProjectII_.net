using DidactischeLeermiddelen.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            MaterialenViewModel vm=new MaterialenViewModel()
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
    }
}