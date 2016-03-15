using System.Web.Mvc;
using DidactischeLeermiddelen.Controllers;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Tests.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DidactischeLeermiddelen.Tests.Controllers
{
    [TestClass]
    public class VerlanglijstControllerTest
    {
        private VerlanglijstController controller;
        private Mock<IMateriaalRepository> mockMateriaalRepository;
        private Mock<IGebruikerRepository> mockGebruikerRepository;
        private Gebruiker gebruiker;
        private Materiaal m;
        [TestInitialize]
        public void OpzettenContext()
        {
            DummyContext context = new DummyContext();
            gebruiker = context.Toon;
            mockMateriaalRepository = new Mock<IMateriaalRepository>();
            mockGebruikerRepository = new Mock<IGebruikerRepository>();
            mockMateriaalRepository.Setup(t => t.FindAll()).Returns(context.Materialen);
            mockMateriaalRepository.Setup(t => t.FindById(1)).Returns(context.Encyclopedie);
            mockGebruikerRepository.Setup(t => t.FindByName("student@student.hogent.be")).Returns(context.Toon);

            m = context.Encyclopedie;

            controller = new VerlanglijstController(mockMateriaalRepository.Object, mockGebruikerRepository.Object);

        }

        //[TestMethod]
        //public void ReservatieDetailsGrafiekGeeftJsonTerug()
        //{
        //    var result = controller.ReservatieDetailsGrafiek(2, 1) as JsonResult;
        //    var data = result.Data;
        //    var type = data.GetType();
        //    var countPropertyInfo = type.GetProperty("Count");
        //    var expectedCount = countPropertyInfo.GetValue(data, null);
        //    Assert.AreEqual(4, expectedCount);
        //}
    }
}
