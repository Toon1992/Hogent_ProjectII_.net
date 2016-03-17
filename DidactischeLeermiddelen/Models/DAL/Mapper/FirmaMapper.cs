using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class FirmaMapper:EntityTypeConfiguration<Firma>
    {
        public FirmaMapper()
        {
            Property(m => m.Naam).IsRequired();
            Property(m => m.Email).IsRequired();
        }
    }
}