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
            HasKey(g => g.GebruikersId);
            HasOptional(g => g.Verlanglijst).WithRequired();
            HasMany(g => g.Reservaties).WithOptional();
        }
    }
}