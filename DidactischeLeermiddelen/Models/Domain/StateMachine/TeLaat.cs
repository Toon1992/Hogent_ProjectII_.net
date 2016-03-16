using System;

namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    public class TeLaat : ReservatieState
    {
        public TeLaat(Reservatie reservatie) : base(reservatie)
        {
            
        }
        public TeLaat() { }

        public override void Blokkeer()
        {
            Reservatie.ToState(new Geblokkeerd(Reservatie));
        }

        public override void Overruul()
        {
            Reservatie.ToState(new Overruled(Reservatie));
        }
    }
}