using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Student : IGebruiker
    {
        public string Email { get; set; }
        public string Naam { get; set; }
        public Verlanglijst Verlanglijst { get; set; }
        public IList<Reservatie> Reservaties { get; set; }

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


        public void VoegReservatieToe(Materiaal materiaal, DateTime startDatum)
        {
            Reservatie reservatie = new Reservatie();
            reservatie.MaakReservatie(materiaal, startDatum);
            Reservaties.Add(reservatie);
        }
    }
}