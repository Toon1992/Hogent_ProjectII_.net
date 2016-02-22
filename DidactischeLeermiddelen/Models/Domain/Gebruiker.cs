using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
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

        public void VoegReservatieToe(IList<Materiaal> materiaal, int[] aantal, int week)
        {
            materiaal.ForEach(m =>
            {
                foreach (var a in aantal)
                {
                    Reservatie reservatie = new Reservatie();
                    reservatie.MaakReservatie(m, a, week);
                    Reservaties.Add(reservatie);
                }                          
            });

            //Reservatie reservatie = new Reservatie();
            //reservatie.MaakReservatie(materiaal, startDatum);
            //Reservaties.Add(reservatie);
        }
    }
}