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
            HasMany(r => r.GeblokkeerdeDagen).WithOptional().Map(m => m.MapKey("ReservatieId")).WillCascadeOnDelete(true);
            //HasRequired(m => m.ReservatieState).WithOptional().WillCascadeOnDelete(false);
        }
    }
}