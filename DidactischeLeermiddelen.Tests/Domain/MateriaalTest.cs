using DidactischeLeermiddelen.Models.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DidactischeLeermiddelen.Tests.Domain
{
    [TestClass]
    public class MateriaalTest
    {
        private DummyContext context;
        [TestInitialize]
        public void Initialize()
        {
            context = new DummyContext();
        }

        [TestMethod]
        public void AddReservatieVoegtReservatieToe()
        {          
            context.Bol.AddReservatie(new ReservatieStudent());
            Assert.AreEqual(context.Bol.Reservaties.Count, 1);
        }

        
    }
}
