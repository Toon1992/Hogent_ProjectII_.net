﻿using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class GebruikerMapper:EntityTypeConfiguration<Gebruiker>
    {
        public GebruikerMapper()
        {
            HasKey(m => m.Email);
            HasOptional(g => g.Verlanglijst).WithRequired().Map(m => m.MapKey("GebruikerEmail")).WillCascadeOnDelete(false);
            HasMany(g => g.Reservaties).WithRequired(g => g.Gebruiker).Map(m => m.MapKey("GebruikerEmail"));
            Map<Student>(s => s.Requires("Type").HasValue("ST"));
            Map<Lector>(l => l.Requires("Type").HasValue("LE"));
        }
    }
}