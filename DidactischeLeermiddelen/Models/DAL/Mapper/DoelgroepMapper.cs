using System.Data.Entity.ModelConfiguration;
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