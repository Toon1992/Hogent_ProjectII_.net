using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class ReservatieViewModel : IViewModel
    {
        public string Foto { get; set; }
        public string Naam { get; set; }
        public string Firma { get; set; }
        public int MateriaalId { get; set; }
        public string Omschrijving { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime StartDatum { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime EindDatum { get; set; }
        public int AantalStuksGereserveerd { get; set; }
        public ReservatieState Status { get; set; }

        public ReservatieViewModel(Reservatie reservatie)
        {
            Foto = reservatie.Materiaal.Foto;
            Naam = reservatie.Materiaal.Naam;
            Firma = reservatie.Materiaal.Firma.Naam;
            Omschrijving = reservatie.Materiaal.Omschrijving;
            MateriaalId = reservatie.Materiaal.MateriaalId;
            StartDatum = reservatie.StartDatum;
            EindDatum = StartDatum.AddDays(4);
            AantalStuksGereserveerd = reservatie.Aantal;
            Status = reservatie.ReservatieState;
        }
    }

    public class ReservatieMaterialenViewModel : IViewModel
    {
        public IEnumerable<ReservatieViewModel> Materialen { get; set; }
    }
}