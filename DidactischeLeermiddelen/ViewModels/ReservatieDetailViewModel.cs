using System;
using System.Collections.Generic;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class ReservatiesDetailViewModel:IViewModel
    {
        public Dictionary<DateTime, ICollection<ReservatieDetailViewModel>> ReservatieMap { get; set; }
        public Materiaal Material { get; set; }
        public string GeselecteerdeWeek { get; set; }
    }

    public class ReservatieDetailViewModel:IViewModel
    {
        public int Aantal { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string GeblokkeerdTot { get; set; }
        public DateTime StartDatum { get; set; }
    }
}