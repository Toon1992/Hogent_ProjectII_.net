using System.Collections.Generic;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public abstract class MailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public abstract void VerzendMail(IDictionary<Materiaal,int> reservaties,Materiaal materiaal, string startDatum,string eindDatum,string[] dagen,Gebruiker gebruiker );
        
    }
}
