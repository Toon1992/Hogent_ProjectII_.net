using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public class MailNaBlokkeringStudent : MailTemplate
    {
        protected override void VerzendMail(IDictionary<Materiaal, int> reservaties,Materiaal materiaal, string startDatum, string eindDatum, string[] dagen, Gebruiker gebruiker)
        {
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", gebruiker.Email);


            m.Subject = Subject;

            StringBuilder lijst = new StringBuilder(Body);
            

            lijst.Replace("_NAAM", gebruiker.Naam);
            lijst.Replace("_ITEMS", materiaal.Naam);
            lijst.Replace("_STARTDATUM", startDatum);
            m.Body = lijst.ToString();
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(m);
        }

        
    }
}