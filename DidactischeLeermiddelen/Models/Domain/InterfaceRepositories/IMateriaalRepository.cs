using System.Collections.Generic;
using System.Linq;

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
