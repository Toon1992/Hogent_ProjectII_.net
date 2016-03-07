using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    public class Gereserveerd : ReservatieState
    {
        public Gereserveerd(Reservatie reservatie):base(reservatie)
        {
        }
        public Gereserveerd() { }
        public override void Reserveer()
        {
            throw new ArgumentException("Materiaal is al gereserveerd");
        }

        public override void Blokkeer()
        {
            Reservatie.ToState(new Geblokkeerd(Reservatie));
        }
    }
}