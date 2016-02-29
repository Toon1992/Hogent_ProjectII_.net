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
        public Onbeschikbaar() { }
        public override void Reserveer()
        {
            throw new ArgumentException("Materiaal is onbeschikbaar");
        }

        public override void Annuleer()
        {
            throw new ArgumentException("Materiaal is onbeschikbaar");
        }

        public override void Deblokkeer()
        {
            Reservatie.ToState(new Beschikbaar(Reservatie));
        }

        public override void Blokkeer()
        {
            throw new ArgumentException("Materiaal is onbeschikbaar");
        }
    }
}