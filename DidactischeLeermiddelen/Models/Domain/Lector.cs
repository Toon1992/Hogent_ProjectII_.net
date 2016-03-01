using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Lector:Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public override void VoegReservatieToe(IList<Materiaal> materiaal, int[] aantal, string startDatum, string eindDatum)
        {
            ICollection<Reservatie> nieuweReservaties = new List<Reservatie>();
            if (materiaal.Count != aantal.Length)
                throw new ArgumentException("Er moeten evenveel aantallen zijn als materialen");

            int index = 0;
            materiaal.ForEach(m =>
            {
                Reservatie reservatie = new Reservatie(this, m, startDatum,eindDatum, aantal[index]);
                reservatie.Gebruiker = this;
                reservatie.Reserveer();
                m.AddReservatie(reservatie);
                Reservaties.Add(reservatie);
                nieuweReservaties.Add(reservatie);
                index++;
            });

            VerzendMailNaReservatie(nieuweReservaties, week, this); //gebruiker, materiaal, week);
        }

        public void MaakBlokkeringen(ICollection<Reservatie> reservaties, int[] aantal, string startDatum, string eindDatum)
        {
            reservaties.ForEach(r =>
            {
                r.Status = Status.Geblokkeerd;
                r.ReservatieState.Blokkeer();
            });

            IList<Materiaal> materialen = Reservaties.Select(m => m.Materiaal).ToList(); 

            VoegReservatieToe(materialen,aantal,startDatum, eindDatum);
        }
    }
}