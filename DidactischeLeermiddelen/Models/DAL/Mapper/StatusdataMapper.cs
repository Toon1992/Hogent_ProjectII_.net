using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class StatusDataMapper:EntityTypeConfiguration<StatusData>
    {
        public StatusDataMapper()
        {
            HasKey(s => s.StatusDataId);
            Property(s => s.Week);
            Property(s => s.Status);
        }
    }
}