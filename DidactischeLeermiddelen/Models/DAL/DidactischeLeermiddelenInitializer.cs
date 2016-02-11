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
                Leergebied aardrijkskunde = new Leergebied {Naam = "Aardrijkskunde"};
                Leergebied fysica = new Leergebied {Naam = "Fysica"};
                Leergebied chemie = new Leergebied { Naam = "Chemie" };
                Leergebied wiskunde = new Leergebied { Naam = "Wiskunde" };

                //Doelgroepen
                Doelgroep lagerOnderwijs = new Doelgroep {Naam = "Lager onderwijs"};
                Doelgroep secundairOnderwijs = new Doelgroep { Naam = "Secundair onderwijs" };

                //Materialen
                Materiaal wereldbol = new Materiaal("Wereldbol",1,4) {Firma = "Nova Rico", Foto = "~/Content/Images/wereldbol.jpg", Omschrijving = "Columbus wereldbol",Prijs = 44.90M,Status = Status.Catalogus,Leergebieden = new List<Leergebied>{aardrijkskunde},Doelgroepen = new List<Doelgroep>{lagerOnderwijs, secundairOnderwijs}};
                Materiaal rekentoestel = new Materiaal("TI84+", 20, 2) { Firma = "Texas Instruments", Foto = "~/Content/Images/rekentoestel.jpg", Omschrijving = "Grafisch rekentoestel", Prijs = 106.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs } };
                Materiaal microscoopCeti = new Materiaal("Microscoop Ceti", 2, 3) { Firma = "Ceti", Foto = "~/Content/Images/mircoscoopCeti.jpg", Omschrijving = "Microscoop Ceti", Prijs = 534M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { chemie }, Doelgroepen = new List<Doelgroep> { secundairOnderwijs } };
                Materiaal pincet = new Materiaal("Pincet", 1, 4) {  Firma = "Zwilling", Foto = "~/Content/Images/pincet.jpg", Omschrijving = "Pincet Zwilling", Prijs = 6.95M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs,secundairOnderwijs } };
                Materiaal bordGeodriekhoek = new Materiaal("bordGeoDriehoek", 1, 4) {Firma = "Wissner", Foto = "~/Content/Images/geodriehoek.jpg", Omschrijving = "Geodriehoek om op het bord te gebruiken", Prijs = 16M, Status = Status.Catalogus, Leergebieden = new List<Leergebied> { wiskunde, fysica, chemie }, Doelgroepen = new List<Doelgroep> { lagerOnderwijs, secundairOnderwijs } };
                Materiaal[] materialen = new Materiaal[] {wereldbol, rekentoestel, microscoopCeti, pincet, bordGeodriekhoek};
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