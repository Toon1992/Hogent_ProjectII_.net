using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Lector : Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public void MaakBlokkeringen(IDictionary<Materiaal, int> PotentieleReservaties, string startDatum, string eindDatum)
        {
           

            foreach (KeyValuePair<Materiaal, int> potentiele in PotentieleReservaties)
            {
                if (potentiele.Key.CheckNieuwAantal(Convert.ToDateTime(startDatum)) >= potentiele.Value)
                {
                    VoegReservatieToe(potentiele.Key,potentiele.Value, startDatum, eindDatum, false);
                }            
            }

            //VerzendMailNaarLectorNaBlokkering(reservaties, startDatum, eindDatum);
        }

        private void VerzendMailNaarLectorNaBlokkering(ICollection<Reservatie> reservatiesOmTeBlokkeren, string startDatum, string eindDatum)
        {
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", this.Email);// hier nog gebruiker email pakken, nu testen of het werkt

            m.Subject = "Blokkering van reservatie";
            m.Body = string.Format("Dag {0} <br/>", this.Naam);
            m.IsBodyHtml = true;
            m.Body += "<p>U heeft zonet het volgende geblokkeerd: </p>";
            m.Body += "<ul>";
            foreach (var item in reservatiesOmTeBlokkeren)
            {
                m.Body += $"<li>{item.Aantal} x {item.Materiaal.Naam}</li>";
            }
            m.Body += "</ul>";
            m.Body += "<br/>";
            m.Body += $"<p>De periode van blokkering is van {startDatum} tot {eindDatum}</p>";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}