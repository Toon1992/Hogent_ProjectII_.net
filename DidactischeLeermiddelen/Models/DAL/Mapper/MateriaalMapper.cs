using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class MateriaalMapper : EntityTypeConfiguration<Materiaal>
    {
        public MateriaalMapper()
        {
            HasKey(m => m.ArtikelNr);
            HasMany(m => m.Doelgroepen).WithRequired().Map(m => m.MapKey("MateriaalNr")).WillCascadeOnDelete(false);
            HasMany(m => m.Leergebieden).WithRequired().Map(m => m.MapKey("MateriaalNr")).WillCascadeOnDelete(false);
            Property(m => m.Naam).IsRequired();
            Property(m => m.AantalInCatalogus).IsRequired();
        }
    }
}