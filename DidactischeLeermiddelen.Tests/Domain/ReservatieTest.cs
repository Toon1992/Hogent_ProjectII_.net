using System;
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
            //context.Toon.VoegReservatieToe(materiaal,DateTime.Now);
            Assert.AreEqual(1,context.Toon.Reservaties.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MaakReservatieMetNullMateriaal()
        {
            Materiaal materiaal = null;
           // context.Toon.VoegReservatieToe(materiaal, DateTime.Now);
        }
    }
}
