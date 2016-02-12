using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class MateriaalRepository : IMateriaalRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<Materiaal> materialen;

        public MateriaalRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            materialen = context.Materialen;
        }
        public IQueryable<Materiaal> FindAll()
        {
            return materialen;
        }

        public IQueryable<Materiaal> FindByTrefWoord(string trefwoord)
        {
            return materialen.Where(m => m.Naam.Contains(trefwoord) || m.Omschrijving.Contains(trefwoord));
        }

        public IQueryable<Materiaal> FindByDoelgroep(int doelgroepId)
        {
            return materialen.Where(m => m.Doelgroepen.Any(d => d.DoelgroepId.Equals(doelgroepId)));
        }

        public IQueryable<Materiaal> FindByLeergebied(int leergebiedId)
        {
            return materialen.Where(m => m.Leergebieden.Any(d => d.LeergebiedId.Equals(leergebiedId)));
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}