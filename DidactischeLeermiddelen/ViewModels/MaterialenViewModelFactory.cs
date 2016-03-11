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
        public override IViewModel CreateFirmaViewModel(Materiaal materiaal)
        {
            return new FirmaViewModel(materiaal.Firma);
        }
        public override IViewModel CreateMateriaalViewModel(Materiaal materiaal)
        {
            return new MateriaalViewModel(materiaal);
        }

        public override IViewModel CreateMaterialenViewModel(SelectList doelgroepen, SelectList leergebieden, IEnumerable<Materiaal> lijst)
        {
            IViewModel vm = new MaterialenViewModel
            {
                Materialen = lijst.Select(b => new MateriaalViewModel(b)),
                DoelgroepSelectList = new DoelgroepViewModel(doelgroepen),
                LeergebiedSelectList = new LeergebiedViewModel(leergebieden)
            };
            return vm;
        }
    }
}