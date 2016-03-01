using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Lector:Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public override void VoegReservatieToe(IList<Materiaal> materiaal, int[] aantal, string startDatum, string eindDatum)
        {
            ICollection<Reservatie> nieuweReservaties = new List<Reservatie>();
            if (materiaal.Count != aantal.Length)
                throw new ArgumentException("Er moeten evenveel aantallen zijn als materialen");

            int index = 0;
            materiaal.ForEach(m =>
            {
                Reservatie reservatie = new Reservatie(this, m, startDatum,eindDatum, aantal[index]);
                reservatie.Gebruiker = this;
                reservatie.Reserveer();
                m.AddReservatie(reservatie);
                Reservaties.Add(reservatie);
                nieuweReservaties.Add(reservatie);
                index++;
            });

            VerzendMailNaReservatie(nieuweReservaties, week, this); //gebruiker, materiaal, week);
        }

        public void MaakBlokkeringen(ICollection<Reservatie> reservaties, int[] aantal, string startDatum, string eindDatum)
        {
            reservaties.ForEach(r =>
            {
                r.Status = Status.Geblokkeerd;
                r.ReservatieState.Blokkeer();
            });

            IList<Materiaal> materialen = Reservaties.Select(m => m.Materiaal).ToList(); 

            VoegReservatieToe(materialen,aantal,startDatum, eindDatum);
        }

        private void verzendMailNaarLectorNaBlokkering(ICollection<Reservatie> reservatiesOmTeBlokkeren)
        {
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", this.Email);// hier nog gebruiker email pakken, nu testen of het werkt

            m.Subject = "Bevestiging reservatie";
            m.Body = string.Format("Dag {0} <br/>", this.Naam);
            m.IsBodyHtml = true;
            m.Body += "<p>U heeft zonet het volgende geblokkeerd: </p>";
            m.Body += "<ul>";
            foreach (var item in reservatiesOmTeBlokkeren )
            {
                m.Body += $"<li>{item.Aantal} x {item.Materiaal.Naam}</li>";
            }
            m.Body += "</ul>";
            m.Body += "<br/>";
            //m.Body += $"<p>Je periode van reservatie is van {startDatum} tot {eindDatum}</p>";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}