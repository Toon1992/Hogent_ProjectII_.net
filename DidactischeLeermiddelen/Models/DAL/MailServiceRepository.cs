using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain.InterfaceRepositories;
using DidactischeLeermiddelen.Models.Domain.Mail;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class MailServiceRepository:IMailServiceRepository
    {
        private DidactischeLeermiddelenContext context;
        private DbSet<MailService> mailTemplates; 
        public MailServiceRepository(DidactischeLeermiddelenContext context)
        {
            this.context = context;
            mailTemplates = context.MailTemplates;

        }
        public MailService GeefMailTemplate(string onderwerp)
        {
            return mailTemplates.FirstOrDefault(m => m.Subject.Equals(onderwerp));
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}