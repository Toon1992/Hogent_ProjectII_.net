﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class FirmaMapper:EntityTypeConfiguration<Firma>
    {
        public FirmaMapper()
        {
            HasKey(m => m.Email);
            Property(m => m.Naam).IsRequired();
            Property(m => m.Email).IsRequired();
        }
    }
}