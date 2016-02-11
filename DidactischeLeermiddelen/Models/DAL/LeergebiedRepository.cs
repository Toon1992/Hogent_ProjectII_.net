using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class LeergebiedRepository:ILeergebiedRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<Leergebied> leergebieden;

        public LeergebiedRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            leergebieden = context.Leergebieden;
        }
        public IQueryable<Leergebied> FindByLeergebiedList(Leergebied leergebied)
        {
            return leergebieden.Where(l => l.Naam.Equals(leergebied.Naam));
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}