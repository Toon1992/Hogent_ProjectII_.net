using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class DoelgroepMapper: EntityTypeConfiguration<Doelgroep>
    {
        public DoelgroepMapper()
        {
            Property(d => d.Naam);
        }
    }
}