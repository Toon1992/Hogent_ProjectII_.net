using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.Mail;

namespace DidactischeLeermiddelen.Models.DAL
{
    public class DidactischeLeermiddelenInitializer : DropCreateDatabaseIfModelChanges<DidactischeLeermiddelenContext>
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



                Firma f=new Firma("Ceti","ceti@gmail.com","ceti.be",contactpersoon:"Silke");
                Firma b = new Firma("Wissner", "wissner@gmail.com","wissner.com",adres:"Voskenslaan", contactpersoon: "Silke");
                Firma c = new Firma("Texas Instruments", "instruments@gmail.com","texasinstruments.com"); //veranderen van firma werkt niet, blijft bij eerst initialisatie
              
                //Materialen
                Materiaal wereldbol = new Materiaal {AantalInCatalogus = 4,ArtikelNr = 1111, MateriaalId = 1, Firma = b,Naam = "Wereldbol", Foto = "/Content/Images/wereldbol.jpg", Omschrijving = "Columbus wereldbol", Prijs = 44.90M, Leergebieden = new List<Leergebied> { aardrijkskunde }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true};
                Materiaal rekentoestel = new Materiaal { AantalInCatalogus = 20, ArtikelNr = 2222, MateriaalId = 2, Firma = c, Naam = "TI 84+", Foto = "/Content/Images/rekentoestel.jpg", Omschrijving = "Grafisch rekentoestel", Prijs = 106.95M, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, IsReserveerBaar = true };
                Materiaal microscoopCeti = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 3333, MateriaalId = 3, Firma = f, Naam = "Microscoop Ceti", Foto = "/Content/Images/microscoopCeti.jpg", Omschrijving = "Microscoop Ceti", Prijs = 534.00M, Leergebieden = new List<Leergebied> { chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, IsReserveerBaar = true};
                Materiaal pincet = new Materiaal { AantalInCatalogus = 2, ArtikelNr = 4444, MateriaalId = 4, Firma = b, Naam = "Pincet", Foto = "/Content/Images/pincet.jpg", Omschrijving = "Pincet Zwilling", Prijs = 6.95M,Leergebieden = new List<Leergebied> { fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true};
                Materiaal bordGeodriekhoek = new Materiaal { AantalInCatalogus = 15, ArtikelNr = 5555, MateriaalId = 5, Firma = c, Naam = "Bordgeodriehoek", Foto = "/Content/Images/geodriehoek.jpg", Omschrijving = "Geodriehoek om op het bord te gebruiken", Prijs = 16.00M,  Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, IsReserveerBaar = true };
                Materiaal ReddingsPop = new Materiaal { AantalInCatalogus = 5, ArtikelNr = 6666, MateriaalId = 6, Firma = f, Naam = "Reddingspop", Foto = "/Content/Images/reddingspop.jpg", Omschrijving = "Met behulp van deze pop wordt je een geweldig duiker", Prijs = 245.00M, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = false};
                Materiaal Basketbal = new Materiaal { AantalInCatalogus = 30, ArtikelNr = 7777, MateriaalId = 7, Firma = b, Naam = "Spalding basketbal", Foto = "/Content/Images/basketbal.jpg", Omschrijving = "Officiële NBA basketbal, hiermee scoort iedereen 3-punters", Prijs = 169.00M,  Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = false};
                Materiaal Bok = new Materiaal { AantalInCatalogus = 1, ArtikelNr = 8888, MateriaalId = 8, Firma = c, Naam = "Bok", Foto = "/Content/Images/bok.jpg", Omschrijving = "Niet de alledaagse bok van in de turnles", Prijs = 0.00M,  Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs, kleuterOnderwijs }, Leergebieden = new List<Leergebied> { LO }, IsReserveerBaar = true};
                Materiaal Duitser = new Materiaal { AantalInCatalogus = 21, ArtikelNr = 9999, MateriaalId = 9, Firma = f, Naam = "Woordenboek Duits-Nederlands", Foto = "/Content/Images/woordenboek.jpg", Omschrijving = "Pocketwoordenboek Nederlands Duits", Prijs = 13.00M, Doelgroepen = new List<Doelgroep> { secundairOnderwijs }, Leergebieden = new List<Leergebied> { Duits }, IsReserveerBaar = false};
                Materiaal[] materialen = new Materiaal[] { wereldbol, rekentoestel, microscoopCeti, pincet, bordGeodriekhoek, ReddingsPop, Basketbal, Bok, Duitser };
                          
                //rekentoestel.AddReservatie(reservatie);
                //bordGeodriekhoek.AddReservatie(reservatie2);

                context.Materialen.AddRange(materialen);

                MailService mail = new MailNaReservatie()
                {
                    Body = string.Format("<p>Dag </br> _NAAM</p>%n<p> Hieronder vind je terug wat je zonet reserveerde: </p>%n" +
                                         "<ul> "
                                         ),
                    Subject = "Bevestiging reservatie"

                };
                context.MailTemplates.Add(mail);
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