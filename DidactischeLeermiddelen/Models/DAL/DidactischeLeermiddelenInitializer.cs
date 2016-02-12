﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class DidactischeLeermiddelenInitializer : DropCreateDatabaseAlways<DidactischeLeermiddelenContext>
    {
        protected override void Seed(DidactischeLeermiddelenContext context)
        {
            try
            {
                //Leergebieden
                Leergebied aardrijkskunde = new Leergebied { Naam = "Aardrijkskunde" };
                Leergebied fysica = new Leergebied { Naam = "Fysica" };
                Leergebied chemie = new Leergebied { Naam = "Chemie" };
                Leergebied wiskunde = new Leergebied { Naam = "Wiskunde" };
                Leergebied LO = new Leergebied { Naam = "L.O." };
                Leergebied Duits = new Leergebied { Naam = "Duits" };

                //Doelgroepen
                Doelgroep lagerOnderwijs = new Doelgroep { Naam = "Lager" };
                Doelgroep secundairOnderwijs = new Doelgroep { Naam = "Secundair" };


                //Materialen
                Materiaal wereldbol = new Materiaal { AantalInCatalogus = 4, ArtikelNr = 1, Firma = "Nova Rico", Naam = "Wereldbol", Foto = "/Content/Images/wereldbol.jpg", Omschrijving = "Columbus wereldbol", Prijs = 44.90M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { aardrijkskunde }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs } };
                Materiaal rekentoestel = new Materiaal { AantalInCatalogus = 20, ArtikelNr = 2, Firma = "Texas Instruments", Naam = "TI 84+", Foto = "/Content/Images/rekentoestel.jpg", Omschrijving = "Grafisch rekentoestel", Prijs = 106.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs } };
                Materiaal microscoopCeti = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 3, Firma = "Ceti", Naam = "Microscoop Ceti", Foto = "/Content/Images/microscoopCeti.jpg", Omschrijving = "Microscoop Ceti", Prijs = 534.00M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs } };
                Materiaal pincet = new Materiaal { AantalInCatalogus = 1, ArtikelNr = 4, Firma = "Zwilling", Naam = "Pincet", Foto = "/Content/Images/pincet.jpg", Omschrijving = "Pincet Zwilling", Prijs = 6.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs } };
                Materiaal bordGeodriekhoek = new Materiaal { AantalInCatalogus = 15, ArtikelNr = 5, Firma = "Wissner", Naam = "Bordgeodriehoek", Foto = "/Content/Images/geodriehoek.jpg", Omschrijving = "Geodriehoek om op het bord te gebruiken", Prijs = 16.00M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs } };
                Materiaal ReddingsPop = new Materiaal { AantalInCatalogus = 1, ArtikelNr = 6, Firma = "Witte merk", Naam = "Reddingspop", Foto = "/Content/Images/reddingspop.jpg", Omschrijving = "Met behulp van deze pop wordt je een geweldig duiker", Prijs = 245.00M, Status = Status.Gereserveerd, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO } };
                Materiaal Basketbal = new Materiaal { AantalInCatalogus = 30, ArtikelNr = 7, Firma = "Spalding", Naam = "Spalding basketbal", Foto = "/Content/Images/basketbal.jpg", Omschrijving = "Officiële NBA basketbal, hiermee scoort iedereen 3-punters", Prijs = 169.00M, Status = Status.Reserveerbaar, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO } };
                Materiaal Bok = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 8, Firma = "Moeder natuur", Naam = "Bok", Foto = "/Content/Images/bok.jpg", Omschrijving = "Niet de alledaagse bok van in de turnles", Prijs = 0.00M, Status = Status.TeLaat, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO } };
                Materiaal Duitser = new Materiaal { AantalInCatalogus = 1, ArtikelNr = 9, Firma = "SS", Naam = "Duitse hulp", Foto = "/Content/Images/adolf.jpg", Omschrijving = "Deze vriendelijke vertaler kan handig zijn als bijstand. Moeilijk in samenwerking met joden.", Prijs = 0.00M, Status = Status.Geblokkeerd, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, Leergebieden = new List<Leergebied> { Duits } };
                Materiaal[] materialen = new Materiaal[] { wereldbol, rekentoestel, microscoopCeti, pincet, bordGeodriekhoek,ReddingsPop, Basketbal, Bok,Duitser };


                context.Materialen.AddRange(materialen);
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                string s = "Fout creatie database ";
                foreach (var eve in e.EntityValidationErrors)
                {
                    s += String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.GetValidationResult());
                    foreach (var ve in eve.ValidationErrors)
                    {
                        s += String.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(s);
            }
        }
    }

}