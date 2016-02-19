using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class ReservatieViewModel : IViewModel
    {
        public int MateriaalId { get; set; }
        public string Foto { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public Status Status { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }

        public ReservatieViewModel(Materiaal materiaal)
        {
            MateriaalId = materiaal.MateriaalId;
            Foto = materiaal.Foto;
            Naam = materiaal.Naam;
            Omschrijving = materiaal.Omschrijving;
            Status = materiaal.Status;
            materiaal.ReservatieData.OrderBy(b => b.Day);
            StartDatum = materiaal.ReservatieData.FirstOrDefault();
            EindDatum = StartDatum.AddDays(4);
        }
    }

    public class ReservatieMaterialenViewModel : IViewModel
    {
        public IEnumerable<ReservatieViewModel> Materialen { get; set; }

    }
}