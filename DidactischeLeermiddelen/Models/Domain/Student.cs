using System;
using System.Collections.Generic;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Student : Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public void MaakReservaties(IDictionary<Materiaal, int> potentieleReservaties, string startDatum)
        {
            try
            {
                foreach (KeyValuePair<Materiaal, int> potentiele in potentieleReservaties)
                {
                    VoegReservatieToe(potentiele.Key, potentiele.Value, startDatum);
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Iets fout gelopen hier");
            }
        }

        protected override void VoegReservatieToe(Materiaal materiaal, int aantal, string startdatum,string[] dagen =null)
        {
            Reservatie reservatie = MaakReservatieObject(this, materiaal, startdatum, aantal);
            materiaal.AddReservatie(reservatie);
            Reservaties.Add(reservatie);
        }

        protected override Reservatie MaakReservatieObject(Gebruiker gebruiker, Materiaal mat, string startdatum,int aantal, string[] dagen = null)
        {
            Reservatie reservatie = new ReservatieStudent(gebruiker, mat, startdatum, aantal);
           
            if (reservatie == null)
            {
                throw new ArgumentNullException("Er is geen reservatie Object gemaakt");
            }

            return reservatie;
        }
        public override string GeefBeschikbaarheid(DateTime startDatum, DateTime eindDaum, IList<DateTime> dagen, Materiaal materiaal)
        {
            return $"Niet meer beschikbaar van {HulpMethode.DateToString(startDatum)} tot {HulpMethode.DateToString(eindDaum)}";
        }
    }
}