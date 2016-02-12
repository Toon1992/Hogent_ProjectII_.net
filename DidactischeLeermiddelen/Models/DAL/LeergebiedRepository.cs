using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class LeergebiedRepository : ILeergebiedRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<Leergebied> leergebieden;

        public LeergebiedRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            leergebieden = context.Leergebieden;
        }

        public IQueryable<Leergebied> FindAll()
        {
            return leergebieden;
        }

        public Leergebied FindById(int id)
        {
            return leergebieden.FirstOrDefault(l => l.LeergebiedId.Equals(id));
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}