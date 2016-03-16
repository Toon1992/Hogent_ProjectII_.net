using System;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class ReservatieMaterialenViewModelFactory : ViewModelFactory
    {
        public override IViewModel CreateReservatieMaterialenViewModel(Gebruiker gebruiker)
        {
            if (gebruiker == null)
                throw new ArgumentNullException();

            IViewModel rmv = new ReservatieMaterialenViewModel()
            {
                Materialen = gebruiker.Reservaties.OrderBy(r => r.StartDatum).Select(b => new ReservatieViewModel(b)),
            };
            return rmv;
        }
    }
}