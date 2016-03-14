using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class ReservatieDetailViewModelFactory : ViewModelFactory
    {
        public override IViewModel CreateReservatieDetailViewModel(Reservatie reservatie)
        {
            ReservatieDetailViewModel vm = new ReservatieDetailViewModel
            {
                Aantal = reservatie.Aantal,
                Email = reservatie.Gebruiker.Email,
                Status = reservatie.ReservatieState.GetType().Name.ToLower(),
                Type = reservatie.Gebruiker is Lector ? "Lector" : "Student",
                GeblokkeerdTot = reservatie.Gebruiker is Lector ? reservatie.EindDatum.ToString("d") : ""
            };
            return vm;
        }
    }
}