using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class TeLaat : ReservatieState
    {
        public TeLaat(Reservatie reservatie) : base(reservatie)
        {
            
        }
        public TeLaat() { }
        public void Onbeschikbaar()
        {
            Reservatie.ToState(new Onbeschikbaar(Reservatie));
        }

        public override void Reserveer()
        {
            throw new NotImplementedException();
        }

        public override void Annuleer()
        {
            throw new NotImplementedException();
        }

        public override void Onblokkeer()
        {
            Reservatie.ToState(new Beschikbaar(Reservatie));
        }
    }
}