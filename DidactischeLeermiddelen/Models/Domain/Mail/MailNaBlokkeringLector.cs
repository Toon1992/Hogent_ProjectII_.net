using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public class MailNaBlokkeringLector:MailTemplate
    {
        public override void VerzendMail(IDictionary<Materiaal, int> reservaties,Materiaal materiaal, string startDatum, string eindDatum, string[] dagen, Gebruiker gebruiker)
        {
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", gebruiker.Email);


            m.Subject = Subject;

            StringBuilder lijst = new StringBuilder(Body);
            string items = "";
            string dagenMail = "";

            foreach (var item in reservaties)
            {
                items += string.Format("<li>" + item.Key.Naam + " x" + item.Value + "</li>" + "\n");

            }

            if (dagen == null)
            {
                dagenMail = startDatum;

            }
            else
            {
                foreach (var dag in dagen)
            {
                dagenMail += dag + ", ";
            }
                
            }
            

            lijst.Replace("_NAAM", gebruiker.Naam);
            lijst.Replace("_ITEMS", items);
            lijst.Replace("_DATUMS", dagenMail);
            m.Body = lijst.ToString();
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(m);
        }
        
    }
}