using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Student : Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }

        public void MaakReservaties(IDictionary<Materiaal, int> potentieleReservaties, string startDatum,
            string eindDatum)
        {
            try
            {
                foreach (KeyValuePair<Materiaal, int> potentiele in potentieleReservaties)
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

        public override DateTime GetStartDatum(string startDatum, string eindDatum)
        {
            var dateFromString = Convert.ToDateTime(startDatum);
            var week = HulpMethode.GetIso8601WeekOfYear(dateFromString);
            return HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week);     
        }

        public override DateTime GetEindDatum(string startDatum, string eindDatum)
        {
            return GetStartDatum(startDatum, eindDatum).AddDays(4);
        }

        public override string DateToString(DateTime startDatum, DateTime eindDatum, DateTimeFormatInfo format)
        {
            return startDatum.ToString("d", format);
        }
    }
}