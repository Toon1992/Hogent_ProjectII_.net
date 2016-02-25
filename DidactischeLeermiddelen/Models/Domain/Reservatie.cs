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
        public Materiaal Materiaal { get; set; }
        public DateTime StartDatum { get; set; }
        public long ReservatieId { get; set; }

        public Boolean MaakReservatie(Materiaal materiaal, int week)
        {
            if (materiaal == null)
                throw new ArgumentNullException("U heeft nog geen items geselecteerd voor deze reservatie");
            if (week <= 0)
                throw new ArgumentException("Week moet op zijn minst hoger dan nul zijn");

            StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Today.Year, week);
            Materiaal = materiaal;


            IList<Stuk> stuks = materiaal.Stuks;

            if (stuks == null)
                throw new ArgumentNullException("Materiaal heeft een lijst met Stuks nodig");

            Stuk stuk = stuks.FirstOrDefault(t => t.HuidigeStatus == Status.Beschikbaar);

            if (stuk != null)
            {
                stuk.VoegNieuweStatusDataToe(week, Status.Gereserveerd);
                stuk.WordtGereserveerd();
                materiaal.CheckNieuwAantal();
                return true;
            }

            return false;
        }
    }
}
