using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class FirmaViewModel:IViewModel
    {
        public string Naam { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public string Contactpersoon { get; set; }
        public string Website { get; set; }
        public FirmaViewModel(Firma firma)
        {
            Naam = firma.Naam;
            Email = firma.Email;
            Adres = firma.Adres;
            Contactpersoon = firma.Contactpersoon;
            Website = firma.Website;
        }
    }
}