using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
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

       // public bool Onbeschikbaar { get; set; }
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

        //public int CheckNieuwAantal()
        //{
        //    //AantalInCatalogus = Reservaties.Count(r => r.Status.Equals(Status.Beschikbaar));
        //    return AantalInCatalogus;
        //}

        public int GeefAantal(ReservatieState status, DateTime startDatum)
        {
            DateTime eindDatum = startDatum.AddDays(4);

            if (status is Geblokkeerd)
            {
                return Reservaties.Where(r =>
                      ((r.StartDatum <= startDatum && r.EindDatum <= eindDatum && r.EindDatum >= startDatum) ||
                       (r.StartDatum >= startDatum && r.EindDatum <= eindDatum) ||
                       (r.StartDatum <= startDatum && r.EindDatum >= eindDatum) ||
                       (r.StartDatum >= startDatum && r.EindDatum >= eindDatum && r.StartDatum <= eindDatum)) &&
                      r.ReservatieState is Geblokkeerd).Sum(r => r.Aantal);
            }
            else if (status is Onbeschikbaar)
            {
                return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Onbeschikbaar).Sum(r => r.Aantal);
            }
            else if(status is Gereserveerd)
            {
                return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.Aantal);
            }

            return 0;

        }

        public int GeefAantalBeschikbaar(DateTime startDatum)
        {
            DateTime eindDatum = startDatum.AddDays(4);
            int aantal= AantalInCatalogus 
                              - Reservaties.Where(r =>
                                ((r.StartDatum <= startDatum && r.EindDatum <= eindDatum && r.EindDatum >= startDatum) ||
                                (r.StartDatum >= startDatum && r.EindDatum <= eindDatum) ||
                                (r.StartDatum <= startDatum && r.EindDatum >= eindDatum) ||
                                (r.StartDatum >= startDatum && r.EindDatum >= eindDatum && r.StartDatum <= eindDatum)) &&
                                r.ReservatieState is Geblokkeerd).Sum(r => r.Aantal)
                              - Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Onbeschikbaar).Sum(r => r.Aantal)
                              - Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.Aantal);
            return aantal <= 0 ? 0 : aantal;
        }

        public int GeefAantalBeschikbaarLector(DateTime startDatum, DateTime eindDatum)
        {
            int aantal = AantalInCatalogus -
                         Reservaties.Where(r =>
                             ((r.StartDatum <= startDatum && r.EindDatum <= eindDatum && r.EindDatum >= startDatum) ||
                              (r.StartDatum >= startDatum && r.EindDatum <= eindDatum) ||
                              (r.StartDatum <= startDatum && r.EindDatum >= eindDatum) ||
                              (r.StartDatum >= startDatum && r.EindDatum >= eindDatum && r.StartDatum <= eindDatum)) &&
                             (r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald)  ).Sum(r => r.Aantal);
            return aantal <= 0 ? 0 : aantal;
        }
    }
}