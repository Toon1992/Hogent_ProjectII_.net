using DidactischeLeermiddelen.Models.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidactischeLeermiddelen.Tests.Domain
{
    [TestClass]
    public class VerlanglijstTest
    {
        private DummyContext context;
        [TestInitialize]
        public void Initialize()
        {
            context = new DummyContext();
        }

        [TestMethod]
        public void BevatMateriaalAanroepenZonderMaterialenGeeftFalse()
        {
            Assert.IsFalse(context.Toon.Verlanglijst.BevatMateriaal(context.Bol));
        }

        [TestMethod]
        public void BevatMateriaalAanroepenZonderDatMateriaalAanwezigIsGeeftFalse()
        {
            context.Toon.Verlanglijst.VoegMateriaalToe(context.Bol);
            Materiaal materiaal = new Materiaal("Test",123,3);
            Assert.IsFalse(context.Toon.Verlanglijst.BevatMateriaal(materiaal));
        }

        public void BevatMateriaalAanroepenMetMateriaalAanwezigGeeftTrue()
        {
            context.Toon.Verlanglijst.VoegMateriaalToe(context.Bol);
            
            Assert.IsTrue(context.Toon.Verlanglijst.BevatMateriaal(context.Bol));
        }

        [TestMethod]
        public void VoegMateriaalAanVerlangLijstMetMateriaalObjectVoegtObjectToe()
        {
            Materiaal materiaal = new Materiaal("WereldBol", 25, 10);
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal);
            Assert.AreEqual(1, context.Toon.Verlanglijst.Materialen.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VoegMateriaalToeDatAlInVerlanglijstStaat()
        {
            Materiaal materiaal = new Materiaal("WereldBol", 25, 10);
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal);
            context.Toon.VoegMateriaalAanVerlanglijstToe(materiaal);
            Assert.AreEqual(1, context.Toon.Verlanglijst.Materialen.Count);
        }
    }
}
