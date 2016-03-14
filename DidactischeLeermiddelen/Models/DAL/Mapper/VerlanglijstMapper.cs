using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class VerlanglijstMapper:EntityTypeConfiguration<Verlanglijst>
    {
        public VerlanglijstMapper()
        {
            HasKey(v => v.Id);
            HasMany(v => v.Materialen).WithMany().Map(m =>
            {
                m.ToTable("MaterialenVerlanglijst");
                m.MapLeftKey("ArtikelNr");
                m.MapRightKey("VerlanglijstId");
            });
        }
    }
}