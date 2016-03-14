﻿using System;
using System.Collections.Generic;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain.DtoObjects;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;

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

        public decimal Prijs { get; set; }

        public virtual Firma Firma { get; set; }
        public virtual IList<Reservatie> Reservaties { get; set; }
        public bool IsReserveerBaar { get; set; }
        public bool InVerlanglijst { get; set; }

        public virtual IList<Doelgroep> Doelgroepen { get; set; }
        public virtual IList<Leergebied> Leergebieden { get; set; }

       // public bool Onbeschikbaar { get; set; }
        #endregion

        public Materiaal(string naam, int artikeNr, int aantal):this()
        {
            Naam = naam;
            ArtikelNr = artikeNr;
            AantalInCatalogus = aantal;
                Reservaties = new List<Reservatie>();
            }
        public Materiaal() { Reservaties = new List<Reservatie>(); }

        public void AddReservatie(Reservatie reservatie)
        {          
            Reservaties.Add(reservatie);
        }

        public int GeefAantalPerStatus(ReservatieState status, DateTime startDatum, DateTime eindDatum)
        {         
            if (status is Geblokkeerd)
            {
                return Reservaties.Where(r => r.KanOverschrijvenMetReservatie(startDatum, eindDatum) && r.ReservatieState is Geblokkeerd).Sum(r => r.Aantal);
            }
            if(status is Gereserveerd)
            {
                return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.Aantal);
            }

            return 0;

        }

        public int GeefAantalBeschikbaar(DateTime startDatum, DateTime eindDatum, Gebruiker gebruiker)
        {
            int aantal = 0;
            if (gebruiker is Lector)
            {
                aantal = AantalInCatalogus - Reservaties.Where(r => r.Dagen.Select(d => d.Datum).Contains(startDatum)).Sum(r => r.Aantal);
            }
            else if (gebruiker is Student)
            {
                aantal = AantalInCatalogus -
                         Reservaties.Where(r => r.KanOverschrijvenMetReservatie(startDatum, eindDatum) &&
                                                (r.ReservatieState is Geblokkeerd || r.ReservatieState is Gereserveerd))
                             .Sum(r => r.Aantal);
            }
            return aantal <= 0 ? 0 : aantal;
        }
        public int GeefAantalBeschikbaarVoorBlokkering()
        {
            int aantal = AantalInCatalogus -
                         Reservaties.Where(r => !(r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald || r.ReservatieState is Overruled))
                             .Sum(r => r.Aantal);         
            return aantal <= 0 ? 0 : aantal;
        }

        public ICollection<Reservatie> GeefNietGeblokkeerdeReservaties()
        {
           return Reservaties.Where(r => !(r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald || r.ReservatieState is Overruled)).OrderBy(r => r.StartDatum).ToList();
        }

        public ICollection<Reservatie> GeeftReservatiesVanEenBepaaldeTijd(DateTime start)
        {
            return Reservaties.Where(r => r.StartDatum <= start && (!(r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald || r.ReservatieState is Overruled))).ToList();
        } 
        public Dictionary<DateTime, ICollection<ReservatieDetailViewModel>> ReservatieDetails(int week)
        {
            Dictionary<DateTime, ICollection<ReservatieDetailViewModel>> reservatieMap = new Dictionary<DateTime, ICollection<ReservatieDetailViewModel>>();
            var reservaties = Reservaties;
            if (week > -1)
            {
                var geselecteerdeDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week);
                reservaties = reservaties.Where(r => r.StartDatum.Equals(geselecteerdeDatum)).ToList();
            }
            foreach (Reservatie reservatie in reservaties)
            {
                if(week < 0 || reservatie.StartDatum.Equals(HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week))) { }
                if (reservatie.Gebruiker is Lector)
                {
                    bool overschrijft = false;
                    //Kijken of de blokkering van de lector een reservatie van de student overschrijft.
                    //Zoja, dan wordt deze in dezelfde week als die van de student geplaatst.
                    foreach (var e in reservatieMap)
                    {
                        DateTime startStudent = Convert.ToDateTime(e.Key);
                        DateTime eindStudent = startStudent.AddDays(4);
                        overschrijft = reservatie.KanOverschrijvenMetReservatie(startStudent, eindStudent);
                        if (overschrijft)
                        {
                            reservatieMap[e.Key].Add(CreateReservatieDetailVm(reservatie));
                            break;
                        }
                    }

                    if (!overschrijft)
                    {
                        int wk = HulpMethode.GetIso8601WeekOfYear(reservatie.StartDatum);
                        DateTime date = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, wk);

                        if (reservatieMap.ContainsKey(date))
                        {
                            reservatieMap[date].Add(CreateReservatieDetailVm(reservatie));
                        }
                        else
                        {
                            reservatieMap.Add(date, CreateListReservatieDetailVm(reservatie));
                        }
                    }
                }
                else if (reservatie.Gebruiker is Student)
                {
                    if (!reservatieMap.ContainsKey(reservatie.StartDatum))
                    {
                        reservatieMap.Add(reservatie.StartDatum, CreateListReservatieDetailVm(reservatie));
                    }
                    else
                    {
                        reservatieMap[reservatie.StartDatum].Add(CreateReservatieDetailVm(reservatie));
                    }
                }
            }
            return reservatieMap;
        }
        public List<ReservatieDetailViewModel> CreateListReservatieDetailVm(Reservatie reservatie)
        {
            return new List<ReservatieDetailViewModel> { CreateReservatieDetailVm(reservatie) };
        }
        public ReservatieDetailViewModel CreateReservatieDetailVm(Reservatie reservatie)
        {
            ViewModelFactory factory = new ReservatieDetailViewModelFactory();
            return factory.CreateReservatieDetailViewModel(reservatie) as ReservatieDetailViewModel;
        }
        public List<ReservatieDataDTO> MaakLijstReservatieDataInRange(DateTime startDatumFilter, DateTime eindDatumFilter)
        {
            List<ReservatieDataDTO> reservatieList = new List<ReservatieDataDTO>();

            //De reservaties overlopen en reservatieDataDTO objecten met juiste waarden maken.
            foreach (var r in Reservaties.Where(r => !(r.ReservatieState is Overruled)).OrderByDescending(r => r.Gebruiker.GetType().Name).ThenBy(r => r.StartDatum))
            {
                if (r.StartDatum >= startDatumFilter && r.StartDatum <= eindDatumFilter)
                {
                    reservatieList = UpdateReservatieDataDtoLijst(reservatieList, r.StartDatum, r.Aantal);
                }
            }

            //Voor de data waar geen reservaties zijn worden reservatieDataDTO objecten met standaardWaarden gemaakt.
            while (startDatumFilter <= eindDatumFilter)
            {
                reservatieList = UpdateReservatieDataDtoLijst(reservatieList, startDatumFilter, 0);
                startDatumFilter = startDatumFilter.AddDays(7);
            }

            return reservatieList;
        }

        public List<ReservatieDataDTO> UpdateReservatieDataDtoLijst(List<ReservatieDataDTO> reservatieList, DateTime startDatum, int aantal)
        {
            ReservatieDataDTO data = GeefDataDtoOpDatum(reservatieList, startDatum);

            if (data == null)
            {
                reservatieList.Add(new ReservatieDataDTO
                {
                    Aantal = AantalInCatalogus - aantal,
                    StartDatum = GeefEersteDagVanWeek(startDatum)
                });
            }
            else
            {
                data.Aantal -= aantal;
            }

            return reservatieList;
        }
        public ReservatieDataDTO GeefDataDtoOpDatum(List<ReservatieDataDTO> lijst, DateTime datum)
        {
            var maandag = GeefEersteDagVanWeek(datum);
            return lijst.FirstOrDefault(e => e.StartDatum.Equals(maandag));
        }

        public DateTime GeefEersteDagVanWeek(DateTime datum)
        {
            return HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year,
                        HulpMethode.GetIso8601WeekOfYear(datum));
        }
    }
}