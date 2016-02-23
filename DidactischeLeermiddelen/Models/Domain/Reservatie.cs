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

        public void MaakReservatie(Materiaal materiaal, int aantal, int week)
        {
            if (materiaal == null)
                throw new ArgumentNullException("U heeft nog geen items geselecteerd voor deze reservatie");

            StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Today.Year, week);
            Materiaal = materiaal;

            try
            {
                IList<Stuk> stuks = materiaal.Stuks;
                Stuk stuk = stuks.FirstOrDefault(t => t.StatusData[week].Status == Status.Beschikbaar);

                if (stuk != null)
                {
                    stuk.StatusData[week].Status = Status.Gereserveerd;
                }

                //foreach (Stuk t in )
                //{
                //    t.StatusData[week].Status = Status.Gereserveerd;
                //    break;
                //}
            }
            catch (Exception)
            {
                Console.WriteLine("Er is een fout opgetreden tijdens het maken van reservaties");
            }

            //foreach (Materiaal m in materialen)
            //{
            //    //if (m.ReservatieData.Contains(startDatum))
            //    //{
            //    //    throw new ArgumentException(
            //    //        "Het materiaal {0} is reeds gereserveerd op deze datum, gelieve uw reservatie aan te passen.",
            //    //        m.Naam);
            //    //}
            //    //else
            //    //{
            //    //    m.ReservatieData.Add(startDatum);
            //    //}
            //}
            //Materialen = materialen;

        }
    }
}