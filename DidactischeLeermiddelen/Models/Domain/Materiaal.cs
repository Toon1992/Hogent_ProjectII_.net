using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Materiaal
    {
        #region fields
        public string Foto { get; set; }
        public string Naam { get; set; }

        public string Omschrijving { get; set; }

        public int AantalInCatalogus { get; set; }
        public int ArtikelNr { get; set; }

        public Decimal Prijs { get; set; }

        public string Firma { get; set; }

        public virtual List<Doelgroep> Doelgroepen { get; set; }
        public virtual List<Leergebied> Leergebieden { get; set; }

        public Status Status { get; set; }
        #endregion

        public Materiaal(String naam, int artikeNr, int aantal)
        {
            Naam = naam;
            ArtikelNr = artikeNr;
            AantalInCatalogus = aantal;
        }
        public Materiaal() { }
    }
}