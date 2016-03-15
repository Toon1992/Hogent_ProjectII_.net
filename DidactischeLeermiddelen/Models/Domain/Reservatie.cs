using System;
using System.Collections.Generic;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public abstract class Reservatie :IComparable<Reservatie>
    {
        private ReservatieState _reservatieState;
        public long ReservatieId { get; set; }
        public virtual Materiaal Materiaal { get; set; }
        public virtual Gebruiker Gebruiker { get; set; }
        public virtual IList<Dag> GeblokkeerdeDagen { get; set; } 
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
                    case ReservatieStateEnum.Overruled: return new Overruled(this);
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
                    case "Overruled":
                        ReservatieStateEnum = ReservatieStateEnum.Overruled;
                        break;

                }
            }
        }

        public Reservatie()
        {
            GeblokkeerdeDagen = new List<Dag>();
        }

        public Reservatie(Gebruiker gebruiker, Materiaal materiaal, string startDatum, int aantal)
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
                EindDatum = Convert.ToDateTime(HulpMethode.GetEindDatum(startDatum));
            }

            Materiaal = materiaal;
            Aantal = aantal;
            Gebruiker = gebruiker;
            GeblokkeerdeDagen = new List<Dag>();
        }

        public bool KanOverschrijvenMetReservatie(DateTime startdatum, DateTime eindDatum)
        {
            return startdatum <= EindDatum && eindDatum >= StartDatum;
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


        public int CompareTo(Reservatie other)
        {
            if (this.Gebruiker is Lector && this.Gebruiker is Lector)
                return -1;
            if (this.Gebruiker is Student && other.Gebruiker is Student)
                return 0;

            return 1;
        }
    }
}
