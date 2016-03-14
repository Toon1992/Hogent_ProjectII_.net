using System;
using System.Collections.Generic;
using System.Linq;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Lector : Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public void MaakBlokkeringen(IDictionary<Materiaal, int> potentieleReservaties, string startDatum)
        {
            //Het converten van string naar DateTime
            DateTime start = Convert.ToDateTime(startDatum);
            DateTime einde = Convert.ToDateTime(HulpMethode.GetEindDatum(startDatum));

            //Overlopen van map met potentiele reserveringen/blokkeringen/overrulingen
            foreach (KeyValuePair<Materiaal, int> potentiele in potentieleReservaties)
            {
                //Aantal Lokale variabele aanmaken die we nodig hebben
                Materiaal mat = potentiele.Key;
                int reserveerAantal = potentiele.Value;

                //opvragen van het aantal reservaties die niet geblokkeerd, opgehaald of overruult zijn
                int aantalBeschikbaar = mat.GeefAantalBeschikbaarVoorBlokkering();

                //Eerst gaan we kijken of er nog genoeg beschikbaar zijn om gwn te reserveren
                //we vergelijken de aantal beschikbare stuks voor het materiaal met het aantal dat we nodig hebben voor onze reservatie
                //Zo ja maken we gwn reservaties (lectoren blokkeren altijd!!)
                //Zo niet gaan we over tot het overrulen van reservaties
                if (aantalBeschikbaar >= reserveerAantal)
                {
                    //Aanmaken van reservaties
                    VoegReservatieToe(mat, reserveerAantal, startDatum);
                }
                else
                {
                    //Overrulen
                    BerekenenOverrulen(mat, reserveerAantal, aantalBeschikbaar, start);

                    //Aanmaken van reservaties (overrulen betekend dat lector altijd zal kunnen reserveren)
                    VoegReservatieToe(mat, reserveerAantal, startDatum);
                }
            }


        }

        private void BerekenenOverrulen(Materiaal mat, int reserveerAantal, int aantalBeschikbaar, DateTime start)
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
                    OverrulenVanReservatie(laatsReservatie);

                    //nu gaan we kijken of er nog over zijn in de reservatie
                    int verschil = laatsReservatie.Aantal - aantal;

                    ////Originele aantal wordt vermindert van de laatste reservatie
                    laatsReservatie.Aantal -= verschil;

                    //Blijft er nog over dan wordt er een nieuwe reservatie gemaakt voor student
                    if (verschil > 0)
                    {
                        MaakNieuweReservatieVoorStudent(laatsReservatie, verschil);
                    }

                    //aantal wordt op nul gezet, want er zijn geen materialen meer te overrulen
                    aantal = 0;
                }
                else
                {
                    //overrulen van de reservatie
                    OverrulenVanReservatie(laatsReservatie);

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
        }

        private void OverrulenVanReservatie(Reservatie laatsReservatie)
        {
            //Nu worden volledige reservaties overrult
            laatsReservatie.ReservatieState.Overruul();
        }

        private void MaakNieuweReservatieVoorStudent(Reservatie laatsReservatie, int verschil)
        {
            Student student = laatsReservatie.Gebruiker as Student;

            //Dit zou nooit moeten kunnen voorvallen
            //Toch voor de zekerheid opvangen
            if (student == null)
                throw new ArgumentNullException("Gebruiken is null");

            IDictionary<Materiaal, int> nieuw = new Dictionary<Materiaal, int>();
            nieuw.Add(laatsReservatie.Materiaal, verschil);

            student.MaakReservaties(nieuw, laatsReservatie.StartDatum.ToShortDateString());
        }

        protected override void VoegReservatieToe(Materiaal materiaal, int aantal, string startdatum)
        {
            Reservatie reservatie = MaakReservatieObject(this, materiaal, startdatum, aantal);
            reservatie.Blokkeer();
            materiaal.AddReservatie(reservatie);
            Reservaties.Add(reservatie);
        }

        protected override Reservatie MaakReservatieObject(Gebruiker gebruiker, Materiaal mat, string startdatum,int aantal)
        {
            Reservatie reservatie = new BlokkeringLector(gebruiker,mat,startdatum,aantal);
           
            if (reservatie == null)
            {
                throw new ArgumentNullException("Er is geen reservatie Object gemaakt");
            }

            return reservatie;
        }
    }
}