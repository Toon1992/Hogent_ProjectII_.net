using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Reservatie
    {
        public long ReservatieId { get; set; }
        public virtual Materiaal Materiaal { get; set; }
        public virtual Gebruiker Gebruiker { get; set; }
        public int Aantal { get; set; }
        public DateTime StartDatum { get; set; }
        public virtual ReservatieState ReservatieState { get; set; }
        public Status Status { get; set; }

        public Reservatie() { }
        public Reservatie(Materiaal materiaal, int week, int aantal)
        {
            if (materiaal == null)
                throw new ArgumentNullException("U heeft nog geen items geselecteerd voor deze reservatie");
            if (week <= 0)
                throw new ArgumentException("Week moet op zijn minst hoger dan nul zijn");
            if (aantal <= 0)
                throw new ArgumentException("Aantal moet groter dan 0 zijn.");

            StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Today.Year, week);
            Materiaal = materiaal;
            Aantal = aantal;
            ReservatieState = new Beschikbaar(this);
        }

        public void Reserveer()
        {
            ReservatieState.Reserveer();
        }

        public void ToState(ReservatieState reservatieState)
        {
            ReservatieState = reservatieState;
        }
    }
}
