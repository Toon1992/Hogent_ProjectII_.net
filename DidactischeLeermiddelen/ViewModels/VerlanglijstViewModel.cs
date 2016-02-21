﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{


    public class VerlanglijstViewModel : IViewModel
    {
        public int MateriaalId { get; set; }
        public string Foto { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public Status Status { get; set; }
        public string Firma { get; set; }
        public List<Leergebied> Leergebieden { get; set; }
        public int AantalInCatalogus { get; set; }
        public int AantalBeschikbaar { get; set; }
        public int AantalGeselecteerd { get; set; }
        public bool Beschikbaar { get; set; }
        public bool Geselecteerd { get; set; }      

        public VerlanglijstViewModel(Materiaal materiaal)
        {
            MateriaalId = materiaal.MateriaalId;
            Foto = materiaal.Foto;
            Naam = materiaal.Naam;
            Omschrijving = materiaal.Omschrijving;
            Status = materiaal.Status;
            Firma = materiaal.Firma;
            Leergebieden = materiaal.Leergebieden as List<Leergebied>;
            AantalInCatalogus = materiaal.AantalInCatalogus;
            AantalBeschikbaar = materiaal.AantalInCatalogus;
            Beschikbaar = true;
        }
        public VerlanglijstViewModel() { }
    }

    public class VerlanglijstMaterialenViewModel : IViewModel
    {
        public IEnumerable<VerlanglijstViewModel> Materialen { get; set; }
        [DisplayFormat(DataFormatString = "{dd-mm-yyyy}")]
        public DateTime GeselecteerdeWeek { get; set; }
        public int TotaalGeselecteerd { get; set; }
    }
}