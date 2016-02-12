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
        public void VoegMateriaalAanVerlangLijstMetMateriaalObject()
        {
            Materiaal materiaal = new Materiaal("WereldBol",25,10);
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal,1);
            Assert.AreEqual(1,context.Toon.Verlanglijst.Materialen.Count);
        }



        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void VoegMateriaalNullToe()
        {
            context.Toon.VoegMateriaalAanVerlanglijstToe(null, 1);
        }

        [TestMethod]
        public void VoegMateriaalToeDatAlInVerlanglijstStaat()
        {
            Materiaal materiaal = new Materiaal("WereldBol", 25, 10);
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal, 1);
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal, 1);
            Assert.AreEqual(1, context.Toon.Verlanglijst.Materialen.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VoegMateriaalToeAanVerlanglijstMetAantalHogerDanAantalInCatalogus()
        {
            Materiaal materiaal = new Materiaal("WereldBol", 25, 10);
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal, 11);
        }
    }
}
