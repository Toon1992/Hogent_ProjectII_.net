using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;
using WebGrease.Css.Extensions;

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
        public int MateriaalId { get; set; }

        public Decimal Prijs { get; set; }

        public virtual Firma Firma { get; set; }
        public virtual IList<Reservatie> Reservaties { get; set; }
        public bool IsReserveerBaar { get; set; }

        public virtual IList<Doelgroep> Doelgroepen { get; set; }
        public virtual IList<Leergebied> Leergebieden { get; set; }

        public bool Onbeschikbaar { get; set; }
        #endregion

        public Materiaal(string naam, int artikeNr, int aantal)
        {
            Naam = naam;
            ArtikelNr = artikeNr;
            AantalInCatalogus = aantal;
        }
        public Materiaal() { }

        public void AddReservatie(Reservatie reservatie)
        {
            if (Reservaties == null)
            {
                Reservaties = new List<Reservatie>();
            }
            Reservaties.Add(reservatie);
        }

        public int CheckNieuwAantal(DateTime startDatum)
        {
            Reservatie reservatie = Reservaties.FirstOrDefault(r => r.StartDatum.Equals(startDatum));
            if (reservatie != null)
            {
                return AantalInCatalogus - reservatie.Aantal;
            }
            return AantalInCatalogus;
        }

        public int GeefAantal(Status status, DateTime startDatum)
        {
            switch (status)
            {
                case Status.Geblokkeerd: return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Geblokkeerd).Sum(r => r.Aantal);
                case Status.Onbeschikbaar: return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Onbeschikbaar).Sum(r => r.Aantal);
                case Status.Gereserveerd: return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.Aantal);
            }
            return 0;
        }

        public int GeefAantalBeschikbaar(DateTime startDatum)
        {
            return AantalInCatalogus - Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Geblokkeerd).Sum(r => r.Aantal)
                              - Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Onbeschikbaar).Sum(r => r.Aantal)
                              - Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.Aantal);
        }
    }
}