using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.DAL;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class ViewModelFactory
    {
        public static IViewModel CreateViewModel(String type, SelectList doelgroepen, SelectList leergebieden, IEnumerable<Materiaal> lijst = null, Gebruiker gebruiker = null)
        {
            switch (type)
            {
                case "MaterialenViewModel":
                    IViewModel vm = new MaterialenViewModel()
                    {
                        Materialen = lijst.Select(b => new MateriaalViewModel(b)),
                        DoelgroepSelectList = new DoelgroepViewModel(doelgroepen),
                        LeergebiedSelectList = new LeergebiedViewModel(leergebieden)
                    };
                    return vm;
                case "VerlanglijstMaterialenViewModel":
                    IViewModel vmm = new VerlanglijstMaterialenViewModel()
                    {
                        Materialen = gebruiker.Verlanglijst.Materialen.Select(b => new VerlanglijstViewModel(b))
                    };
                    return vmm;
                case "ReservatieMaterialenViewModel":
                    IViewModel rmv = new ReservatieMaterialenViewModel()
                    {
                        Materialen = gebruiker.Reservaties.Select(b => new ReservatieViewModel(b)),
                    };
                    return rmv;
            }

            return null;
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
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}