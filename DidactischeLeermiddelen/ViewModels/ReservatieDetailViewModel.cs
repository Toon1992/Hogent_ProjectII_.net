using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class ReservatiesDetailViewModel
    {
        public Dictionary<DateTime, ICollection<ReservatieDetailViewModel>> Reservaties { get; set; }
        public Materiaal Material { get; set; }
        public string GeselecteerdeWeek { get; set; }
    }

    public class ReservatieDetailViewModel
    {
        public int Aantal { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string GeblokkeerdTot { get; set; }
        public DateTime StartDatum { get; set; }
    }
}