using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;
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

        public bool ControleMateriaalGeselecteerdeDatum(List<Materiaal> materialen)
        {
            return false;
        }
        public bool ControleGeselecteerdMateriaal(List<Materiaal> materialen, int[] aantal, DateTime startDatum, DateTime eindDatum)
        {
            for (int i = 0; i < aantal.Length; i++)
            {
                int aantalBeschikbaar = materialen[i].GeefAantalBeschikbaar(startDatum, eindDatum, this is Student);
                if (aantalBeschikbaar == 0 || aantalBeschikbaar < aantal[i])
                {
                    return false;
                }
            }
            return true;
        }

        public VerlanglijstMaterialenViewModel CreateVerlanglijstMaterialenVm(List<Materiaal> materialen, int[] materiaalIds, int[] aantallen, DateTime startDatum, DateTime eindDatum, bool naarReserveren)
        {
            string datum = DateToString(startDatum, eindDatum, CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat);
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            if (materiaalIds != null)
            {
                materiaalAantal = GetMateriaalAantalMap(materiaalIds, aantallen);
            }
            VerlanglijstMaterialenViewModel vm = CreateVerlangMaterialenViewModel(materialen, datum, startDatum, eindDatum, materiaalAantal, naarReserveren);
            return vm;
        }

        public Dictionary<int, int> GetMateriaalAantalMap(int[] materiaalIds, int[] aantallen)
        {
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            for (int i = 0; i < materiaalIds.Length; i++)
            {
                materiaalAantal.Add(materiaalIds[i], aantallen[i]);
            }
            return materiaalAantal;
        }

        public int GetAantalGeselecteerdeMaterialen(Dictionary<int, int> materiaalAantal)
        {
            int totaalGeselecteerd = 0;
            foreach (var e in materiaalAantal)
            {
                totaalGeselecteerd += materiaalAantal[e.Key];
            }
            return totaalGeselecteerd;
        }
        public VerlanglijstMaterialenViewModel CreateVerlangMaterialenViewModel(List<Materiaal> materialen, string datum, DateTime startDatum, DateTime eindDatum, Dictionary<int, int> materiaalAantal, bool naarReserveren)
        {
            int aantalBeschikbaar, aantalGeselecteerd = 0;
            return new VerlanglijstMaterialenViewModel
            {
                VerlanglijstViewModels = (naarReserveren ? materialen : Verlanglijst.Materialen).Select(m => new VerlanglijstViewModel
                {
                    AantalBeschikbaar = aantalBeschikbaar = m.GeefAantalBeschikbaar(startDatum, eindDatum, this is Lector),
                    AantalGeblokkeerd = m.GeefAantalPerStatus(new Geblokkeerd(), startDatum, eindDatum),
                    Beschikbaar = aantalBeschikbaar == 0,
                    Firma = m.Firma,
                    Prijs = m.Prijs,
                    Foto = m.Foto,
                    AantalGeselecteerd = aantalGeselecteerd = materiaalAantal.ContainsKey(m.MateriaalId) ? aantalBeschikbaar == 0 ? 0 : materiaalAantal[m.MateriaalId] : (aantalGeselecteerd == 0 ? 0 : aantalGeselecteerd > aantalBeschikbaar ? aantalBeschikbaar : aantalGeselecteerd),
                    Geselecteerd = aantalBeschikbaar > 0 ? materialen.Any(k => k.MateriaalId.Equals(m.MateriaalId)) : false,
                    Leergebieden = m.Leergebieden as List<Leergebied>,
                    Doelgroepen = m.Doelgroepen as List<Doelgroep>,
                    ArtikelNr = m.ArtikelNr,
                    AantalInCatalogus = m.AantalInCatalogus,
                    MateriaalId = m.MateriaalId,
                    Beschikbaarheid = aantalBeschikbaar == 0 ?
                                     string.Format("Niet meer beschikbaar van {0} tot {1}", Convert.ToDateTime(startDatum).ToString("d"), Convert.ToDateTime(eindDatum).ToString("d")) :
                                     aantalBeschikbaar < aantalGeselecteerd ? string.Format("Slechts {0} stuks beschikbaar", aantalBeschikbaar) : "",
                    Naam = m.Naam,
                    Omschrijving = m.Omschrijving,
                }),
                GeselecteerdeWeek = datum,
                StartDatum = startDatum.ToString("d"),
                EindDatum = eindDatum.ToString("d"),
                TotaalGeselecteerd = GetAantalGeselecteerdeMaterialen(materiaalAantal),
                Gebruiker = this
            };

        }
        public abstract DateTime GetStartDatum(string startDatum, string eindDatum);
        public abstract DateTime GetEindDatum(string startDatum, string eindDatum);
        public abstract string DateToString(DateTime startDatum, DateTime eindDatum, DateTimeFormatInfo format);

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