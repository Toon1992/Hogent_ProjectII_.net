using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Lector : Gebruiker
    {
        public IList<Reservatie> OverruledeReservaties { get; set; } 

        public void MaakBlokkeringen(IDictionary<Materiaal, int> potentieleReservaties, string startDatum, string[] dagen)
        {
            //Het converten van string naar DateTime
            DateTime start = HulpMethode.GetStartDatum(startDatum);
            DateTime einde = HulpMethode.GetEindDatum(startDatum);
            OverruledeReservaties=new List<Reservatie>();
            IDictionary<DateTime, IList<string>> dagenGeblokkeerd = verdeelDagenOverWeken(dagen);

            foreach (var pair in dagenGeblokkeerd)
            {
                string startDate = HulpMethode.DateToString(pair.Key);
                string[] geblokkeerdeDagen = pair.Value.ToArray();

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
                        VoegReservatieToe(mat, reserveerAantal, startDate, geblokkeerdeDagen);

                    }
                    else
                    {
                        //Overrulen
                        BerekenenOverrulenEnBlokkeren(mat, reserveerAantal, aantalBeschikbaar, start, geblokkeerdeDagen);

                        //Aanmaken van reservaties (overrulen betekend dat lector altijd zal kunnen reserveren)
                        VoegReservatieToe(mat, reserveerAantal, startDate, geblokkeerdeDagen);
                    }
                }
            }

        }

        private void BerekenenOverrulenEnBlokkeren(Materiaal mat, int reserveerAantal, int aantalBeschikbaar, DateTime start, string[] geblokkeerdeDagen)
        {
            //Hier berekenen we hoeveel stuks we nog moeten Overrulen
            int aantal = reserveerAantal - aantalBeschikbaar;

            List<Reservatie> reservatiePool = new List<Reservatie>();

            //We vragen alle reservaties op van het materiaal tijdens de week dat we een reservatie willen doen
            reservatiePool = mat.GeeftReservatiesVanEenBepaaldeTijd(start) as List<Reservatie>;

            reservatiePool.Sort((r1, r2) => r1.CompareTo(r2));

            //We gaan een while lus willen starten omdat het kan zijn dat er verschillende gebruikers hun reservatie kwijtgeraken
            bool flag = false;

            while (!flag)
            {
                //Eerst kijken of er nog reservaties in de pool zitten die we gaan kunnen overrulen 
                if (reservatiePool.Count == 0)
                    break;

                //De laatste Reservatie opvragen die er bij gekomen is
                Reservatie laatsteReservatie = reservatiePool.Last();
                            
                    //kijken heeft die genoeg stuks om het materiaal te kunnen reserveren
                    if (aantal <= laatsteReservatie.Aantal)
                    {
                        //nu gaan we kijken of er nog over zijn in de reservatie
                        int verschil = laatsteReservatie.Aantal - aantal;

                        ////Originele aantal wordt vermindert van de laatste reservatie
                        laatsteReservatie.Aantal -= verschil;

                    if (!(laatsteReservatie is BlokkeringLector))
                    {
                        //Blijft er nog over dan wordt er een nieuwe reservatie gemaakt voor student
                        if (verschil > 0)
                        {
                            MaakNieuweReservatieVoorStudent(laatsteReservatie, verschil);
                        }
                        OverrulenVanReservatie(laatsteReservatie);
                        OverruledeReservaties.Add(laatsteReservatie);
                    }

                        //aantal wordt op nul gezet, want er zijn geen materialen meer te overrulen
                        aantal = 0;
                    }
                    else
                    {
                    if (!(laatsteReservatie is BlokkeringLector))
                    {
                        //overrulen van de reservatie
                        OverrulenVanReservatie(laatsteReservatie);
                        OverruledeReservaties.Add(laatsteReservatie);
                    }

                        //Nu moeten we nog berekenen wat er nog overblijft
                        aantal -= laatsteReservatie.Aantal;

                        //De laatstereservatie moet nu uit de lijst met potentiele reservatie verwijdert worden
                        reservatiePool.Remove(laatsteReservatie);
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
            laatsReservatie.Overruul();
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

        protected override void VoegReservatieToe(Materiaal materiaal, int aantal, string startdatum, string[] dagen = null)
        {
            Reservatie reservatie = MaakReservatieObject(this, materiaal, startdatum, aantal, dagen);
            reservatie.Blokkeer();
            materiaal.AddReservatie(reservatie);
            Reservaties.Add(reservatie);
        }

        protected override Reservatie MaakReservatieObject(Gebruiker gebruiker, Materiaal mat, string startdatum, int aantal, string[] dagen = null)
        {
            Reservatie reservatie = new BlokkeringLector(gebruiker, mat, startdatum, aantal, dagen);
            return reservatie;
        }

        private IDictionary<DateTime, IList<string>> verdeelDagenOverWeken(string[] dagen)
        {
            IDictionary<DateTime, IList<string>> dagenGeblokkeerd = new Dictionary<DateTime, IList<string>>();

            foreach (var dag in dagen)
            {
                DateTime startDatum = HulpMethode.GetStartDatum(dag);

                if (!dagenGeblokkeerd.ContainsKey(startDatum))
                {
                    IList<string> dagenInWeek = new List<string>();
                    dagenInWeek.Add(dag);
                    dagenGeblokkeerd.Add(startDatum, dagenInWeek);
                }
                else
                {
                    IList<string> dagenInWeek = dagenGeblokkeerd[startDatum];
                    dagenInWeek.Add(dag);
                    dagenGeblokkeerd[startDatum] = dagenInWeek;
                }
            }

            return dagenGeblokkeerd;
        }

        public override string GeefBeschikbaarheid(DateTime startDatum, DateTime eindDaum, IList<DateTime> dagen, Materiaal materiaal)
        {
            if (dagen != null)
            {
                var geblokeerdeDagen =
                    materiaal.Reservaties
                        .Where(r => startDatum <= r.EindDatum && eindDaum >= r.StartDatum)
                        .SelectMany(r => r.GeblokkeerdeDagen.Select(d => d.Datum))
                        .Intersect(dagen);
                return $"Niet meer beschikbaar op {HulpMethode.DatesToString(geblokeerdeDagen)}";
            }
            return "";
        }
    
    }
}