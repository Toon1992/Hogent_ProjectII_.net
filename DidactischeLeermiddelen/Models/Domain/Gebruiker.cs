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
           
            //Heeft verlanglijst al dit materiaal in zijn lijst staan?
            //Ja, niks doen
            //Nee, toevoegen aan de lijst
            
            
                Verlanglijst.VoegMateriaalToe(materiaal);
            
        }

        public void VerwijderMateriaalUitVerlanglijst(Materiaal materiaal)
        {
            Verlanglijst.VerwijderMateriaal(materiaal);
        }

        
        public void VoegReservatieToe(Reservatie reservatie)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}