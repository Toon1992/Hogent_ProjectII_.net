using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain.Mail;

namespace DidactischeLeermiddelen.Models.Domain.InterfaceRepositories
{
    public interface IMailServiceRepository
    {
        MailService GeefMailTemplate(string onderwerp);
        void SaveChanges();
    }
}