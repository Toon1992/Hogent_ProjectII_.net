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
        public void BevatMateriaalAanroepenZonderDatMateriaalAanwezigIs()
        {
            context.Toon.Verlanglijst.VoegMateriaalToe(context.Bol, 2);
            Materiaal materiaal = new Materiaal("Test",123,3);
            Assert.IsFalse(context.Toon.Verlanglijst.BevatMateriaal(materiaal));
        }
        [TestMethod]
        public void GeefAantalMateriaalInVerlanglijstGeeftJuisteAantal()
        {
            context.Toon.Verlanglijst.VoegMateriaalToe(context.Bol, 2);
            Assert.AreEqual(2, context.Toon.Verlanglijst.GeefAantalMateriaalInVerlanglijst(context.Bol));
        }
    }
}
