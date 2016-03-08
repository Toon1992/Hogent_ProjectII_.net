using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    public class TeLaat : ReservatieState
    {
        public TeLaat(Reservatie reservatie) : base(reservatie)
        {
            
        }
        public TeLaat() { }

        public override void Reserveer()
        {
            Reservatie.ToState(new Gereserveerd(Reservatie));
        }

        public override void Blokkeer()
        {
            Reservatie.ToState(new Geblokkeerd(Reservatie));
        }

        public override void Overruul()
        {
            throw new NotImplementedException();
        }
    }
}