using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class DoelgroepRepository : IDoelgroepRepository
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
            try
            {
                context.SaveChanges();
            }
          
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation(
                              "Class: {0}, Property: {1}, Error: {2}",
                              validationErrors.Entry.Entity.GetType().FullName,
                              validationError.PropertyName,
                              validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }
    }
}