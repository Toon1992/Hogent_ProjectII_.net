using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    public class Opgehaald : ReservatieState
    {
        public Opgehaald()
        {
            
        }
        public Opgehaald(Reservatie reservatie) : base(reservatie)
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

        public override void Deblokkeer()
        {
            Reservatie.ToState(new Beschikbaar(Reservatie));
        }

        public override void Blokkeer()
        {
            throw new NotImplementedException();
        }
    }
}