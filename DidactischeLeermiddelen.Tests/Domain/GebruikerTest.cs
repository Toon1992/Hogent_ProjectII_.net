using System;
using DidactischeLeermiddelen.Models.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DidactischeLeermiddelen.Tests.Domain
{
    [TestClass]
    public class GebruikerTest
    {
        private DummyContext context;
        [TestInitialize]
        public void Initialize()
        {
            context = new DummyContext();
        }

        [TestMethod]
        public void VerwijderReservatieVerwijdertToegevoegde()
        {
            Reservatie r = new ReservatieStudent();
            context.Toon.Reservaties.Add(r);
            context.Toon.VerwijderReservatie(r);
            Assert.AreEqual(0, context.Toon.Reservaties.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerwijderReservatieGeeftFoutWanneerGeenReservaties()
        {
            
            context.Toon.VerwijderReservatie(new ReservatieStudent());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VerwijderMateriaalUitVerlanglijstGeeftFoutBijMateriaalNull()
        {

            context.Toon.VerwijderMateriaalUitVerlanglijst(null);
        }


    }
}
