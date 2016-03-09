using System;
using System.Collections.Generic;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
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
        public void StudentMaakReservatie()
        {
            Student student = context.Toon as Student;
            IDictionary<Materiaal,int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol,5);
            student.maakReservaties(materiaalMap,"23/3/2016","28/3/2016");
            Assert.AreEqual(1,student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties.First().ReservatieState is Gereserveerd);
            Assert.AreEqual(5,student.Reservaties.First().Aantal);
        }

        [TestMethod]
        public void StudentMaakReservatieMeerdereMaterialen()
        {
            Student student = context.Toon as Student;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            materiaalMap.Add(context.Encyclopedie, 2);
            student.maakReservaties(materiaalMap, "23/3/2016", "28/3/2016");
            Assert.AreEqual(2, student.Reservaties.Count);
        }

        [TestMethod]
        public void StudentMaakReservatieMaxAantal()
        {
            Student student = context.Toon as Student;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 10);
            student.maakReservaties(materiaalMap, "23/3/2016", "28/3/2016");
            Assert.AreEqual(1, student.Reservaties.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void StudentMaakReservatieMetTweeKeerZelfdeMateriaal()
        {
            Student student = context.Toon as Student;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            materiaalMap.Add(context.Bol, 2);
            student.maakReservaties(materiaalMap, "23/3/2016", "28/3/2016");
        }

        [TestMethod]
        public void LectorMaakReservatie()
        {
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            lector.MaakBlokkeringen(materiaalMap, "23/3/2016", "28/3/2016");
            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(5, lector.Reservaties.First().Aantal);
        }

        [TestMethod]
        public void LectorMaakBlokkeringWaarStudentAlHeeftGereserveerdMaarErIsNogGenoegOver()
        {
            Student student = context.Toon as Student;
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            student.maakReservaties(materiaalMap, "23/3/2016", "28/3/2016");
            lector.MaakBlokkeringen(materiaalMap, "23/3/2016", "28/3/2016");

            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(5, lector.Reservaties.First().Aantal);
            Assert.AreEqual(1, student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties.First().ReservatieState is Gereserveerd);
            Assert.AreEqual(5, student.Reservaties.First().Aantal);
        }

        [TestMethod]
        public void LectorMaakBlokkeringWaarStudentAlHeeftGereserveerdWaarBlokkeringNodigIs()
        {
            Student student = context.Toon as Student;
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            IDictionary<Materiaal, int> materiaalLectorMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            materiaalLectorMap.Add(context.Bol, 6);
            student.maakReservaties(materiaalMap, "23/3/2016", "28/3/2016");
            lector.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", "28/3/2016");

            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(6, lector.Reservaties.First().Aantal);
            Assert.AreEqual(2, student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties.First().ReservatieState is Overrulen);
            Assert.IsTrue(student.Reservaties[student.Reservaties.Count-1].ReservatieState is Gereserveerd);
            Assert.AreEqual(5, student.Reservaties.First().Aantal);
        }
    }
}
