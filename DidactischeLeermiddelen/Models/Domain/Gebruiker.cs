using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mail;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public abstract class Gebruiker
    {
        public string Email { get; set; }
        public string Naam { get; set; }
        public virtual Verlanglijst Verlanglijst { get; set; }
        public virtual IList<Reservatie> Reservaties { get; set; }

        public void VoegMateriaalAanVerlanglijstToe(Materiaal materiaal)
        {

            if (materiaal == null)
                throw new ArgumentNullException(
                    "Materiaal mag niet null zijn als die wordt toevoegd aan de verlanglijst!");

            //aan de Velanglijst materiaal Toevoegen      
            Verlanglijst.VoegMateriaalToe(materiaal);
        }

        public void VerwijderMateriaalUitVerlanglijst(Materiaal materiaal)
        {
            if (materiaal == null)
                throw new ArgumentNullException(
                    "Materiaal mag niet null zijn als die wordt verwijdert van de verlanglijst!");

            //Verwijderen van materiaal van de verlanglijst
            Verlanglijst.VerwijderMateriaal(materiaal);
        }

        public void VoegReservatieToe(IList<Materiaal> materiaal, int[] aantal, int week,Gebruiker gebruiker)
        {
            
            if(materiaal.Count != aantal.Length)
                throw new ArgumentException("Er moeten evenveel aantallen zijn als materialen");

            int index = 0;
            materiaal.ForEach(m =>
            {
                for(int i = 0; i < aantal[index];i++)
                {
                    Reservatie reservatie = new Reservatie();
                    if (reservatie.MaakReservatie(m, week))
                        Reservaties.Add(reservatie);

                }
                VerzendMailNaReservatie(gebruiker,materiaal,week);

                index++;
            });

            //Reservatie reservatie = new Reservatie();
            //reservatie.MaakReservatie(materiaal, startDatum);
            //Reservaties.Add(reservatie);
        }

        private void VerzendMailNaReservatie(Gebruiker gebruiker, IList<Materiaal> materialen,int week)
        {
            DateTime startDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Today.Year, week);
            DateTime eindDatum = startDatum.AddDays(4);
            // ook nog datum erbij pakken tot wanneer uitgeleend
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", "projecten2groep6@gmail.com");// hier nog gebruiker email pakken, nu testen of het werkt

            m.Subject = "Bevestiging reservatie";
            m.Body = string.Format("Dag {0} <br/>", gebruiker.Naam);
            m.IsBodyHtml = true;
            m.Body += "<p>Hieronder vind je terug wat je zonet reserveerde: </p>";
            m.Body += "<ul>";
            foreach (var item in materialen)
            {
                m.Body += $"<li>{item.Naam}</li>";
            }
            m.Body += "</ul>";
            m.Body += "<br/>";
            m.Body += $"<p>Je periode van reservatie is van {startDatum} tot {eindDatum}</p>";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}