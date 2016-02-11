using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public class MaterialenViewModel
    {
        public IEnumerable<MateriaalViewModel> Materialen { get; set; }
        

        
    }

    public class MateriaalViewModel
    {
        public string Foto { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }
        public int AantalInCatalogus { get; set; }
        public int ArtikelNr { get; set; }
        public double Prijs { get; set; }
        public string Firma { get; set; }
        public List<Doelgroep> Doelgroepen { get; set; }
        public List<Leergebied> Leergebieden { get; set; }
        public Status Status { get; set; }

        public MateriaalViewModel(Materiaal materiaal)
        {
            Foto = materiaal.Foto;
            Naam = materiaal.Naam;
            Omschrijving = materiaal.Omschrijving;
            AantalInCatalogus = materiaal.AantalInCatalogus;
            ArtikelNr = materiaal.ArtikelNr;
            Prijs = materiaal.Prijs;
            Firma = materiaal.Firma;
            Doelgroepen = materiaal.Doelgroepen;
            Leergebieden = materiaal.Leergebieden;
            Status = materiaal.Status;
        }

    }
}