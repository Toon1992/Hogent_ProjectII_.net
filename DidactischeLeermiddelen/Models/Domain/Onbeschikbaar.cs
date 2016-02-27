using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Onbeschikbaar : ReservatieState
    {
        public Onbeschikbaar(Reservatie reservatie) : base(reservatie)
        {
            
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