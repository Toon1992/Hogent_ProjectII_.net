using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
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
            return materialen
                .Include(m => m.Leergebieden)
                .Include(m => m.Doelgroepen)
                .OrderBy(m => m.Naam);
        }

        public Materiaal FindById(int id)
        {
            return materialen.FirstOrDefault(m => m.MateriaalId.Equals(id));
        }

        public IList<Materiaal> FindByTrefWoord(string trefwoord)
        {
            //Lijsten opvullen met resultaten
            ICollection<Materiaal> resultMaterialen =
                materialen
                .Where(m => m.Naam.Contains(trefwoord))
                .Include(m => m.Leergebieden)
                .Include(m => m.Doelgroepen)
                .ToList();
            IEnumerable<Materiaal> trefwoordMaterialen = 
                materialen
                .Where(m =>m.Omschrijving.Contains(trefwoord))
                .Include(m => m.Leergebieden)
                .Include(m => m.Doelgroepen)
                .ToList();

            //Lijsten samen brengen
            foreach (var materiaal in trefwoordMaterialen)
            {
                //Als de materiaal nog niet in de resultaten zit mag dit toegvoegd worden
                if(!resultMaterialen.Contains(materiaal))
                    resultMaterialen.Add(materiaal);
            }
       
            return resultMaterialen.OrderBy(m => m.Naam).ToList();
        }

        public IQueryable<Materiaal> FindByDoelgroep(int doelgroepId)
        {
            return materialen
                .Where(m => m.Doelgroepen.Any(d => d.DoelgroepId.Equals(doelgroepId)))
                .Include(m => m.Leergebieden).Include(m => m.Doelgroepen)
                .OrderBy(m => m.Naam);
        }

        public IQueryable<Materiaal> FindByLeergebied(int leergebiedId)
        {
            return materialen
                .Where(m => m.Leergebieden.Any(d => d.LeergebiedId.Equals(leergebiedId)))
                .Include(m => m.Leergebieden)
                .Include(m => m.Doelgroepen)
                .OrderBy(m => m.Naam);
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