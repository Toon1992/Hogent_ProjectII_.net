using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class MateriaalRepository: IMateriaalRepository
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

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}