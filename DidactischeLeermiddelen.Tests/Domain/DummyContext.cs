using System;
using System.Collections.Generic;
using System.Linq;
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
        public Gebruiker Manu { get; set; }
        public Gebruiker LectorGebruiker { get; set; }
        public Gebruiker LectorGebruiker2 { get; set; }
        public Materiaal Bol { get; set; }
        public IQueryable<Materiaal> Materialen { get { return materialen.AsQueryable(); } }

        public Materiaal Encyclopedie { get; set; }

        public Materiaal Kaart { get; set; }
        public Materiaal GeoDriehoek { get; set; }
        public Reservatie ReservatieWeek1Aantal2 { get; set; }
        public Reservatie ReservatieWeek1Aantal8 { get; set; }
        public Reservatie ReservatieWeek1Aantal5 { get; set; }
        public Reservatie ReservatieWeek2Aantal2 { get; set; }
        public string StartDatum { get; set; }
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

            Manu = new Student()
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

            LectorGebruiker2 = new Lector()
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
            }; 
            Encyclopedie = new Materiaal("Encyclopedie", 111, 80)
            {
                IsReserveerBaar = true,
                MateriaalId = 1

            };
            GeoDriehoek = new Materiaal("GeoDriehoek", 111, 2)
            {
                IsReserveerBaar = true,
                MateriaalId = 2

            };
            StartDatum = "14/03/2016";
            ReservatieWeek1Aantal2 = new ReservatieStudent(Toon, Bol, StartDatum, 2);
            ReservatieWeek1Aantal8 = new ReservatieStudent(Toon, Bol, StartDatum, 8);
            ReservatieWeek1Aantal5 = new ReservatieStudent(Toon, Bol, StartDatum, 5);
            ReservatieWeek2Aantal2 = new ReservatieStudent(Toon, GeoDriehoek, StartDatum, 2);
            //Reservatie = new Reservatie();
            Student = new Student();
            materialen.Add(Bol);
            materialen.Add(Kaart);
            materialen.Add(Encyclopedie);


        }

    }
}
