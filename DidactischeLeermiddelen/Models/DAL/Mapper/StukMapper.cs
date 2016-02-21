using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class StukMapper:EntityTypeConfiguration<Stuk>
    {
        public StukMapper()
        {
            HasKey(s => s.StukId);
            HasMany(s => s.StatusData).WithMany().Map(m =>
            {
                m.ToTable("StukStatusData");
                m.MapRightKey("StukId");
                m.MapLeftKey("StatusDataId");
            });
        }
    }
}