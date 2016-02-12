using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DidactischeLeermiddelen;
using DidactischeLeermiddelen.Controllers;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Tests.Domain;
using DidactischeLeermiddelen.ViewModels;
using moq;

namespace DidactischeLeermiddelen.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        private CatalogusController controller;
        private Mock<IMateriaalRepository> mockMateriaalRepository;

        [TestInitialize]
        public void OpzettenContext()
        {
            DummyContext context=new DummyContext();
            mockMateriaalRepository=new Mock<IMateriaalRepository>();
            controller=new CatalogusController(mockMateriaalRepository.Object);

        }

        [TestMethod]
        public void IndexMethodeGeeftCatalogusWeer()
        {
            ViewResult result= controller.Index() as ViewResult;
            MaterialenViewModel vm=result.Model as MaterialenViewModel;
            Assert.AreEqual(5,vm.Materialen.Count());

        }
        
    }
}
