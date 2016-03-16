using System;

namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    public class Gereserveerd : ReservatieState
    {
        public Gereserveerd(Reservatie reservatie):base(reservatie)
        {
        }
        public Gereserveerd() { }

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