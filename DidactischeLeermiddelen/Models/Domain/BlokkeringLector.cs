using System;
using System.Collections.Generic;
using System.Linq;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class BlokkeringLector : Reservatie
    {     
        public BlokkeringLector(Gebruiker gebruiker, Materiaal materiaal, string startDatum, int aantal,string[]dagen)
            : base(gebruiker, materiaal, startDatum, aantal)
        {
            ArrayNaarListDagen(dagen);
        }

        private void ArrayNaarListDagen(string[] dagen)
        {
            foreach (var dag in dagen)
            {
                GeblokkeerdeDagen.Add(new Dag() {Datum = Convert.ToDateTime(dag)});
            }
        }
    }
}