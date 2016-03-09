﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public class MailNaReservatie : MailService
    {

        public override void VerzendMail(IDictionary<Materiaal, int> reservaties, string startDatum, string eindDatum, Gebruiker gebruiker)
        {
            DateTime start = DateTime.ParseExact(startDatum, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            startDatum = start.ToShortDateString();
            eindDatum = start.ToShortDateString();
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", gebruiker.Email);


            m.Subject = "Bevestiging reservatie";
            var bodyAanpassen=new StringBuilder(Body);
            bodyAanpassen.Replace("_NAAM", gebruiker.Naam);
            m.Body = bodyAanpassen.ToString();
            m.IsBodyHtml = true;
            
            //m.Body += "<p>Hieronder vind je terug wat je zonet reserveerde: </p>";
            //m.Body += "<ul>";
            //foreach (var item in reservaties)
            //{
            //    m.Body += $"<li>{item.Value} x {item.Key.Naam}</li>";
            //}
            //m.Body += "</ul>";
            //m.Body += "<br/>";
            //m.Body += $"<p>Je periode van reservatie is van {startDatum} tot {eindDatum}</p>";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
    }