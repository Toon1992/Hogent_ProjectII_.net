using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

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
    }
}