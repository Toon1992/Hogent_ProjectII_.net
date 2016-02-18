using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Gebruiker
    {
        #region fields
        public string Naam { get; set; }

        public bool IsLector { get; set; }

        public string Email { get; set; }

        public virtual List<Reservatie> Reservaties { get; set; }

        public virtual Verlanglijst Verlanglijst { get; set; }
        #endregion

        #region Methodes

        public Gebruiker()
        {
        }

        public void VoegMateriaalAanVerlanglijstToe(Materiaal materiaal)
        {
            if(materiaal == null)
                throw new ArgumentNullException("Materiaal mag niet null zijn als die wordt toevoegd aan de verlanglijst!");

            //aan de Velanglijst materiaal Toevoegen      
            Verlanglijst.VoegMateriaalToe(materiaal);
            
        }

        public void VerwijderMateriaalUitVerlanglijst(Materiaal materiaal)
        {
            if (materiaal == null)
                throw new ArgumentNullException("Materiaal mag niet null zijn als die wordt verwijdert van de verlanglijst!");

            //Verwijderen van materiaal van de verlanglijst
            Verlanglijst.VerwijderMateriaal(materiaal);
        }

        
        public void VoegReservatieToe(Reservatie reservatie)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}