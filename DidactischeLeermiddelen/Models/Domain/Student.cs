using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Student : Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        //public override void VoegReservatieToe(Materiaal materiaal, DateTime startDatum)
        //{
        //    Reservatie reservatie = new Reservatie();
        //    reservatie.MaakReservatie(materiaal, startDatum);
        //    Reservaties.Add(reservatie);
        //}
    }
}