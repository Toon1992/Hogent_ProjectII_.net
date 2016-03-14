using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class ReservatieMapper : EntityTypeConfiguration<Reservatie>
    {
        public ReservatieMapper()
        {
            HasKey(m => m.ReservatieId);
            Ignore(m => m.ReservatieState);
            HasMany(r => r.Dagen).WithOptional().Map(m => m.MapKey("ReservatieId"));
            //HasRequired(m => m.ReservatieState).WithOptional().WillCascadeOnDelete(false);
        }
    }
}