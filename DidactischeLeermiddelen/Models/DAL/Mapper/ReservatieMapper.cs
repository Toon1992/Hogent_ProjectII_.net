using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class ReservatieMapper : EntityTypeConfiguration<Reservatie>
    {
        public ReservatieMapper()
        {
            HasKey(m => m.ReservatieId);
            HasRequired(m => m.ReservatieState).WithOptional().WillCascadeOnDelete(false);
        }
    }
}