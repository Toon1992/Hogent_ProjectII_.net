using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class LeergebiedMapper:EntityTypeConfiguration<Leergebied>
    {
        public LeergebiedMapper()
        {
            Property(l => l.Naam);
        }
    }
}