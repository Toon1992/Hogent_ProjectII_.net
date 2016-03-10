using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Mail;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Models.Domain
{
    public abstract class Gebruiker
    {
        public int GebruikerId { get; set; }
        public string Email { get; set; }
        public string Naam { get; set; }
        public string Faculteit { get; set; }
        public virtual Verlanglijst Verlanglijst { get; set; }
        public virtual IList<Reservatie> Reservaties { get; set; }

        public void VoegMateriaalAanVerlanglijstToe(Materiaal materiaal)
        {
            if (materiaal == null)
                throw new ArgumentNullException(
                    "Materiaal mag niet null zijn als die wordt toevoegd aan de verlanglijst!");

            //aan de Velanglijst materiaal Toevoegen      
            Verlanglijst.VoegMateriaalToe(materiaal);
        }

        public void VerwijderMateriaalUitVerlanglijst(Materiaal materiaal)
        {
            if (materiaal == null)
                throw new ArgumentNullException(
                    "Materiaal mag niet null zijn als die wordt verwijdert van de verlanglijst!");

            //Verwijderen van materiaal van de verlanglijst
            Verlanglijst.VerwijderMateriaal(materiaal);
        }

        public void VerwijderReservatie(Reservatie r)
        {
            if (Reservaties.Contains(r))
            {
                Reservaties.Remove(r);
            }
            else
            {
                throw new ArgumentException("Er is geen reservatie om te verwijderen");
            }

        }

        protected virtual void VoegReservatieToe(Materiaal materiaal, int aantal, string startdatum, string eindDatum)
        {
            Reservatie reservatie = new Reservatie(this, materiaal, startdatum, eindDatum, aantal)
            {
                Gebruiker = this
            };

            if (this is Lector)
                reservatie.Blokkeer();
            else
            {
                reservatie.ToState(new Gereserveerd());
            }
            //reservatie.ToState(new Gereserveerd());
            materiaal.AddReservatie(reservatie);
            Reservaties.Add(reservatie);

        }

        protected Reservatie MaakReservatieObject(Gebruiker gebruiker, Materiaal mat, string startdatum, string eindDatum,
            int aantal)
        {
            Reservatie reservatie = new Reservatie(gebruiker, mat, startdatum, eindDatum, aantal)
            {
                Gebruiker = this
            };

            if (reservatie == null)
            {
                throw new ArgumentNullException("Er is geen reservatie Object gemaakt");
            }

            return reservatie;
        }
    }
}