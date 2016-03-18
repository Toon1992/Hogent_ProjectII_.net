//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Web.Mvc;
//using DidactischeLeermiddelen.Controllers;
//using DidactischeLeermiddelen.Models.Domain;
//using DidactischeLeermiddelen.Tests.Domain;
//using DidactischeLeermiddelen.ViewModels;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace DidactischeLeermiddelen.Tests.Controllers
//{
//    [TestClass]
//    public class VerlanglijstControllerTest
//    {
//        private VerlanglijstController verlanglijstController;
//        private ReservatieController reservatieController;
//        private Mock<IMateriaalRepository> mockMateriaalRepository;
//        private Mock<IGebruikerRepository> mockGebruikerRepository;
//        private Gebruiker student;
//        private Gebruiker lector;
//        private Materiaal m;
//        private DummyContext context;
//        private string[] dagenString;
//        private IList<DateTime> dagen;
//        private Reservatie reservatieLector;
//        private Reservatie reservatieStudent;
//        private int[] materiaalIds;
//        [TestInitialize]
//        public void OpzettenContext()
//        {
//            context = new DummyContext();
//            student = context.Toon;
//            lector = context.LectorGebruiker;
//            mockMateriaalRepository = new Mock<IMateriaalRepository>();
//            mockGebruikerRepository = new Mock<IGebruikerRepository>();
//            mockMateriaalRepository.Setup(t => t.FindAll()).Returns(context.Materialen);
//            mockMateriaalRepository.Setup(t => t.FindById(1)).Returns(context.Bol);
//            mockGebruikerRepository.Setup(t => t.FindByName("student@student.hogent.be")).Returns(context.Toon);
//            m = context.Bol;
//            materiaalIds = new []{ m.MateriaalId };
//            verlanglijstController = new VerlanglijstController(mockMateriaalRepository.Object, mockGebruikerRepository.Object);
//        }
//        [TestMethod]
//        public void ControleInvoerStudent8Beschikbaar5GeselecteerdReturnTrue()
//        {
//            Assert.AreEqual(10, m.AantalInCatalogus);
//            m.AddReservatie(context.ReservatieWeek1Aantal2Student);
//            int[] aantal = {5};
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(student, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsTrue(beschikbaar);
//            Assert.AreEqual(8, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, student));
//        }
//        [TestMethod]
//        public void ControleInvoerStudent0Beschikbaar5GeselecteerdReturnFalse()
//        {
//            m.AddReservatie(context.ReservatieWeek1Aantal2Student);
//            m.AddReservatie(context.ReservatieWeek1Aantal8Student);
//            int[] aantal = { 5 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(student, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsFalse(beschikbaar);
//            Assert.AreEqual(0, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, student));
//        }
//        [TestMethod]
//        public void ControleInvoerStudent5Beschikbaar8GeselecteerdReturnTrue()
//        {
//            m.AddReservatie(context.ReservatieWeek1Aantal5Student);
//            int[] aantal = { 8 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(student, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsFalse(beschikbaar);
//            Assert.AreEqual(5, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, student));
//        }
//        [TestMethod]
//        public void ControleInvoerStudent5Beschikbaar20GeselecteerdReturnFalse()
//        {
//            m.AddReservatie(context.ReservatieWeek1Aantal5Student);
//            int[] aantal = { 20 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(student, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsFalse(beschikbaar);
//            Assert.AreEqual(5, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, student));
//        }
//        [TestMethod]
//        public void ControleInvoerLector2Gereserveerd5GeselecteerdReturnTrue()
//        {
//            Assert.AreEqual(10, m.AantalInCatalogus);
//            m.AddReservatie(context.ReservatieWeek1Aantal2Student);
//            int[] aantal = { 5 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsTrue(beschikbaar);
//            Assert.AreEqual(10, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector2Gereserveerd50GeselecteerdReturnTrue()
//        {
//            Assert.AreEqual(10, m.AantalInCatalogus);
//            m.AddReservatie(context.ReservatieWeek1Aantal2Student);
//            int[] aantal = { 50 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsFalse(beschikbaar);
//            Assert.AreEqual(10, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector10Gereserveerd5GeselecteerdReturnTrue()
//        {
//            m.AddReservatie(context.ReservatieWeek1Aantal2Student);
//            m.AddReservatie(context.ReservatieWeek1Aantal8Student);
//            int[] aantal = { 5 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsTrue(beschikbaar);
//            Assert.AreEqual(10, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector5Beschikbaar8GeselecteerdReturnTrue()
//        {
//            m.AddReservatie(context.ReservatieWeek1Aantal5Student);
//            int[] aantal = { 8 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, null);
//            Assert.IsTrue(beschikbaar);
//            Assert.AreEqual(10, m.GeefAantalBeschikbaar(startDatum, eindDatum, null, lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector5Geblokkeerd2GeselecteerdZelfdeDataReturnTrue()
//        {
//            dagenString = new[] { "15/03/2016"};
//            MaakBlokkeringLector(dagenString, 5);

//            int[] aantal = { 2 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, dagen);
//            Assert.IsTrue(beschikbaar);
//            Assert.AreEqual(5, m.GeefAantalBeschikbaar(startDatum, eindDatum, dagen,lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector8Geblokkeerd2GeselecteerdZelfdeDataReturnTrue()
//        {
//            dagenString = new[] { "15/03/2016", "16/03/2016", "17/03/2016" };
//            MaakBlokkeringLector(dagenString, 8);

//            int[] aantal = { 2 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, dagen);
//            Assert.IsTrue(beschikbaar);
//            Assert.AreEqual(2, m.GeefAantalBeschikbaar(startDatum, eindDatum, dagen, lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector8Geblokkeerd8GeselecteerdZelfdeDataReturnFalse()
//        {
//            dagenString = new[] { "15/03/2016", "16/03/2016" };
//            MaakBlokkeringLector(dagenString, 8);

//            int[] aantal = { 8 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, dagen);
//            Assert.IsFalse(beschikbaar);
//            Assert.AreEqual(2, m.GeefAantalBeschikbaar(startDatum, eindDatum, dagen, lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector8Geblokkeerd8GeselecteerdOverschrijvendeDataReturnTrue()
//        {
//            dagenString = new[] { "15/03/2016", "16/03/2016" };
//            MaakBlokkeringLector(dagenString, 8);

//            dagenString = new[] { "16/03/2016", "18/03/2016" };
//            dagen = dagenString.Select(d => Convert.ToDateTime(d)).ToList();
//            int[] aantal = { 8 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//           // bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, dagen);
//            Assert.IsFalse(beschikbaar);
//            Assert.AreEqual(2, m.GeefAantalBeschikbaar(startDatum, eindDatum, dagen, lector));
//        }
//        [TestMethod]
//        public void ControleInvoerLector8Geblokkeerd8GeselecteerdVerschillendeDataReturnTrue()
//        {
//            dagenString = new[] { "15/03/2016", "16/03/2016" };
//            MaakBlokkeringLector(dagenString, 8);

//            dagenString = new[] { "17/03/2016", "18/03/2016", "21/03/2016" };
//            dagen = dagenString.Select(d => Convert.ToDateTime(d)).ToList();
//            int[] aantal = { 8 };
//            DateTime startDatum = Convert.ToDateTime(context.StartDatum);
//            DateTime eindDatum = startDatum.AddDays(4);
//            bool beschikbaar = verlanglijstController.ControleGeselecteerdMateriaal(lector, materiaalIds, aantal, startDatum, eindDatum, dagen);
//            Assert.IsTrue(beschikbaar);
//            Assert.AreEqual(10, m.GeefAantalBeschikbaar(startDatum, eindDatum, dagen, lector));
//        }
//        [TestMethod]
//        public void ReservatieDetails4Geblokeerd5Gereserveerd()
//        {
//            m.AddReservatie(context.ReservatieWeek1Aantal5Student);
//            dagenString = new[] { "15/03/2016", "16/03/2016" };
//            MaakBlokkeringLector(dagenString, 4);
//            int week = HulpMethode.GetIso8601WeekOfYear(Convert.ToDateTime(context.StartDatum));
//            PartialViewResult result = verlanglijstController.ReservatieDetails(student, m.MateriaalId, week) as PartialViewResult;
//            ReservatiesDetailViewModel vm = result.Model as ReservatiesDetailViewModel;
//            Materiaal materiaal = vm.Material;
//            IList<Reservatie> reservaties = materiaal.Reservaties;
//            Assert.AreEqual(2, materiaal.Reservaties.Count);
//            Assert.AreEqual(m.Reservaties, materiaal.Reservaties);
//        }
//        [TestMethod]
//        public void VerwijderMateriaalUitVerlanglijst()
//        {
//            student.VoegMateriaalAanVerlanglijstToe(m);
//            Assert.AreEqual(1, student.Verlanglijst.Materialen.Count);
//            Assert.AreEqual(m, student.Verlanglijst.Materialen.FirstOrDefault());
//            RedirectToRouteResult result = verlanglijstController.VerwijderUitVerlanglijst(m.MateriaalId, student) as RedirectToRouteResult;
//            Assert.AreEqual(0, student.Verlanglijst.Materialen.Count);
//            mockGebruikerRepository.Verify(m => m.SaveChanges(), Times.Once);
//            Assert.AreEqual("Index", result.RouteValues["Action"]);
//        }
//        private void MaakBlokkeringLector(string[] dagenString, int aantal)
//        {
//            dagen = dagenString.Select(d => Convert.ToDateTime(d)).ToList();
//            reservatieLector = new BlokkeringLector(context.LectorGebruiker, m, context.StartDatum, aantal, dagenString);
//            m.AddReservatie(reservatieLector);
//        }
//    }
//}
