using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                .Include(m => m.Stuks)
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
                .Include(m => m.Stuks)
                .ToList();
            IEnumerable<Materiaal> trefwoordMaterialen = 
                materialen
                .Where(m =>m.Omschrijving.Contains(trefwoord))
                .Include(m => m.Leergebieden)
                .Include(m => m.Doelgroepen)
                .Include(m => m.Stuks)
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
                .Include(m => m.Stuks)
                .OrderBy(m => m.Naam);
        }

        public IQueryable<Materiaal> FindByLeergebied(int leergebiedId)
        {
            return materialen
                .Where(m => m.Leergebieden.Any(d => d.LeergebiedId.Equals(leergebiedId)))
                .Include(m => m.Leergebieden)
                .Include(m => m.Doelgroepen)
                .Include(m => m.Stuks)
                .OrderBy(m => m.Naam);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}