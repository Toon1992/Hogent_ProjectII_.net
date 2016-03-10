using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.StateMachine;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Reservatie
    {
        private ReservatieState _reservatieState;

        public long ReservatieId { get; set; }
        public virtual Materiaal Materiaal { get; set; }
        public virtual Gebruiker Gebruiker { get; set; }
        public int Aantal { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }
        public ReservatieStateEnum ReservatieStateEnum { get; set; }
        public ReservatieState ReservatieState
        {
            get
            {
                switch (ReservatieStateEnum)
                {
                    case ReservatieStateEnum.Geblokkeerd: return new Geblokkeerd(this);
                    case ReservatieStateEnum.Gereserveerd: return new Gereserveerd(this);
                    case ReservatieStateEnum.Opgehaald: return new Opgehaald(this);
                    case ReservatieStateEnum.TeLaat: return new TeLaat(this);               
                    case ReservatieStateEnum.Overrulen: return new Overrulen(this);
                }
                return null;
            }
            set
            {
               _reservatieState = value;
                switch (_reservatieState.GetType().Name)
                {
                    case "Geblokkeerd": ReservatieStateEnum = ReservatieStateEnum.Geblokkeerd; break;
                    case "Gereserveerd": ReservatieStateEnum = ReservatieStateEnum.Gereserveerd; break;
                    case "TeLaat": ReservatieStateEnum = ReservatieStateEnum.TeLaat; break;
                    case "Opgehaald": ReservatieStateEnum = ReservatieStateEnum.Opgehaald; break;
                    case "Overrulen": ReservatieStateEnum = ReservatieStateEnum.Overrulen;
                        break;

                }
            }
        }

        public Reservatie() { }

        public Reservatie(Gebruiker gebruiker, Materiaal materiaal, string startDatum, string eindDatum, int aantal)
        {
            if (materiaal == null)
                throw new ArgumentNullException("U heeft nog geen items geselecteerd voor deze reservatie");

            if (gebruiker is Student)
            {
                var week = HulpMethode.GetIso8601WeekOfYear(Convert.ToDateTime(startDatum));
                StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week);
                EindDatum = StartDatum.AddDays(4);
            }
            if (gebruiker is Lector)
            {
                StartDatum = Convert.ToDateTime(startDatum);
                EindDatum = Convert.ToDateTime(eindDatum);
            }
            Materiaal = materiaal;
            Aantal = aantal;
        }

        public bool OverschrijftMetReservatie(DateTime startdatum, DateTime eindDatum)
        {
            return startdatum <= EindDatum && eindDatum >= StartDatum;
        }
        public void Reserveer()
        {
            ReservatieState.Reserveer();
        }

        public void Blokkeer()
        {
            ReservatieState.Blokkeer();
        }

        public void Overruul()
        {
            ReservatieState.Overruul();
        }

        public void ToState(ReservatieState reservatieState)
        {
            ReservatieState = reservatieState;
            ReservatieState.Reservatie = this;
        }
        
    }
}
