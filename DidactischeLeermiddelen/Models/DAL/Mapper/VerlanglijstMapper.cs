﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class VerlanglijstMapper:EntityTypeConfiguration<Verlanglijst>
    {
        public VerlanglijstMapper()
        {
            HasKey(v => v.Id);
            HasMany(v => v.Materialen).WithOptional().WillCascadeOnDelete(false);
        }
    }
}