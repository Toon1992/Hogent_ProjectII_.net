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
            try
            {
                foreach (KeyValuePair<Materiaal, int> potentiele in PotentieleReservaties)
                {
                    potentiele.Key.MaakReservatieLijstAan();
                    VoegReservatieToe(potentiele.Key, potentiele.Value, startDatum, eindDatum);
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Iets fout gelopen hier");
            }
            //VerzendMailNaReservatie(PotentieleReservaties,startDatum,eindDatum,this);
        }
    }
}