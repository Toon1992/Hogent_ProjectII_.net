using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public class MailNaReservatie : MailTemplate
    {

        protected override void VerzendMail(IDictionary<Materiaal, int> reservaties,Materiaal materiaal, string startDatum, string eindDatum, string[] dagen, Gebruiker gebruiker)
        {
            DateTime start = Convert.ToDateTime(startDatum);
            startDatum = start.ToShortDateString();
            eindDatum = start.AddDays(4).ToShortDateString();
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", gebruiker.Email);


            m.Subject = Subject;

            StringBuilder lijst = new StringBuilder(Body);
            string items = "";
            foreach (var item in reservaties)
            {
                items += string.Format("<li>" +item.Key.Naam + " x" + item.Value + "</li>" +"\n");
                
            }
            lijst.Replace("_NAAM", gebruiker.Naam);
            lijst.Replace("_STARTDATUM", startDatum);
            lijst.Replace("_EINDDATUM", eindDatum);
            lijst.Replace("_ITEMS", items);
            m.Body = lijst.ToString();
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(m);
        }
    }
    }