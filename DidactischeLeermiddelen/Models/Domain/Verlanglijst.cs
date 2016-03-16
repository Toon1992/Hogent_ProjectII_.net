using System;
using System.Collections.Generic;

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
            if(!BevatMateriaal(materiaal))
                throw new ArgumentException("Er bevindt zich niks in de materiaal lijst");

            if (Materialen.Contains(materiaal))
            {
                Materialen.Remove(materiaal);
            }
            else
            {
                throw new ArgumentException("Materiaal bevindt zich niet in de lijst");
            }
        }

        public bool BevatMateriaal(Materiaal materiaal)
        {
            if(Materialen.Count == 0)
                return false;

            return Materialen.Contains(materiaal);
        }
        #endregion

       
    }
}