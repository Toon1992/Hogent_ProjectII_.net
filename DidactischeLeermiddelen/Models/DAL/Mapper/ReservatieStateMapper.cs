using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.StateMachine;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class ReservatieStateMapper : EntityTypeConfiguration<ReservatieState>
    {
        public ReservatieStateMapper()
        {
            HasKey(r => r.ReservatieStateId);
            HasOptional(r => r.Reservatie).WithRequired();
            Map<Beschikbaar>(s => s.Requires("Type").HasValue("Beschikbaar"));
            Map<Onbeschikbaar>(l => l.Requires("Type").HasValue("Onbeschikbaar"));
            Map<Gereserveerd>(l => l.Requires("Type").HasValue("Gereserveerd"));
            Map<Catalogus>(l => l.Requires("Type").HasValue("Catalogus"));
            Map<TeLaat>(l => l.Requires("Type").HasValue("TeLaat"));
            Map<Geblokkeerd>(l => l.Requires("Type").HasValue("Geblokkeerd"));
            Map<Opgehaald>(l => l.Requires("Type").HasValue("Opgehaald"));
        }
    }
}