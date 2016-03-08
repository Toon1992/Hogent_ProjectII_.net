using System;
using DidactischeLeermiddelen.Controllers;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Tests.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DidactischeLeermiddelen.Tests.Controllers
{
    [TestClass]
    public class ReservatieControllerTest
    {
        private Mock<IMateriaalRepository> mockMateriaalRepository;
        private Mock<IGebruikerRepository> mockGebruikerRepository;
        private Mock<IReservatieRepository> mockReservatieRepository;
        private ReservatieController controller;
        private Gebruiker gebruiker;
        private Materiaal m;

        public void OpzettenContext()
        {
            DummyContext context = new DummyContext();
            gebruiker = context.Toon;
            mockMateriaalRepository = new Mock<IMateriaalRepository>();
            mockGebruikerRepository = new Mock<IGebruikerRepository>();
            mockReservatieRepository = new Mock<IReservatieRepository>();
            mockMateriaalRepository.Setup(t => t.FindAll()).Returns(context.Materialen);
            m = context.Encyclopedie;

            controller = new ReservatieController(mockMateriaalRepository.Object, mockGebruikerRepository.Object, mockReservatieRepository.Object);

        }



    }
}
