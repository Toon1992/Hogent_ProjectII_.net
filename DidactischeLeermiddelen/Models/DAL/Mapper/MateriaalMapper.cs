using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class MateriaalMapper : EntityTypeConfiguration<Materiaal>
    {
        public MateriaalMapper()
        {
            HasKey(m => m.ArtikelNr);

            HasMany(m => m.Doelgroepen).WithMany().Map(m =>
            {
                m.MapLeftKey("ArtikelNr");
                m.MapRightKey("DoelgroepId");
                m.ToTable("MateriaalDoelgroep");
            });

            HasMany(m => m.Leergebieden).WithMany().Map(m =>
            {
                m.MapLeftKey("ArtikelNr");
                m.MapRightKey("LeergebiedId");
                m.ToTable("MateriaalLeergebied");
            });

            Property(m => m.Naam).IsRequired();
            Property(m => m.AantalInCatalogus).IsRequired();
        }
    }
}