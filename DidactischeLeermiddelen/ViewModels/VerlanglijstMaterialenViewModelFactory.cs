using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class VerlanglijstMaterialenViewModelFactory : ViewModelFactory
    {
        public override  IViewModel CreateViewModel(SelectList doelgroepen, SelectList leergebieden, IEnumerable<Materiaal> lijst = null,
            DateTime startDatum = new DateTime(), Gebruiker gebruiker = null)
        {
            IViewModel vmm = new VerlanglijstMaterialenViewModel()
            {
                VerlanglijstViewModels = gebruiker.Verlanglijst.Materialen.Select(b => new VerlanglijstViewModel(b, startDatum))
            };
            return vmm;
        }
    }
}