using System;

namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    public class Geblokkeerd : ReservatieState
    {
        public Geblokkeerd(Reservatie reservatie):base(reservatie)
        {
            
        }
        public Geblokkeerd() { }

        public override void Reserveer()
        {
            throw new ArgumentException("Materiaal is geblokkeerd");
        }

        public override void Blokkeer()
        {
            throw new ArgumentException("Materiaal is al geblokkeerd");
        }

        public override void Overruul()
        {
            throw new ArgumentException("Materiaal is al geblokkeerd");
        }
    }
}