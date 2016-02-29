using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Beschikbaar : ReservatieState
    {
        public Beschikbaar() {}
        public Beschikbaar(Reservatie reservatie):base(reservatie)
        {
        }

        public override void Reserveer()
        {
           
        }

        public override void Annuleer()
        {
            throw new ArgumentException("Materiaal  Reservatie.ToState(new Gereserveerd(Reservatie));is reeds beschikbaar");
        }

        public override void Deblokkeer()
        {
            throw new ArgumentException("Materiaal is reeds beschikbaar");
        }

        public override void Blokkeer()
        {
            Reservatie.ToState(new Geblokkeerd(Reservatie));
        }
    }
}