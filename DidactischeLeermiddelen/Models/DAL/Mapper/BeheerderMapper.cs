﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class BeheerderMapper:EntityTypeConfiguration<Beheerder>
    {
        public BeheerderMapper()
        {
            HasKey(b => b.GebruikersId);
        }
    }
}