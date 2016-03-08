using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Lector : Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public void MaakBlokkeringen(IDictionary<Materiaal, int> potentieleReservaties, string startDatum, string eindDatum)
        {
            //Het converten van string naar DateTime
            DateTime start = Convert.ToDateTime(startDatum);
            DateTime einde = Convert.ToDateTime(eindDatum);

            //Overlopen van map met potentiele reserveringen/blokkeringen/overrulingen
            foreach (KeyValuePair<Materiaal, int> potentiele in potentieleReservaties)
            {
                //Aantal Lokale variabele aanmaken die we nodig hebben
                
                Materiaal mat = potentiele.Key;
                int reserveerAantal = potentiele.Value;
                mat.MaakReservatieLijstAan();

                //opvragen van het aantal reservaties die niet geblokkeerd, opgehaald of overruult zijn
                int aantalBeschikbaar = mat.GeefAantalBeschikbaarVoorBlokkering();

                //Eerst gaan we kijken of er nog genoeg beschikbaar zijn om gwn te reserveren
                //we vergelijken de aantal beschikbare stuks voor het materiaal met het aantal dat we nodig hebben voor onze reservatie
                //Zo ja maken we gwn reservaties (lectoren blokkeren altijd!!)
                //Zo niet gaan we over tot het overrulen van reservaties
                if (aantalBeschikbaar >= reserveerAantal)
                {
                    //Aanmaken van reservaties
                    VoegReservatieToe(mat, reserveerAantal, startDatum, eindDatum);
                }
                else
                {
                    //Hier berekenen we hoeveel stuks we nog moeten Overrulen
                    int aantal = reserveerAantal - aantalBeschikbaar;

                    //We vragen alle reservaties op van het materiaal tijdens de week dat we een reservatie willen doen
                    ICollection<Reservatie> reservatiePool = mat.GeeftReservatiesVanEenBepaaldeTijd(start);

                    //We gaan een while lus willen starten omdat het kan zijn dat er verschillende gebruikers hun reservatie kwijtgeraken
                    bool flag = false;

                    while (!flag)
                    {
                        //Eerst kijken of er nog reservaties in de pool zitten die we gaan kunnen overrulen 
                        if (reservatiePool.Count <= 0)
                            break;

                        //De laatste Reservatie opvragen die er bij gekomen is
                        Reservatie laatsReservatie = reservatiePool.Last();

                        //kijken heeft die genoeg stuks om het materiaal te kunnen reserveren
                        if (aantal <= laatsReservatie.Aantal)
                        {
                            //dit betekent dat er genoeg stuks waren in een reservatie.
                            laatsReservatie.ReservatieState.Overruul();

                            //nu gaan we kijken of er nog over zijn in de reservatie
                            int verschil = aantal - laatsReservatie.Aantal;

                            //Blijft er nog over dan wordt er een nieuwe reservatie gemaakt voor student
                            if (verschil > 0)
                            {
                                Student student = laatsReservatie.Gebruiker as Student;
                                IDictionary<Materiaal, int> nieuw = new Dictionary<Materiaal, int>();
                                nieuw.Add(laatsReservatie.Materiaal, verschil);

                                //Dit zou nooit moeten kunnen voorvallen
                                //Toch voor de zekerheid opvangen
                                if(student == null)
                                    throw new ArgumentNullException("Gebruiken is null");

                                student.maakReservaties(nieuw, laatsReservatie.StartDatum.ToShortDateString(), laatsReservatie.EindDatum.ToShortDateString());
                            }

                            //aantal wordt op nul gezet, want er zijn geen materialen meer te overrulen
                            aantal = 0;
                        }
                        else
                        {
                            //Nu worden volledige reservaties overrult
                            laatsReservatie.ReservatieState.Overruul();

                            //Nu moeten we nog berekenen wat er nog overblijft
                            aantal -= laatsReservatie.Aantal;

                            //De laatstereservatie moet nu uit de lijst met potentiele reservatie verwijdert worden
                            reservatiePool.Remove(laatsReservatie);
                        }

                        //Nu moet er nog een veiligheid in gebouwd worden zodat we nog uit de while lus geraken
                        //als aantal minder dan 0 is moet er niet meer overruult worden
                        if (aantal <= 0)
                        {
                            flag = true;
                        }
                    }
              
                    //Aanmaken van reservaties (overrulen betekend dat lector altijd zal kunnen reserveren)
                    VoegReservatieToe(mat, reserveerAantal, startDatum, eindDatum);
                }
            }

            //VerzendMailNaarLectorNaBlokkering(reservaties, startDatum, eindDatum);
        }

        //private void VerzendMailNaarLectorNaBlokkering(IDictionary<Materiaal, int> reservatiesOmTeBlokkeren, string startDatum, string eindDatum)
        //{
        //    MailMessage m = new MailMessage("projecten2groep6@gmail.com", this.Email);// hier nog gebruiker email pakken, nu testen of het werkt

        //    m.Subject = "Blokkering van reservatie";
        //    m.Body = string.Format("Dag {0} <br/>", this.Naam);
        //    m.IsBodyHtml = true;
        //    m.Body += "<p>U heeft zonet het volgende geblokkeerd: </p>";
        //    m.Body += "<ul>";
        //    foreach (var item in reservatiesOmTeBlokkeren)
        //    {
        //        m.Body += $"<li>{item.Value} x {item.Key.Naam}</li>";
        //    }
        //    m.Body += "</ul>";
        //    m.Body += "<br/>";
        //    m.Body += $"<p>De periode van blokkering is van {startDatum} tot {eindDatum}</p>";

        //    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        //    smtp.Credentials = new System.Net.NetworkCredential("projecten2groep6@gmail.com", "testenEmail");
        //    smtp.EnableSsl = true;
        //    smtp.Send(m);
        //}
    }
}