﻿using System;
using System.Collections.Generic;
using DidactischeLeermiddelen.Models.Domain;
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

    }
}
