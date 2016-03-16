namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IGebruikerRepository
    {
        Gebruiker FindByName(string email);
        void SaveChanges();
        void AddGebruiker(Gebruiker gebruiker);
    }
}