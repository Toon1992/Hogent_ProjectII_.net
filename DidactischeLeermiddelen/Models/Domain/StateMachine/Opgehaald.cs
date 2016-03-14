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
        public override void Reserveer()
        {
            throw new NotImplementedException();
        }

        public override void Blokkeer()
        {
            throw new NotImplementedException();
        }

        public override void Overruul()
        {
            throw new NotImplementedException();
        }
    }
}