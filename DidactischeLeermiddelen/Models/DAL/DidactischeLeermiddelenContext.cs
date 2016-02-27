using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class DidactischeLeermiddelenContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<Materiaal> Materialen { get; set; }
        public DbSet<Doelgroep> Doelgroepen { get; set; }
        public DbSet<Leergebied> Leergebieden { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DidactischeLeermiddelenContext() : base("DidactischeLeermiddelen")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }

        public static DidactischeLeermiddelenContext Create()
        {
            return DependencyResolver.Current.GetService<DidactischeLeermiddelenContext>();
        }

    }
}