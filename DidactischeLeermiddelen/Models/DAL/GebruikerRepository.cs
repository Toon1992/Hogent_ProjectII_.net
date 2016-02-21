using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class GebruikerRepository:IGebruikerRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<IGebruiker> gebruikers;

        public GebruikerRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            this.gebruikers = context.Gebruikers;
           
        } 
        public IGebruiker FindByName(string email)
        {
            return gebruikers.FirstOrDefault(g => g.Email.Equals(email));
        }


        public void SaveChanges()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
               Console.WriteLine(e.InnerException.Message);
            }
            
        }

        public void AddGebruiker(IGebruiker gebruiker)
        {
            gebruikers.Add(gebruiker);
        }
    }
}