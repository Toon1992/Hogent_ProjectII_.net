using System;
using System.Collections.Generic;
using DidactischeLeermiddelen.Models.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DidactischeLeermiddelen.Tests.Domain
{
    [TestClass]
    public class ReservatieTest
    {
        private DummyContext context;

        [TestInitialize]
        public void Initialize()
        {
            context = new DummyContext();
        }

        [TestMethod]
        public void MaakJuistReservatie()
        {
            Materiaal materiaal =new Materiaal("Test", 123, 3);
            materiaal.Stuks = new List<Stuk>() {new Stuk() {HuidigeStatus = Status.Beschikbaar} };
            int[] aantal = new[] {1};
           context.Toon.VoegReservatieToe(new List<Materiaal>() { materiaal }, aantal,5,context.Toon );
            Assert.AreEqual(1,context.Toon.Reservaties.Count);                  
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MaakReservatieMetMeerAantalenDanMaterialen()
        {
            Materiaal materiaal = new Materiaal("Test", 123, 3);
            materiaal.Stuks = new List<Stuk>() { new Stuk() { HuidigeStatus = Status.Beschikbaar } };
            int[] aantal = new[] { 1,1 };
            context.Toon.VoegReservatieToe(new List<Materiaal>() { materiaal }, aantal, 5,context.Toon);
            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MaakReservatieMetNullMateriaal()
        {
            Materiaal materiaal = null;
            Reservatie reservatie = new Reservatie();
            reservatie.MaakReservatie(materiaal,1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MaakReservatieMetMinderdanNulWeken()
        {
            Materiaal materiaal = new Materiaal("Test", 123, 3);
            Reservatie reservatie = new Reservatie();
            reservatie.MaakReservatie(materiaal, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MaakReservatieOpWeeknul()
        {
            Materiaal materiaal = new Materiaal("Test", 123, 3);
            Reservatie reservatie = new Reservatie();
            reservatie.MaakReservatie(materiaal, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MaakReservatieZonderStuksLijst()
        {
            Materiaal materiaal = new Materiaal("Test", 123, 3);
            Reservatie reservatie = new Reservatie();
            reservatie.MaakReservatie(materiaal, 2);
        }

        [TestMethod]      
        public void ErMagGeenReservatieGemaaktworden()
        {
            Materiaal materiaal = new Materiaal("Test", 123, 3);
            materiaal.Stuks = new List<Stuk>() {new Stuk() {HuidigeStatus = Status.Gereserveerd} };
            Reservatie reservatie = new Reservatie();
            reservatie.MaakReservatie(materiaal, 2);

            Assert.IsFalse(reservatie.MaakReservatie(materiaal, 2));
        }
    }
}
