using System.Data.Entity.ModelConfiguration;
using DidactischeLeermiddelen.Models.Domain.Mail;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class MailTemplateMapper:EntityTypeConfiguration<MailTemplate>
    {
        public MailTemplateMapper()
        {
            HasKey(m => m.Subject);
            Map<MailNaReservatie>(m => m.Requires("onderwerp").HasValue("mailNaReservatie"));
            Map<MailNaBlokkeringLector>(m => m.Requires("onderwerp").HasValue("mailNaBlokkeringLector"));
        }
    }
}