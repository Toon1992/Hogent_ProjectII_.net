using System.Linq;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface ILeergebiedRepository
    {
        IQueryable<Leergebied> FindAll();
        Leergebied FindById(int id);
        void SaveChanges();
    }
}