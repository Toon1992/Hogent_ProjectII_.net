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

        public CatalogusController(IMateriaalRepository repo)
        {
            materiaalRepository = repo;
        } 
        public ActionResult Index()
        {
            List<Materiaal> materiaal = materiaalRepository.FindAll().ToList();
            MaterialenViewModel vm=new MaterialenViewModel()
            {
                
                Materialen = materiaal.Select(b => new MateriaalViewModel(b))
            };
            return View(vm);
        }
    }
}