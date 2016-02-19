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
        public IEnumerable<Materiaal> Materialen { get; set; }
        public DateTime StartDatum { get; set; }
        public long ReservatieId { get; set; }

        public Reservatie(List<Materiaal> materialen, DateTime startDatum)
        {
            if (materialen == null)
                throw new ArgumentException("U heeft nog geen items geselecteerd voor deze reservatie");
            foreach (Materiaal m in materialen)
            {
                //if (m.ReservatieData.Contains(startDatum))
                //{
                //    throw new ArgumentException(
                //        "Het materiaal {0} is reeds gereserveerd op deze datum, gelieve uw reservatie aan te passen.",
                //        m.Naam);
                //}
                //else
                //{
                //    m.ReservatieData.Add(startDatum);
                //}
            }
            Materialen = materialen;
            StartDatum = startDatum;
        }
    }
}