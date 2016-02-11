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
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal);
            Assert.AreEqual(1,context.Toon.Verlanglijst.Materialen.Count);
        }

        [TestMethod]
        public void VoegMateriaalAanVerlangLijstMetvariabele()
        {          
            context.Toon.VoegMateriaalAanVerlanglijstToe(null,"Wereldbol",null,10,25,2.60M,null,null,null);
            Assert.AreEqual(1, context.Toon.Verlanglijst.Materialen.Count);
        }
    }
}
