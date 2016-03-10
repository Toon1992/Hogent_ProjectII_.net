using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.DtoObjects;
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
        public bool InVerlanglijst { get; set; }

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

        public void MaakReservatieLijstAan()
        {
            if (Reservaties == null)
            {
                Reservaties = new List<Reservatie>();
            }
        }
        public void AddReservatie(Reservatie reservatie)
        {          
            Reservaties.Add(reservatie);
        }

        //public int CheckNieuwAantal()
        //{
        //    //AantalInCatalogus = Reservaties.Count(r => r.Status.Equals(Status.Beschikbaar));
        //    return AantalInCatalogus;
        //}

        public int GeefAantalPerStatus(ReservatieState status, DateTime startDatum, DateTime eindDatum)
        {         
            if (status is Geblokkeerd)
            {
                return Reservaties.Where(r => r.OverschrijftMetReservatie(startDatum, eindDatum) && r.ReservatieState is Geblokkeerd).Sum(r => r.Aantal);
            }
            if(status is Gereserveerd)
            {
                return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.Aantal);
            }

            return 0;

        }

        public int GeefAantalBeschikbaar(DateTime startDatum, DateTime eindDatum, bool lector)
        {
            int aantal = AantalInCatalogus - Reservaties.Where(r =>r.OverschrijftMetReservatie(startDatum, eindDatum) &&
                             (r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald || r.ReservatieState is Overrulen)).Sum(r => r.Aantal);
            if (!lector)
            {
                aantal -= Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.Aantal);
            }
            return aantal <= 0 ? 0 : aantal;
        }

        public int GeefAantalBeschikbaarVoorBlokkering()
        {
            int aantal = AantalInCatalogus -
                         Reservaties.Where(r => !(r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald || r.ReservatieState is Overrulen))
                             .Sum(r => r.Aantal);         
            return aantal <= 0 ? 0 : aantal;
        }

        public ICollection<Reservatie> GeefNietGeblokkeerdeReservaties()
        {
           return Reservaties.Where(r => !(r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald || r.ReservatieState is Overrulen)).OrderBy(r => r.StartDatum).ToList();
        }

        public ICollection<Reservatie> GeeftReservatiesVanEenBepaaldeTijd(DateTime start)
        {
            return Reservaties.Where(r => r.StartDatum <= start && (!(r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald || r.ReservatieState is Overrulen))).ToList();
        }

        public List<ReservatieDataDTO> MaakLijstReservatieDataInRange(DateTime startDatumFilter, DateTime eindDatumFilter)
        {
            Dictionary<int, bool> checkReservaties = new Dictionary<int, bool>();
            List<ReservatieDataDTO> reservatieList = new List<ReservatieDataDTO>();
            foreach (var r in Reservaties.OrderByDescending(r => r.Gebruiker.GetType().Name).ThenBy(r => r.StartDatum))
            {
                if (r.StartDatum >= startDatumFilter && r.StartDatum <= eindDatumFilter)
                {
                    ReservatieDataDTO reservatieData = new ReservatieDataDTO
                    {
                        Aantal = AantalInCatalogus - r.Aantal,
                        StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, HulpMethode.GetIso8601WeekOfYear(r.StartDatum))
                    };

                    if (checkReservaties.ContainsKey(HulpMethode.GetIso8601WeekOfYear(r.StartDatum)))
                    {
                        var reservatie =
                            reservatieList.FirstOrDefault(
                                p =>
                                    p.StartDatum.Equals(HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year,
                                        HulpMethode.GetIso8601WeekOfYear(r.StartDatum))));
                        var aantal = reservatie.Aantal - r.Aantal;
                        reservatie.Aantal = aantal < 0 ? 0 : aantal;
                    }
                    else
                    {
                        checkReservaties.Add(HulpMethode.GetIso8601WeekOfYear(r.StartDatum), true);
                        reservatieList.Add(reservatieData);
                    }
                }
            }
            while (startDatumFilter <= eindDatumFilter)
            {
                if (!checkReservaties.ContainsKey(HulpMethode.GetIso8601WeekOfYear(startDatumFilter)))
                {
                    reservatieList.Add(new ReservatieDataDTO
                    {
                        Aantal = AantalInCatalogus,
                        StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, HulpMethode.GetIso8601WeekOfYear(startDatumFilter))
                    });
                }
                startDatumFilter = startDatumFilter.AddDays(7);
            }
            return reservatieList;
        }

        
    }
}