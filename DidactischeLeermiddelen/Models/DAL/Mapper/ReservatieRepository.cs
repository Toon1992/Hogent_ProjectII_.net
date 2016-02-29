using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class ReservatieRepository : IReservatieRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<Reservatie> reservaties;


        public ReservatieRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            reservaties = context.Reservaties;
        }
        public IQueryable<Reservatie> FindAll()
        {
            return reservaties;
        }

        public IList<Reservatie> FindById(string email)
        {
            return reservaties.Where(r => r.Gebruiker.Email.Equals(email)).ToList();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}