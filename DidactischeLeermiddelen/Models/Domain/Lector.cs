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

        public void MaakBlokkeringen(IDictionary<Materiaal, int> potentieleReservaties, string startDatum, string eindDatum)
        {
            DateTime start = Convert.ToDateTime(startDatum);
            DateTime einde = Convert.ToDateTime(eindDatum);
            

            foreach (KeyValuePair<Materiaal, int> potentiele in potentieleReservaties)
            {
                ICollection<Reservatie> reservaties = potentiele.Key.Reservaties.Where(r =>!(r.ReservatieState is  Geblokkeerd)).OrderBy(r => r.StartDatum).ToList();
                int aantalBeschikbaar = potentiele.Key.GeefAantalBeschikbaarLector(start, einde);

                if (aantalBeschikbaar >= potentiele.Value)
                {
                    VoegReservatieToe(potentiele.Key, potentiele.Value, startDatum, eindDatum, true);
                }
                else
                {
                    int aantal = potentiele.Value - aantalBeschikbaar;
                    int berekenAantal = aantal;

                    IList<Reservatie> res = potentiele.Key.Reservaties.Where(r => r.StartDatum <= start).ToList();

                    while (berekenAantal > 0)
                    {
                        Reservatie last = res.Last();
                        
                        if (last.Aantal < berekenAantal)
                        {
                            berekenAantal -= last.Aantal;
                        }
                        else
                        {
                            berekenAantal = 0;
                        }

                        last.ReservatieState.Blokkeer();
                        res.Remove(last);
                    }

                    VoegReservatieToe(potentiele.Key, aantal, startDatum, eindDatum, true);

                }

                //else
                //{
                //    Reservatie reservatie = reservaties.First();

                //    Gebruiker gebruiker = reservatie.Gebruiker;
                //    Materiaal materiaal = reservatie.Materiaal;

                //    Reservatie gebruikerReservatie = gebruiker.Reservaties.First(r => r.Equals(reservatie));
                //    Reservatie materiaalReservatie = materiaal.Reservaties.First(r => r.Equals(reservatie));

                //    reservatie.Blokkeer();
                //    reservatie.Status = Status.Geblokkeerd;
                //    TimeSpan span = Convert.ToDateTime(startDatum) - Convert.ToDateTime(eindDatum);
                //    reservatie.AantalDagenGeblokkeerd = span.Days;
           
                //    gebruikerReservatie = reservatie;
                //    materiaalReservatie = reservatie;

                //    reservaties.Remove(reservatie);

                //    VoegReservatieToe(reservatie.Materiaal,1,startDatum,eindDatum,true);
                //}

            }

            //VerzendMailNaarLectorNaBlokkering(reservaties, startDatum, eindDatum);
        }

        private void VerzendMailNaarLectorNaBlokkering(IDictionary<Materiaal, int> reservatiesOmTeBlokkeren, string startDatum, string eindDatum)
        {
            MailMessage m = new MailMessage("projecten2groep6@gmail.com", this.Email);// hier nog gebruiker email pakken, nu testen of het werkt

            m.Subject = "Blokkering van reservatie";
            m.Body = string.Format("Dag {0} <br/>", this.Naam);
            m.IsBodyHtml = true;
            m.Body += "<p>U heeft zonet het volgende geblokkeerd: </p>";
            m.Body += "<ul>";
            foreach (var item in reservatiesOmTeBlokkeren)
            {
                m.Body += $"<li>{item.Value} x {item.Key.Naam}</li>";
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