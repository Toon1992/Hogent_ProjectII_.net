using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;
using Microsoft.Ajax.Utilities;

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
        public void VoegMateriaalToe(Materiaal materiaal)
        {
            if (!BevatMateriaal(materiaal))
            {
               
                    Materialen.Add(materiaal);
            }
            else
            {
                throw new ArgumentException("Het geselecteerde materiaal staat reeds in uw  verlanglijst");
            }
            //Toevoegen van materiaal
            
        }

        public void VerwijderMateriaal(Materiaal materiaal)
        {
            Materialen.Remove(materiaal);
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