﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public abstract class ViewModelFactory
    {
        public virtual IViewModel CreateMateriaalViewModel(Materiaal materiaal)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateFirmaViewModel(Materiaal materiaal)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateMaterialenViewModel(SelectList doelgroepen, SelectList leergebieden,
            IEnumerable<Materiaal> lijst)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateVerlanglijstMaterialenViewModel(IList<Materiaal> materialen, List<Materiaal> verlanglijstMaterialen, string datum, DateTime startDatum, DateTime eindDatum, Dictionary<int, int> materiaalAantal, bool naarReserveren, Gebruiker gebruiker)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateReservatieMaterialenViewModel(Gebruiker gebruiker)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateReservatieDetailViewModel(Reservatie reservatie)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateReservatiesViewModel(
            Dictionary<DateTime, IEnumerable<ReservatieDetailViewModel>> map, Materiaal materiaal, int week,
            DateTimeFormatInfo dtfi)
        {
            throw new NotImplementedException();
        }
    }

    public class HulpMethode
    {
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;

            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime GetStartDatum(string startDatum)
        {
            var dateFromString = Convert.ToDateTime(startDatum);
            var week = GetIso8601WeekOfYear(dateFromString);
            return FirstDateOfWeekISO8601(DateTime.Now.Year, week);
        }

        public static DateTime GetEindDatum(string startDatum)
        {
            return GetStartDatum(startDatum).AddDays(4);
        }

        public static string DateToString(DateTime startDatum, DateTimeFormatInfo format)
        {
            return startDatum.ToString("d", format);
        }

        public static string DatesToString(IEnumerable<DateTime> dagen, DateTimeFormatInfo format)
        {
            return string.Join(",", dagen.Select(d => d.ToString("d", format)).ToArray());
        }
    }
}