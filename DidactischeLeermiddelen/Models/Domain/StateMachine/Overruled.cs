using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    
    public class Overruled : ReservatieState
    {
        public Overruled(Reservatie reservatie):base(reservatie)
        {
            
        }
        public override void Reserveer()
        {
            throw new ArgumentException("Het materiaal kan niet gereserveerd worden");
        }

        public override void Blokkeer()
        {
            throw new ArgumentException("Kan niet meer geblokkeerd worden");
        }

        public override void Overruul()
        {
            throw new ArgumentException("Het is al overuult");
        }
    }
}