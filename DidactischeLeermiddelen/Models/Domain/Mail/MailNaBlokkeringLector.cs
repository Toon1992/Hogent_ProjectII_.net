using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public class MailNaBlokkeringLector:MailTemplate
    {
        public override void VerzendMail(IDictionary<Materiaal, int> reservaties, string startDatum, string eindDatum, Gebruiker gebruiker)
        {
            
        }
    }
}