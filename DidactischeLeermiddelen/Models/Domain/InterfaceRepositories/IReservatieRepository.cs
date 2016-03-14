using System.Collections.Generic;
using System.Linq;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IReservatieRepository
    {
        IQueryable<Reservatie> FindAll();
        IList<Reservatie> FindByEmail(string email);
        Reservatie FindById(int id);
        void Remove(Reservatie reservatie);
        void SaveChanges();
    }
}
