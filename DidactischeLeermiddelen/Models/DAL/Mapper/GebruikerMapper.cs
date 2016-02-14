using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class GebruikerMapper:EntityTypeConfiguration<Gebruiker>
    {
        public GebruikerMapper()
        {
            HasKey(g => g.Email);
            //HasRequired(g => g.Verlanglijst).WithMany().WillCascadeOnDelete(false);
            HasOptional(g => g.Verlanglijst).WithRequired();
            HasMany(g => g.Reservaties).WithOptional();
        }
    }
}