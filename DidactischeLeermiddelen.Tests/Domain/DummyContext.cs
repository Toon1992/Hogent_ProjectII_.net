using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
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
        public Gebruiker LectorGebruiker { get; set; }
        public Materiaal Bol { get; set; }
        public IQueryable<Materiaal> Materialen { get { return materialen.AsQueryable(); } }

        public Materiaal Encyclopedie { get; set; }

        public Materiaal Kaart { get; set; }
        public Reservatie Reservatie { get; set; }
        public Student Student { get; set; }


        public DummyContext()
        {
            gebruikers = new List<Gebruiker>();
            materialen = new List<Materiaal>();
            doelgroepen = new List<Doelgroep>();
            leergebieden = new List<Leergebied>();

            Toon = new Student()
            {
                Email = "student@student.hogent.be",
                //GebruikersId = "1000000",
                Naam = "Toon",
                Verlanglijst = new Verlanglijst(),
                Reservaties = new List<Reservatie>()

            };

            LectorGebruiker = new Lector()
            {
                Email = "lector@hogent.be",
                //GebruikersId = "1000000",
                Naam = "lector",
                Verlanglijst = new Verlanglijst(),
                Reservaties = new List<Reservatie>()

            };

            Bol = new Materiaal("Wereldbol", 456, 10)
            {
                IsReserveerBaar = true
            };
            Kaart = new Materiaal("Kaart", 4587, 5)
            {
                IsReserveerBaar = true
            }; ;
            Encyclopedie = new Materiaal("Encyclopedie", 111, 80)
            {
                IsReserveerBaar = true,
                MateriaalId = 2

            }; ;
            Reservatie = new Reservatie();
            Student = new Student();
            materialen.Add(Bol);
            materialen.Add(Kaart);
            materialen.Add(Encyclopedie);


        }

    }
}
