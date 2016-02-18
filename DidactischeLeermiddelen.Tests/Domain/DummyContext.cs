using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Tests.Domain
{
    public class DummyContext
    {
        private IList<Gebruiker> gebruikers;
        private IList<Materiaal> materialen;
        private IList<Doelgroep> doelgroepen;
        private IList<Leergebied> leergebieden;

        public Gebruiker Toon { get; set; }
        public Materiaal Bol { get; set; }


        public DummyContext()
        {
            gebruikers = new List<Gebruiker>();
            materialen = new List<Materiaal>();
            doelgroepen = new List<Doelgroep>();
            leergebieden = new List<Leergebied>();

            Toon = new Gebruiker()
            {
                Email = "toon.de.true@hotmail.be",
                //GebruikersId = "1000000",
                Naam = "Toon",
                Verlanglijst =  new Verlanglijst(),
                IsLector = false,
                Reservaties = new List<Reservatie>()
               
            };

            Bol=new Materiaal("Wereldbol",456,10);


        }
    }
}
