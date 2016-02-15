using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{


    public class VerlanglijstViewModel
    {
        public int MateriaalId { get; set; }
        public string Foto { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public Status Status { get; set; }

        public VerlanglijstViewModel(Materiaal materiaal)
        {
            MateriaalId = materiaal.ArtikelNr;
            Foto = materiaal.Foto;
            Naam = materiaal.Naam;
            Omschrijving = materiaal.Omschrijving;
            Status = materiaal.Status;
        }
    }

    public class VerlanglijstMaterialenViewModel
    {
        public IEnumerable<VerlanglijstViewModel> Materialen { get; set; }

    }
}