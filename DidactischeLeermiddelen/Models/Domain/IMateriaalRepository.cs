using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IMateriaalRepository
    {
        IQueryable<Materiaal> FindAll();
        IQueryable<Materiaal> FindByTrefWoord(string trefwoord);   
        void SaveChanges();
    }
}
