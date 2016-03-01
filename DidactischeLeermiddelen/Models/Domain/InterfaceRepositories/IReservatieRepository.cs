using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IReservatieRepository
    {
        IQueryable<Reservatie> FindAll();
        IList<Reservatie> FindByEmail(string email);
        Reservatie FindById(int id);
        void SaveChanges();
    }
}
