﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain.DtoObjects;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Materiaal
    {
        #region fields
        public string Naam { get; set; }
        public string Omschrijving { get; set; }

        public int AantalInCatalogus { get; set; }
        public int AantalOnbeschikbaar { get; set; }

        public int ArtikelNr { get; set; }
        public int MateriaalId { get; set; }
        public string Plaats { get; set; }

        public double Prijs { get; set; }

        public virtual Firma Firma { get; set; }
        public virtual IList<Reservatie> Reservaties { get; set; }
        public Boolean? IsReserveerBaar { get; set; }
        public Boolean? InVerlanglijst { get; set; }

        public virtual IList<Doelgroep> Doelgroepen { get; set; }
        public virtual IList<Leergebied> Leergebieden { get; set; }
        public byte[] Foto { get; set; }
        private string _imagesrc;
        public string ImageSrc
        {
            get
            {
                if (_imagesrc == null)
                {
                    _imagesrc = $"data:image/jpg;base64,{Convert.ToBase64String(Foto)}";
                }
                return _imagesrc;
            }
            set
            {
                Image img = Image.FromFile(value);

                using (var ms = new MemoryStream())
                {
                    img.Save(ms, img.RawFormat);
                    Foto = ms.ToArray();
                }
            }
        }
        // public bool Onbeschikbaar { get; set; }
        #endregion

        public Materiaal(string naam, int artikeNr, int aantal) : this()
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
                int aantal =
                    Reservaties.Where(
                        r => r.KanOverschrijvenMetReservatie(startDatum, eindDatum) && r.ReservatieState is Geblokkeerd)
                        .Sum(r => r.AantalGereserveerd);
                return aantal > AantalInCatalogus ? AantalInCatalogus : aantal;
            }

            if (status is Gereserveerd)
            {
                return Reservaties.Where(r => r.StartDatum.Equals(startDatum) && r.ReservatieState is Gereserveerd).Sum(r => r.AantalGereserveerd);
            }

            return 0;

        }

        public int GeefAantalBeschikbaar(DateTime startDatum, DateTime eindDatum,IList<DateTime> dagen , Gebruiker gebruiker)
        {
            int aantal = AantalInCatalogus;
            if (gebruiker is Lector && dagen != null)
            {
                foreach (var dag in dagen)
                {
                    IEnumerable<Reservatie> overschijvendeReservaties = Reservaties.Where(r => r.GeblokkeerdeDagen.Select(d => d.Datum).Contains(dag)).ToList();
                    int aantalGereserveerd = overschijvendeReservaties.Sum(r => r.AantalGereserveerd);
                    aantal = Math.Min(aantal, AantalInCatalogus - aantalGereserveerd);
                }       
            }
            else if (gebruiker is Student)
            {
                aantal = AantalInCatalogus -
                         Reservaties.Where(r => r.KanOverschrijvenMetReservatie(startDatum, eindDatum) &&
                                                (r.ReservatieState is Geblokkeerd || r.ReservatieState is Gereserveerd))
                             .Sum(r => r.AantalGereserveerd);
            }
            return aantal <= 0 ? 0 : aantal;
        }

        public int GeefAantalGeselecteerd(Dictionary<int, int> materiaalAantal, int aantalBeschikbaar,
            int aantalGeselecteerd)
        {
            int aantal = 0;
            if (materiaalAantal.ContainsKey(MateriaalId))
            {
                if (aantalBeschikbaar == 0)
                {
                    return 0;
                }
                if (aantalBeschikbaar < materiaalAantal[MateriaalId])
                {
                    return aantalBeschikbaar;
                }
                return materiaalAantal[MateriaalId];
            }
            if (aantalGeselecteerd != 0)
            {
                return aantalGeselecteerd > aantalBeschikbaar ? aantalBeschikbaar : aantalGeselecteerd;
            }
            return aantal;
        }

        public int GeefAantalBeschikbaarVoorBlokkering()
        {
            int aantal = AantalInCatalogus -
                         Reservaties.Where(r => r.ReservatieState is Gereserveerd || r.ReservatieState is Geblokkeerd || r.ReservatieState is Opgehaald)
                             .Sum(r => r.AantalGereserveerd);         
            return aantal <= 0 ? 0 : aantal;
        }

        public ICollection<Reservatie> GeeftReservatiesVanEenBepaaldeTijd(DateTime start)
        {
            return Reservaties.Where(r => r.StartDatum <= start && (!(r.ReservatieState is Opgehaald || r.ReservatieState is Overruled))).ToList();
        } 
        public Dictionary<DateTime, ICollection<Reservatie>> ReservatieDetails(int week)
        {
            Dictionary<DateTime, ICollection<Reservatie>> reservatieMap = new Dictionary<DateTime, ICollection<Reservatie>>();
            var reservaties = Reservaties;

            if (week > -1)
            {
                var geselecteerdeDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week);
                reservaties = reservaties.Where(r => r.StartDatum.Equals(geselecteerdeDatum)).ToList();
            }

            foreach (Reservatie reservatie in reservaties)
            {
                if (week < 0 || reservatie.StartDatum.Equals(HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week))) { }
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
                            reservatieMap[e.Key].Add(reservatie);
                            break;
                        }
                    }

                    if (!overschrijft)
                    {
                        int wk = HulpMethode.GetIso8601WeekOfYear(reservatie.StartDatum);
                        DateTime date = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, wk);

                        if (reservatieMap.ContainsKey(date))
                        {
                            reservatieMap[date].Add(reservatie);
                        }
                        else
                        {
                            reservatieMap.Add(date,new List<Reservatie> {reservatie});
                        }
                    }
                }
                else if (reservatie.Gebruiker is Student)
                {
                    if (!reservatieMap.ContainsKey(reservatie.StartDatum))
                    {
                        reservatieMap.Add(reservatie.StartDatum, new List<Reservatie> { reservatie});
                    }
                    else
                    {
                        reservatieMap[reservatie.StartDatum].Add(reservatie);
                    }
                }
            }
            return reservatieMap;
        }
        public Dictionary<DateTime, int[]> MaakLijstReservatieDataInRange(DateTime startDatumFilter, DateTime eindDatumFilter)
        {
            Dictionary<DateTime, int[]> reservatieMap = new Dictionary<DateTime, int[]>();

            //De reservaties overlopen en reservatieDataDTO objecten met juiste waarden maken.
            foreach (var r in Reservaties.Where(r => !(r.ReservatieState is Overruled)).OrderBy(r => r.StartDatum))
            {
                if (r.StartDatum >= startDatumFilter && r.StartDatum <= eindDatumFilter)
                {
                    reservatieMap = UpdateReservatieMap(reservatieMap, r.StartDatum, r.AantalGereserveerd);
                }
            }
            //Voor de data waar geen reservaties zijn worden reservatieDataDTO objecten met standaardWaarden gemaakt.
            while (startDatumFilter <= eindDatumFilter)
            {
                reservatieMap = UpdateReservatieMap(reservatieMap, startDatumFilter, 0);
                startDatumFilter = startDatumFilter.AddDays(7);
            }

            return reservatieMap;
        }

        public Dictionary<DateTime, int[]> MaakLijstReservatieDataSpecifiekeDagen(DateTime[] dagen)
        {
            Dictionary<DateTime, int[]> reservatieMap = new Dictionary<DateTime, int[]>();

            //De reservaties overlopen en reservatieDataDTO objecten met juiste waarden maken.
            foreach (var r in Reservaties.Where(r => !(r.ReservatieState is Overruled) && r.Gebruiker is Lector).OrderBy(r => r.StartDatum))
            {
                IList<DateTime> gemeenschappelijkeDagen = r.GeblokkeerdeDagen.Select(g => g.Datum).Intersect(dagen).ToList();
                if (gemeenschappelijkeDagen.Any())
                {
                    foreach (var dag in gemeenschappelijkeDagen)
                    {
                        reservatieMap = UpdateReservatieMap(reservatieMap, dag, r.AantalGereserveerd);
                    }                   
                }
            }
            var dagenZonderReservaties = dagen.Where(dag => !Reservaties.Any(r => r.GeblokkeerdeDagen.Select(d => d.Datum).Contains(dag)));
            foreach (var dag in dagenZonderReservaties)
            {
                reservatieMap = UpdateReservatieMap(reservatieMap, dag, 0);
            }
            
            return reservatieMap;
        }


        public Dictionary<DateTime, int[]> UpdateReservatieMap(Dictionary<DateTime, int[]> reservatieMap, DateTime startDatum, int aantal)
        {
            if (reservatieMap.ContainsKey(startDatum))
            {
                //Indien negatief op null zetten.

                reservatieMap[startDatum][0] -= aantal;
                if (reservatieMap[startDatum][0] < 0)
                {
                    reservatieMap[startDatum][0] = 0;
                }
            }
            else
            {
                reservatieMap.Add(startDatum, new []{ AantalInCatalogus - aantal, MateriaalId} );
            }
            return reservatieMap;
        } 
    }
}