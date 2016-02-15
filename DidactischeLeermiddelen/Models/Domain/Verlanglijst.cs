using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Verlanglijst
    {
        #region fields
        public virtual List<Materiaal> Materialen { get; set; }
        public long Id { get; set; }
        #endregion

        #region Methodes

        public Verlanglijst()
        {
            Materialen = new List<Materiaal>();
        }
        public void VoegMateriaalToe(Materiaal materiaal, int aantal)
        {
            //Materiaal dat doorgegeven wordt mag niet null zijn
            if(materiaal == null)
                throw new ArgumentNullException("Het materiaal dat aan de verlanglijst wou worden gegeven is null!");
            //Materiaal mag nog niet voorkomen in verlanglijst van de gebruiker
            if (aantal > materiaal.AantalInCatalogus)
                throw new ArgumentException("Het opgegeven aantal is te groot, gelieve een aantal te kiezen tussen 1 en het aantal in de catalogus");
            if (!BevatMateriaal(materiaal))
            {
                for (int i = 0; i < aantal; i++)
                {
                    Materialen.Add(materiaal);
                }
            }
            else
            {
                throw new ArgumentException("Het geselecteerde materiaal staat reeds in uw  verlanglijst");
            }
            //Toevoegen van materiaal
            
        }

        public Boolean BevatMateriaal(Materiaal materiaal)
        {
            if(Materialen.Count == 0)
                return false;

            return Materialen.Contains(materiaal);
        }
        #endregion

        public int GeefAantalMateriaalInVerlanglijst(Materiaal materiaal)
        {
            return Materialen.Count(m => m.ArtikelNr == materiaal.ArtikelNr);
        }
    }
}