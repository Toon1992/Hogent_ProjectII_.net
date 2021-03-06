﻿using System;
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
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            student.MaakReservaties(materiaalMap, "23/3/2016");
            Assert.AreEqual(1, student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties.First().ReservatieState is Gereserveerd);
            Assert.AreEqual(5, student.Reservaties.First().AantalGereserveerd);
        }

        [TestMethod]
        public void StudentenMaaktReservatieMetTweeMaterialen()
        {
            Student student = context.Toon as Student;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            materiaalMap.Add(context.Encyclopedie, 5);
            student.MaakReservaties(materiaalMap, "23/3/2016");
            Assert.AreEqual(2, student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties[1].ReservatieState is Gereserveerd);
            Assert.AreEqual(5, student.Reservaties[0].AantalGereserveerd);
            Assert.AreEqual(5, student.Reservaties[1].AantalGereserveerd);
        }

        [TestMethod]
        public void StudentMaakReservatieMeerdereMaterialen()
        {
            Student student = context.Toon as Student;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            materiaalMap.Add(context.Encyclopedie, 2);
            student.MaakReservaties(materiaalMap, "23/3/2016");
            Assert.AreEqual(2, student.Reservaties.Count);
        }

        [TestMethod]
        public void StudentMaakReservatieMaxAantal()
        {
            Student student = context.Toon as Student;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 10);
            student.MaakReservaties(materiaalMap, "23/3/2016");
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
            student.MaakReservaties(materiaalMap, "23/3/2016");
        }

        [TestMethod]
        public void LectorMaakReservatie()
        {
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            string[] dagenGeblokkeerd = new[] { "23/3/2016" };
            lector.MaakBlokkeringen(materiaalMap, "23/3/2016", dagenGeblokkeerd);
            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(5, lector.Reservaties.First().AantalGereserveerd);
            Assert.AreEqual("23/03/2016", lector.Reservaties.First().GeblokkeerdeDagen[0].Datum.ToShortDateString());
        }

        [TestMethod]
        public void LectorMaakReservatieMetMeerderGeblokkeerdeDagenInDeWeek()
        {
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            string[] dagenGeblokkeerd = new[] { "23/3/2016", "25/3/2016" };
            lector.MaakBlokkeringen(materiaalMap, "23/3/2016", dagenGeblokkeerd);
            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(5, lector.Reservaties.First().AantalGereserveerd);
        }

        [TestMethod]
        public void LectorMaakReservatieMetMeerderGeblokkeerdeDagenInVerschillendeWeken()
        {
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            string[] dagenGeblokkeerd = new[] { "23/3/2016", "29/3/2016" };
            lector.MaakBlokkeringen(materiaalMap, "23/3/2016", dagenGeblokkeerd);
            Assert.AreEqual(2, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(5, lector.Reservaties.First().AantalGereserveerd);
        }

        [TestMethod]
        public void LectorMaakBlokkeringWaarStudentAlHeeftGereserveerdMaarErIsNogGenoegOver()
        {
            Student student = context.Toon as Student;
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 5);
            string[] dagenGeblokkeerd = new[] { "23/3/2016" };
            student.MaakReservaties(materiaalMap, "23/3/2016");
            lector.MaakBlokkeringen(materiaalMap, "23/3/2016", dagenGeblokkeerd);

            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(5, lector.Reservaties.First().AantalGereserveerd);
            Assert.AreEqual(1, student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties.First().ReservatieState is Gereserveerd);
            Assert.AreEqual(5, student.Reservaties.First().AantalGereserveerd);
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
            string[] dagenGeblokkeerd = new[] { "23/3/2016" };
            student.MaakReservaties(materiaalMap, "23/3/2016");
            lector.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", dagenGeblokkeerd);

            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(6, lector.Reservaties.First().AantalGereserveerd);
            Assert.AreEqual(2, student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties.First().ReservatieState is Overruled);
            Assert.IsTrue(student.Reservaties[student.Reservaties.Count - 1].ReservatieState is Gereserveerd);
            Assert.AreEqual(1, student.Reservaties.First().AantalGereserveerd);
            Assert.AreEqual(4, student.Reservaties[student.Reservaties.Count - 1].AantalGereserveerd);
        }

        public void LectorMaakBlokkeringWaarStudentAlHeeftGereserveerdWaarBlokkeringNodigIsMetEenMateriaal()
        {
            Student student = context.Toon as Student;
            Lector lector = context.LectorGebruiker as Lector;
            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            IDictionary<Materiaal, int> materiaalLectorMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bok, 1);
            materiaalLectorMap.Add(context.Bok, 1);
            string[] dagenGeblokkeerd = new[] { "23/3/2016" };
            student.MaakReservaties(materiaalMap, "23/3/2016");
            lector.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", dagenGeblokkeerd);

            Assert.AreEqual(1, lector.Reservaties.Count);
            Assert.IsTrue(lector.Reservaties.First().ReservatieState is Geblokkeerd);
            Assert.AreEqual(1, student.Reservaties.Count);
            Assert.IsTrue(student.Reservaties.First().ReservatieState is Overruled);
        }

        [TestMethod]
        public void LectorOverruultTweeStudenten()
        {
            Student student1 = context.Toon as Student;
            Student student2 = context.Manu as Student;
            Lector lector = context.LectorGebruiker as Lector;

            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            IDictionary<Materiaal, int> materiaalLectorMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.GeoDriehoek, 1);
            materiaalLectorMap.Add(context.GeoDriehoek, 2);
            student1.MaakReservaties(materiaalMap, "23/3/2016");
            student2.MaakReservaties(materiaalMap, "23/3/2016");
            string[] dagenGeblokkeerd = new[] { "23/3/2016" };
            lector.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", dagenGeblokkeerd);

            Assert.IsTrue(student1.Reservaties.First().ReservatieState is Overruled);
            Assert.IsTrue(student2.Reservaties.First().ReservatieState is Overruled);
        }

        [TestMethod]
        public void LectorOverruultTweeStudentenEnBlokkeertMetLector()
        {
            Student student1 = context.Toon as Student;
            Student student2 = context.Manu as Student;
            Lector lector1 = context.LectorGebruiker as Lector;
            Lector lector2 = context.LectorGebruiker2 as Lector;

            IDictionary<Materiaal, int> materiaalMap = new Dictionary<Materiaal, int>();
            IDictionary<Materiaal, int> materiaalLectorMap = new Dictionary<Materiaal, int>();
            materiaalMap.Add(context.Bol, 4);
            materiaalLectorMap.Add(context.Bol, 3);
            student1.MaakReservaties(materiaalMap, "23/3/2016");

            string[] dagenGeblokkeerd1 = new[] { "24/3/2016" };
            lector1.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", dagenGeblokkeerd1);

            student2.MaakReservaties(materiaalMap, "23/3/2016");
            string[] dagenGeblokkeerd2 = new[] { "23/3/2016" };
            lector2.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", dagenGeblokkeerd2);

            Assert.IsTrue(student1.Reservaties.First().ReservatieState is Gereserveerd);
            Assert.IsTrue(student2.Reservaties.First().ReservatieState is Overruled);
        }


        [TestMethod]
        public void LectorBlokkeertZelfdeWeekAlsAndereLectorMaarAndereDagen()
        {
            Lector lector1 = context.LectorGebruiker as Lector;
            Lector lector2 = context.LectorGebruiker2 as Lector;

            IDictionary<Materiaal, int> materiaalLectorMap = new Dictionary<Materiaal, int>();

            materiaalLectorMap.Add(context.Bol, 10);
           
           /* IDictionary<Materiaal, int> materiaalLectorMap = new Dictionary<Materiaal, int>();
           
            materiaalLectorMap.Add(context.Bol, 10);*/
            
            string[] dagenGeblokkeerd1 = new[] { "24/3/2016" };
            lector1.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", dagenGeblokkeerd1);

            string[] dagenGeblokkeerd2 = new[] { "23/3/2016" };
            lector2.MaakBlokkeringen(materiaalLectorMap, "23/3/2016", dagenGeblokkeerd2);

            Assert.AreEqual(1, lector1.Reservaties.Count);
            Assert.AreEqual(1, lector2.Reservaties.Count);
        }

    }
}
