using System;
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
                Doelgroep kleuterOnderwijs=new Doelgroep {Naam = "Kleuter"};

                //ReservatieData
                ReservatieData reservatieData1 = new ReservatieData() {Aantal = 4, Week = 9};
                ReservatieData reservatieData2 = new ReservatieData() { Aantal = 2, Week = 9 };
                ReservatieData reservatieData3 = new ReservatieData() { Aantal = 6, Week = 10 };
                ReservatieData reservatieData4 = new ReservatieData() { Aantal = 13, Week = 10 };

                //Materialen
                Materiaal wereldbol = new Materiaal { AantalInCatalogus = 4, ArtikelNr = 42324, MateriaalId = 1, Firma = "Nova Rico", Naam = "Wereldbol", Foto = "/Content/Images/wereldbol.jpg", Omschrijving = "Columbus wereldbol", Prijs = 44.90M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { aardrijkskunde }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true, ReservatieData = new List<ReservatieData> { reservatieData1}};
                Materiaal rekentoestel = new Materiaal { AantalInCatalogus = 20, ArtikelNr = 53252, MateriaalId = 2, Firma = "Texas Instruments", Naam = "TI 84+", Foto = "/Content/Images/rekentoestel.jpg", Omschrijving = "Grafisch rekentoestel", Prijs = 106.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs } ,IsReserveerBaar = true, ReservatieData = new List<ReservatieData> { reservatieData1, reservatieData3 } };
                Materiaal microscoopCeti = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 6721, MateriaalId = 3, Firma = "Ceti", Naam = "Microscoop Ceti", Foto = "/Content/Images/microscoopCeti.jpg", Omschrijving = "Microscoop Ceti", Prijs = 534.00M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, IsReserveerBaar = true, ReservatieData = new List<ReservatieData> { reservatieData2 } };
                Materiaal pincet = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 6643, MateriaalId = 4, Firma = "Zwilling", Naam = "Pincet", Foto = "/Content/Images/pincet.jpg", Omschrijving = "Pincet Zwilling", Prijs = 6.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true , ReservatieData = new List<ReservatieData> { reservatieData2 } };
                Materiaal bordGeodriekhoek = new Materiaal { AantalInCatalogus = 15, ArtikelNr = 1223, MateriaalId = 5, Firma = "Wissner", Naam = "Bordgeodriehoek", Foto = "/Content/Images/geodriehoek.jpg", Omschrijving = "Geodriehoek om op het bord te gebruiken", Prijs = 16.00M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true, ReservatieData = new List<ReservatieData> { reservatieData2, reservatieData4 } };
                Materiaal ReddingsPop = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 68934, MateriaalId = 6, Firma = "Witte merk", Naam = "Reddingspop", Foto = "/Content/Images/reddingspop.jpg", Omschrijving = "Met behulp van deze pop wordt je een geweldig duiker", Prijs = 245.00M, Status = Status.Gereserveerd, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = false, ReservatieData = new List<ReservatieData> { reservatieData2 } };
                Materiaal Basketbal = new Materiaal { AantalInCatalogus = 30, ArtikelNr = 29188, MateriaalId = 7, Firma = "Spalding", Naam = "Spalding basketbal", Foto = "/Content/Images/basketbal.jpg", Omschrijving = "Officiële NBA basketbal, hiermee scoort iedereen 3-punters", Prijs = 169.00M, Status = Status.Reserveerbaar, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = false, ReservatieData = new List<ReservatieData> { reservatieData2, reservatieData4 } };
                Materiaal Bok = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 2441, MateriaalId = 8, Firma = "Moeder natuur", Naam = "Bok", Foto = "/Content/Images/bok.jpg", Omschrijving = "Niet de alledaagse bok van in de turnles", Prijs = 0.00M, Status = Status.TeLaat, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs,kleuterOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = true, ReservatieData = new List<ReservatieData> { reservatieData2 } };
                Materiaal Duitser = new Materiaal { AantalInCatalogus = 6, ArtikelNr = 9812, MateriaalId = 9, Firma = "Prisma", Naam = "Woordenboek Duits-Nederlands", Foto = "/Content/Images/woordenboek.jpg", Omschrijving = "Pocketwoordenboek Nederlands Duits", Prijs = 13.00M, Status = Status.Geblokkeerd, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, Leergebieden = new List<Leergebied> { Duits }, IsReserveerBaar = false, ReservatieData = new List<ReservatieData> { reservatieData3 } };
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