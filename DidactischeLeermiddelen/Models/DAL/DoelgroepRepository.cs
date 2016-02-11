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

        public IQueryable<Doelgroep> FindAll()
        {
            return doelgroepen;
        }

        public Doelgroep FindById(int id)
        {
            return doelgroepen.FirstOrDefault(d => d.DoelgroepId.Equals(id));
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}