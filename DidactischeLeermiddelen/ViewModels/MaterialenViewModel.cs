using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class MaterialenViewModel : IViewModel
    {
        public IEnumerable<MateriaalViewModel> Materialen { get; set; }
        public DoelgroepViewModel DoelgroepSelectList { get; set; }
        public LeergebiedViewModel LeergebiedSelectList { get; set; }
    }

    public class MateriaalViewModel : IViewModel
    {
        public int MateriaalId { get; set; }
        public string Foto { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        [Display(Name = "Aantal")]
        public int AantalInCatalogus { get; set; }
        [Display(Name = "Artikelnummer")]
        public int ArtikelNr { get; set; }
        public decimal Prijs { get; set; }
        public Firma Firma { get; set; }
        public List<Doelgroep> Doelgroepen { get; set; }
        public List<Leergebied> Leergebieden { get; set; }
        public bool InVerlanglijst { get; set; }

        public MateriaalViewModel(Materiaal materiaal)
        {
            MateriaalId = materiaal.MateriaalId;
            Foto = materiaal.Foto;
            Naam = materiaal.Naam;
            Omschrijving = materiaal.Omschrijving;
            AantalInCatalogus = materiaal.AantalInCatalogus;
            ArtikelNr = materiaal.ArtikelNr;
            Prijs = materiaal.Prijs;
            Firma = materiaal.Firma;
            Doelgroepen = materiaal.Doelgroepen as List<Doelgroep>;
            Leergebieden = materiaal.Leergebieden as List<Leergebied>;
            InVerlanglijst = materiaal.InVerlanglijst;
        }

    }
}