using System;
using System.Collections.Generic;
using System.Globalization;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class ReservatieDetailViewModelFactory : ViewModelFactory
    {
        public override IViewModel CreateReservatieDetailViewModel(Reservatie reservatie)
        {
            ReservatieDetailViewModel vm = new ReservatieDetailViewModel
            {
                Aantal = reservatie.Aantal,
                Email = reservatie.Gebruiker.Email,
                Status = reservatie.ReservatieState.GetType().Name.ToLower(),
                Type = reservatie.Gebruiker is Lector ? "Lector" : "Student",
                GeblokkeerdTot = reservatie.Gebruiker is Lector ? reservatie.EindDatum.ToString("d") : ""
            };
            return vm;
        }

        public override IViewModel CreateReservatiesViewModel(Dictionary<DateTime, ICollection<ReservatieDetailViewModel>> map, Materiaal materiaal, int week, DateTimeFormatInfo dtfi)
        {
            ReservatiesDetailViewModel vm =  new ReservatiesDetailViewModel
            {
                ReservatieMap = map,
                Material = materiaal,
                GeselecteerdeWeek =
                    week != -1 ? HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d", dtfi) : ""
            };
            return vm;
        }
    }
}