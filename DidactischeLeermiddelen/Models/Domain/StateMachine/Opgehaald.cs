using System;

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

    }
}