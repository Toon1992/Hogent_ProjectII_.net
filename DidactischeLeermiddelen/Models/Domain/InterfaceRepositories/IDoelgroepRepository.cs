using System.Linq;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IDoelgroepRepository
    {
        IQueryable<Doelgroep> FindAll();
        Doelgroep FindById(int id);
        void SaveChanges();
    }
}