using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public abstract class ReservatieState
    {
        public int ReservatieStateId { get; set; }
        public virtual Reservatie Reservatie { get; set;}
        protected ReservatieState(Reservatie reservatie)
        {
            Reservatie = reservatie;
        }
        public ReservatieState() { }
        public abstract void Reserveer();
        public abstract void Annuleer();
        public abstract void Deblokkeer();
        public abstract void Blokkeer();

    }
}