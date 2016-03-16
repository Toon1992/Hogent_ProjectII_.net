using System.Data.Entity;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain.InterfaceRepositories;
using DidactischeLeermiddelen.Models.Domain.Mail;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class MailServiceRepository:IMailServiceRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<MailTemplate> mailTemplates; 
        public MailServiceRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            mailTemplates = context.MailTemplates;

        }
        public MailTemplate GeefMailTemplate(string onderwerp)
        {
            return mailTemplates.FirstOrDefault(m => m.Subject.Equals(onderwerp));
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}