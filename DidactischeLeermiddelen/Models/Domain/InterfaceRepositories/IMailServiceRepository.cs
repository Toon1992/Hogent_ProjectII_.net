using DidactischeLeermiddelen.Models.Domain.Mail;

namespace DidactischeLeermiddelen.Models.Domain.InterfaceRepositories
{
    public interface IMailServiceRepository
    {
        MailTemplate GeefMailTemplate(string onderwerp);
        void SaveChanges();
    }
}