using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mail;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public abstract class Gebruiker
    {
        public int GebruikerId { get; set; }
        public string Email { get; set; }
        public string Naam { get; set; }
        public string Faculteit { get; set; }
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

        public void VerwijderReservatie(Reservatie r)
        {
            if (Reservaties.Contains(r))
            {
                Reservaties.Remove(r);
            }
            else
            {
                throw new ArgumentException("Er is geen reservatie om te verwijderen");
            }

        }

        protected void VoegReservatieToe(Materiaal materiaal, int aantal, string startdatum, string eindDatum)
        {
            Reservatie reservatie = new Reservatie(this, materiaal, startdatum, eindDatum, aantal)
            {
                Gebruiker = this
            };

            if (this is Lector)
                reservatie.Blokkeer();
            else
            {
                reservatie.ToState(new Gereserveerd());
            }
            //reservatie.ToState(new Gereserveerd());
            materiaal.AddReservatie(reservatie);
            Reservaties.Add(reservatie);

        }

        //protected void VerzendMailNaReservatie(IDictionary<Materiaal,int> reservaties, string startDatum, string eindDatum, Gebruiker gebruiker)//Gebruiker gebruiker, IList<Materiaal> materialen,int week)
        //{
        //    DateTime start = DateTime.ParseExact(startDatum, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //    startDatum = start.ToShortDateString();
        //    //eind date niet in orde
        //    eindDatum = start.ToShortDateString();
        //    // ook nog datum erbij pakken tot wanneer uitgeleend
        //    MailMessage m = new MailMessage("projecten2groep6@gmail.com", gebruiker.Email);// hier nog gebruiker email pakken, nu testen of het werkt

        //////    m.Subject = "Bevestiging reservatie";
        //////    m.Body = string.Format("Dag {0} <br/>", gebruiker.Naam);
        //////    m.IsBodyHtml = true;
        //////    m.Body += "<p>Hieronder vind je terug wat je zonet reserveerde: </p>";
        //////    m.Body += "<ul>";
        //////    foreach (var item in reservaties)
        //////    {
        //////        m.Body += $"<li>{item.Value} x {item.Key.Naam}</li>";
        //////    }
        //////    m.Body += "</ul>";
        //////    m.Body += "<br/>";
        //////    m.Body += $"<p>Je periode van reservatie is van {startDatum} tot {eindDatum}</p>";

        ////    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        ////    smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
        ////    smtp.EnableSsl = true;
        ////    smtp.Send(m);
        ////}
    }
}