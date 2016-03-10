using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class ReservatieMaterialenViewModelFactory : ViewModelFactory
    {
        public override IViewModel CreateViewModel(SelectList doelgroepen, SelectList leergebieden, IEnumerable<Materiaal> lijst = null,
            DateTime startDatum = new DateTime(), Gebruiker gebruiker = null)
        {
            IViewModel rmv = new ReservatieMaterialenViewModel()
            {
                Materialen = gebruiker.Reservaties.OrderBy(r => r.StartDatum).Select(b => new ReservatieViewModel(b)),
            };
            return rmv;
        }
    }
}