using System.Collections.Generic;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public abstract class MailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public abstract void VerzendMail(IDictionary<Materiaal,int> reservaties, string startDatum,string eindDatum,Gebruiker gebruiker );
    }
}
