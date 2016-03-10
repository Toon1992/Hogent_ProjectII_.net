using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class MaterialenViewModelFactory : ViewModelFactory
    {
        public override IViewModel CreateViewModel(SelectList doelgroepen, SelectList leergebieden, IEnumerable<Materiaal> lijst = null,
            DateTime startDatum = new DateTime(), Gebruiker gebruiker = null)
        {
            IViewModel vm = new MaterialenViewModel()
            {
                Materialen = lijst.Select(b => new MateriaalViewModel(b)),
                DoelgroepSelectList = new DoelgroepViewModel(doelgroepen),
                LeergebiedSelectList = new LeergebiedViewModel(leergebieden)
            };
            return vm;
        }
    }
}