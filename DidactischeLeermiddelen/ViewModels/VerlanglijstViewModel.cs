using System;
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
        //public Status Status { get; set; }
        public Firma Firma { get; set; }
        public List<Leergebied> Leergebieden { get; set; }
        public List<Doelgroep> Doelgroepen { get; set; } 
        public int AantalInCatalogus { get; set; }
        public int AantalBeschikbaar { get; set; }
        public int AantalGeblokkeerd { get; set; }
        public int AantalOnbeschikbaar { get; set; }
        public int AantalGeselecteerd { get; set; }
        public bool Beschikbaar { get; set; }
        public bool Geselecteerd { get; set; }      
        public string Beschikbaarheid { get; set; }
        public int ArtikelNr { get; set; }
        public Decimal Prijs { get; set; }

        public VerlanglijstViewModel(Materiaal materiaal, DateTime startdatum)
        {
            MateriaalId = materiaal.MateriaalId;
            Foto = materiaal.Foto;
            Naam = materiaal.Naam;
            Omschrijving = materiaal.Omschrijving;
            //Status = materiaal.Status;
            Firma = materiaal.Firma;
            Leergebieden = materiaal.Leergebieden as List<Leergebied>;
            Doelgroepen = materiaal.Doelgroepen as List<Doelgroep>;
            AantalInCatalogus = materiaal.AantalInCatalogus;
            AantalBeschikbaar = materiaal.AantalInCatalogus;
            Beschikbaar = true;
            ArtikelNr = materiaal.ArtikelNr;
            Prijs = materiaal.Prijs;
            AantalGeblokkeerd = materiaal.GeefAantal(new Geblokkeerd(), startdatum);
            AantalOnbeschikbaar = materiaal.GeefAantal(new Onbeschikbaar(), startdatum);
        }

        public VerlanglijstViewModel() { }
    }

    public class VerlanglijstMaterialenViewModel : IViewModel
    {
        public IEnumerable<VerlanglijstViewModel> Materialen { get; set; }
        [DisplayFormat(DataFormatString = "{dd-mm-yyyy}")]
        public string GeselecteerdeWeek { get; set; }

        public string StartDatum { get; set; }
        public string EindDatum { get; set; }
        public int TotaalGeselecteerd { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }
}