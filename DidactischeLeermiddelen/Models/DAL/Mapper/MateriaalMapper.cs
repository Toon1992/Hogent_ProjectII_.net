﻿using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class MateriaalMapper : EntityTypeConfiguration<Materiaal>
    {
        public MateriaalMapper()
        {
            HasKey(m => m.MateriaalId);
            HasMany(m => m.Reservaties).WithRequired(m => m.Materiaal).Map(m => m.MapKey("MateriaalId")).WillCascadeOnDelete(false);
            HasMany(m => m.Doelgroepen).WithMany().Map(m =>
            {
                m.MapLeftKey("MateriaalId");
                m.MapRightKey("DoelgroepId");
                m.ToTable("MateriaalDoelgroep");
            });

            HasMany(m => m.Leergebieden).WithMany().Map(m =>
            {
                m.MapLeftKey("MateriaalId");
                m.MapRightKey("LeergebiedId");
                m.ToTable("MateriaalLeergebied");
            });

            HasOptional(m => m.Firma).WithMany().WillCascadeOnDelete(false); // withrequired, dan maar 1 materiaal met die firma, kan meerdere materialen van zelfde firma zijn
            Ignore(m => m.ImageSrc);
            Property(m => m.Naam).IsRequired();
            Property(m => m.ArtikelNr).IsRequired();
            Property(m => m.AantalInCatalogus).IsRequired();

        }
    }
}