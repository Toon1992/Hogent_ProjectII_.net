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
              

                //StatusData
                //Altijd beschikbaar
                List<StatusData> statusData1 = new List<StatusData>();
                for (int i = 1; i < 53; i++)
                {
                    statusData1.Add(new StatusData
                    {
                        Status = Status.Beschikbaar,
                        Week = i
                    });
                }

                //Beschikbaar van week 12
                List<StatusData> statusData2 = new List<StatusData>();
                for (int i = 1; i < 53; i++)
                {
                    statusData2.Add(new StatusData
                    {
                        Status = i < 11 ? Status.Gereserveerd : Status.Beschikbaar,
                        Week = i
                    });
                }
                //Beschikbaar tot week 11
                List<StatusData> statusData3 = new List<StatusData>();
                for (int i = 1; i < 53; i++)
                {
                    statusData3.Add(new StatusData
                    {
                        Status = i > 11 ? Status.Gereserveerd : Status.Beschikbaar,
                        Week = i
                    });
                }

                //Nooit beschikbaar
                List<StatusData> statusData4 = new List<StatusData>();
                for (int i = 1; i < 53; i++)
                {
                    statusData4.Add(new StatusData
                    {
                        Status = Status.Gereserveerd,
                        Week = i
                    });
                }


                //Stuks
                List<Stuk> wereldbolstuks = new List<Stuk>();
                for (int i = 1; i < 5; i++)
                {
                    wereldbolstuks.Add(new Stuk()
                    {
                        StukId = 1111+i,
                        StatusData = statusData2
                    });
                }
                List<Stuk> rekentoestelStuks = new List<Stuk>();
                for (int i = 1; i < 21; i++)
                {
                    rekentoestelStuks.Add(new Stuk()
                    {
                        StukId = 2222 + i,
                        StatusData = i < 11 ? statusData2: statusData3
                    });
                }
                List<Stuk> microscoopStuks = new List<Stuk>();
                for (int i = 1; i < 3; i++)
                {
                    microscoopStuks.Add(new Stuk()
                    {
                        StukId = 3333 + i,
                        StatusData = statusData4
                    });
                }

                List<Stuk> pincetStuks = new List<Stuk>();
                for (int i = 1; i < 3; i++)
                {
                    pincetStuks.Add(new Stuk()
                    {
                        StukId = 4444 + i,
                        StatusData = statusData1
                    });
                }
                List<Stuk> geodriehoekStuks = new List<Stuk>();
                for (int i = 1; i < 16; i++)
                {
                    geodriehoekStuks.Add(new Stuk()
                    {
                        StukId = 5555 + i,
                        StatusData = i < 5 ? statusData2 : statusData3
                    });
                }
                List<Stuk> reddingspopStuks = new List<Stuk>();
                for (int i = 1; i < 6; i++)
                {
                    reddingspopStuks.Add(new Stuk()
                    {
                        StukId = 6666 + i,
                        StatusData = i < 3 ? statusData1 : statusData3
                    });
                }
                List<Stuk> basketbalStuks = new List<Stuk>();
                for (int i = 1; i < 31; i++)
                {
                    basketbalStuks.Add(new Stuk()
                    {
                        StukId = 7777 + i,
                        StatusData = i < 13 ? statusData1 : statusData3
                    });
                }
                List<Stuk> bokStuks = new List<Stuk>();
                for (int i = 1; i < 2; i++)
                {
                    bokStuks.Add(new Stuk()
                    {
                        StukId = 8888 + i,
                        StatusData = statusData4
                    });
                }
                List<Stuk> duitsStuks = new List<Stuk>();
                for (int i = 1; i < 22; i++)
                {
                    duitsStuks.Add(new Stuk()
                    {
                        StukId = 9999 + i,
                        StatusData = statusData1
                    });
                }

                Firma f=new Firma("Ceti","ceti@gmail.com","ceti.be",contactpersoon:"Silke");
                Firma b = new Firma("Wissner", "wissner@gmail.com","wissner.com",adres:"Voskenslaan", contactpersoon: "Silke");
                Firma c = new Firma("Texas Instruments", "instruments@gmail.com","texasinstruments.com"); //veranderen van firma werkt niet, blijft bij eerst initialisatie
                //Materialen
                Materiaal wereldbol = new Materiaal {AantalInCatalogus = 4,ArtikelNr = 1111, MateriaalId = 1, Firma = b,Naam = "Wereldbol", Foto = "/Content/Images/wereldbol.jpg", Omschrijving = "Columbus wereldbol", Prijs = 44.90M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { aardrijkskunde }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true, Stuks = wereldbolstuks};
                Materiaal rekentoestel = new Materiaal { AantalInCatalogus = 20, ArtikelNr = 2222, MateriaalId = 2, Firma = c, Naam = "TI 84+", Foto = "/Content/Images/rekentoestel.jpg", Omschrijving = "Grafisch rekentoestel", Prijs = 106.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, IsReserveerBaar = true, Stuks = rekentoestelStuks };
                Materiaal microscoopCeti = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 3333, MateriaalId = 3, Firma = f, Naam = "Microscoop Ceti", Foto = "/Content/Images/microscoopCeti.jpg", Omschrijving = "Microscoop Ceti", Prijs = 534.00M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, IsReserveerBaar = true, Stuks = microscoopStuks };
                Materiaal pincet = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 4444, MateriaalId = 4, Firma = b, Naam = "Pincet", Foto = "/Content/Images/pincet.jpg", Omschrijving = "Pincet Zwilling", Prijs = 6.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true, Stuks = pincetStuks };
                Materiaal bordGeodriekhoek = new Materiaal { AantalInCatalogus = 15, ArtikelNr = 5555, MateriaalId = 5, Firma = c, Naam = "Bordgeodriehoek", Foto = "/Content/Images/geodriehoek.jpg", Omschrijving = "Geodriehoek om op het bord te gebruiken", Prijs = 16.00M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true, Stuks = geodriehoekStuks };
                Materiaal ReddingsPop = new Materiaal { AantalInCatalogus = 5, ArtikelNr = 6666, MateriaalId = 6, Firma = f, Naam = "Reddingspop", Foto = "/Content/Images/reddingspop.jpg", Omschrijving = "Met behulp van deze pop wordt je een geweldig duiker", Prijs = 245.00M, Status = Status.Gereserveerd, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = false, Stuks = reddingspopStuks };
                Materiaal Basketbal = new Materiaal { AantalInCatalogus = 30, ArtikelNr = 7777, MateriaalId = 7, Firma = b, Naam = "Spalding basketbal", Foto = "/Content/Images/basketbal.jpg", Omschrijving = "Officiële NBA basketbal, hiermee scoort iedereen 3-punters", Prijs = 169.00M, Status = Status.Beschikbaar, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = false, Stuks = basketbalStuks};
                Materiaal Bok = new Materiaal { AantalInCatalogus = 1, ArtikelNr = 8888, MateriaalId = 8, Firma = c, Naam = "Bok", Foto = "/Content/Images/bok.jpg", Omschrijving = "Niet de alledaagse bok van in de turnles", Prijs = 0.00M, Status = Status.TeLaat, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs, kleuterOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = true, Stuks = bokStuks };
                Materiaal Duitser = new Materiaal { AantalInCatalogus = 21, ArtikelNr = 9999, MateriaalId = 9, Firma = f, Naam = "Woordenboek Duits-Nederlands", Foto = "/Content/Images/woordenboek.jpg", Omschrijving = "Pocketwoordenboek Nederlands Duits", Prijs = 13.00M, Status = Status.Geblokkeerd, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, Leergebieden = new List<Leergebied> { Duits }, IsReserveerBaar = false, Stuks = duitsStuks };
                Materiaal[] materialen = new Materiaal[] { wereldbol, rekentoestel, microscoopCeti, pincet, bordGeodriekhoek, ReddingsPop, Basketbal, Bok, Duitser };


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