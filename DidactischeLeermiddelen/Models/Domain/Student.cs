using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Student : Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public void maakReservaties(IDictionary<Materiaal, int> PotentieleReservaties, string startDatum,
            string eindDatum)
        {
            foreach (KeyValuePair< Materiaal, int> potentiele in PotentieleReservaties)
            {
                VoegReservatieToe(potentiele.Key, potentiele.Value, startDatum, eindDatum, false);
            }
            VerzendMailNaReservatie(PotentieleReservaties,startDatum,eindDatum,this);
        }
    }
}