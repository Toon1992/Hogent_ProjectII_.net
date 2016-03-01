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
        public DateTime EindDatum { get; set; }
        public int AantalDagenGeblokkeerd { get; set; }
        public virtual ReservatieState ReservatieState { get; set; }
        public Status Status { get; set; }

        public Reservatie() { }
        public Reservatie(Gebruiker gebruker, Materiaal materiaal, string startDatum, string eindDatum, int aantal)
        {
            if (materiaal == null)
                throw new ArgumentNullException("U heeft nog geen items geselecteerd voor deze reservatie");
            if (aantal <= 0)
                throw new ArgumentException("Aantal moet groter dan 0 zijn.");

            if (gebruker is Student)
            {
                var week = HulpMethode.GetIso8601WeekOfYear(Convert.ToDateTime(startDatum));
                StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week);
                EindDatum = StartDatum.AddDays(4);
            }
            if (gebruker is Lector)
            {
                StartDatum = Convert.ToDateTime(startDatum);
                EindDatum = Convert.ToDateTime(eindDatum);
            }
            Materiaal = materiaal;
            Aantal = aantal;
            ReservatieState = new Beschikbaar(this);
        }

        public void Reserveer()
        {
            ReservatieState.Reserveer();
        }

        public void Blokkeer()
        {
            ReservatieState.Blokkeer();
        }

        public void ToState(ReservatieState reservatieState)
        {
            ReservatieState = reservatieState;
        }
    }
}
