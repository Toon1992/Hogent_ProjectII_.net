using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Verlanglijst
    {
        #region fields
        public List<Materiaal> Materialen { get; set; }
        #endregion

        #region Methodes

        public Verlanglijst()
        {
            Materialen = new List<Materiaal>();
        }
        public void VoegMateriaalToe(Materiaal materiaal)
        {
            //Materiaal dat doorgegeven wordt mag niet null zijn
            if(materiaal == null)
                throw new ArgumentNullException("Het materiaal dat aan de verlanglijst wou worden gegeven is null!");
            //Materiaal mag nog niet voorkomen in verlanglijst van de gebruiker
            if (BevatMateriaal(materiaal))
                throw new ArgumentException("Dit materiaal werd reeds toegevoegd aan de verlanglijst!");
           
                Materialen.Add(materiaal);
            
            
            
            //Toevoegen van materiaal
            
        }

        public Boolean BevatMateriaal(Materiaal materiaal)
        {
            if(Materialen.Count == 0)
                return false;

            return Materialen.Contains(materiaal);
        }
        #endregion 
    }
}