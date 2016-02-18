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
        Materiaal FindById(int id);
        IList<Materiaal> FindByTrefWoord(string trefwoord);
        IQueryable<Materiaal> FindByDoelgroep(int doelgroepId);
        IQueryable<Materiaal> FindByLeergebied(int leergebiedId);

        void SaveChanges();
    }
}
