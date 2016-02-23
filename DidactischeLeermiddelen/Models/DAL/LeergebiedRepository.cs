using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
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