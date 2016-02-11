using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class DoelgroepRepository:IDoelgroepRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<Doelgroep> doelgroepen;

        public DoelgroepRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            doelgroepen = context.Doelgroepen;
        }
        public IQueryable<Doelgroep> FindByDoelGroep(Doelgroep doelgroep)
        {
            return doelgroepen.Where(d => d.Naam.Equals(doelgroep.Naam));
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}