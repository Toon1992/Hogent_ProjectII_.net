using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Reservatie
    {
        public Materiaal Materiaal { get; set; }
        public DateTime StartDatum { get; set; }
        public long ReservatieId { get; set; }

        public void MaakReservatie(Materiaal materiaal, DateTime startDatum)
        {
            if (materiaal == null)
                throw new ArgumentNullException("U heeft nog geen items geselecteerd voor deze reservatie");

            StartDatum = startDatum;
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