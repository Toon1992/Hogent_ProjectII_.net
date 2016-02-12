using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Tests.Domain
{
    class DummyContext
    {
       public Gebruiker Toon { get; set; }

        public DummyContext()
        {
            Toon = new Gebruiker()
            {
                Email = "toon.de.true@hotmail.be",
                GebruikersId = 1000000,
                Naam = "Toon"
            };


        }
    }
}
