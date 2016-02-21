using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class GebruikerMapper:EntityTypeConfiguration<IGebruiker>
    {
        public GebruikerMapper()    
        {
            HasKey(g => g.Email);
            HasOptional(g => g.Verlanglijst).WithRequired().Map(m => m.MapKey("GebruikerEmail")).WillCascadeOnDelete(false);
            HasMany(g => g.Reservaties).WithRequired().Map(m => m.MapKey("GebruikerEmail"));
            Map<Student>(s => s.Requires("Type").HasValue("ST"));
            Map<Lector>(l => l.Requires("Type").HasValue("LE"));
        }
    }
}