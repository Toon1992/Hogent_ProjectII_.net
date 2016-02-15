using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Gebruiker
    {
        #region fields
        public string Naam { get; set; }

        public bool IsLector { get; set; }

        public string Email { get; set; }

        public virtual List<Reservatie> Reservaties { get; set; }

        public virtual Verlanglijst Verlanglijst { get; set; }
        #endregion

        #region Methodes

        public Gebruiker()
        {
        }
        public void VoegMateriaalAanVerlanglijstToe(Materiaal materiaal, int aantal)
        {
            //Materiaal dat doorgevoerd wordt naar de verlanglijst mag niet null zijn
            if (materiaal == null)
                throw new ArgumentNullException("Materiaal mag niet null zijn");

            //Heeft verlanglijst al dit materiaal in zijn lijst staan?
            //Ja, niks doen
            //Nee, toevoegen aan de lijst
            if (aantal > materiaal.AantalInCatalogus || aantal <= 0)
                throw new ArgumentException("Het opgegeven aantal is te groot, gelieve een aantal te kiezen tussen 1 en het aantal in de catalogus");
            if (!Verlanglijst.BevatMateriaal(materiaal))
                Verlanglijst.VoegMateriaalToe(materiaal, aantal);
        }

        //public void VoegMateriaalAanVerlanglijstToe(String fotoSource, String naam, String omschrijving, int aantalInCatalogus, int artikelNr, decimal prijs, String firma, Doelgroep doelgroep, Leergebied leergebied, int aantal)
        //{
        //    //Materiaal heeft altijd een naam
        //    if(string.IsNullOrEmpty(naam) || naam.Trim().Equals(""))
        //        throw new ArgumentException("Naam van materiaal moet ingevoerd zijn");

        //    //Materiaal heeft altijd een artikelNr
        //    if (artikelNr <= 0)
        //        throw new ArgumentException("ArtikelNr moet er altijd zijn");

        //    Materiaal materiaal = new Materiaal(naam,artikelNr,aantalInCatalogus)
        //    {
        //        Foto = fotoSource,              
        //        Doelgroepen = new List<Doelgroep>() {doelgroep},
        //        Firma = firma,
        //        Leergebieden = new List<Leergebied>() {leergebied},
        //        Omschrijving = omschrijving,
        //        Prijs = prijs,
        //        Status = Status.Catalogus
        //    };

        //    //Heeft verlanglijst al dit materiaal in zijn lijst staan?
        //    //Ja, niks doen
        //    //Nee, toevoegen aan de lijst
        //    if (aantal > materiaal.AantalInCatalogus)
        //        throw new ArgumentException("Het opgegeven aantal is te groot, gelieve een aantal te kiezen tussen 1 en het aantal in de catalogus");

        //    if (!Verlanglijst.BevatMateriaal(materiaal))
        //        Verlanglijst.VoegMateriaalToe(materiaal, aantal);
        //}

        public void VoegReservatieToe(Reservatie reservatie)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}