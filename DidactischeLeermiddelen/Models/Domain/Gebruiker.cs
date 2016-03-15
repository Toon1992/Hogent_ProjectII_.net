using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DidactischeLeermiddelen.ViewModels;

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

        protected abstract void VoegReservatieToe(Materiaal materiaal, int aantal, string startdatum,string[] dagen =null);
       
        public bool ControleGeselecteerdMateriaal(IList<Materiaal> materialen, int[] aantal, DateTime startDatum, DateTime eindDatum, IList<DateTime> dagen)
        {
            for (int i = 0; i < aantal.Length; i++)
            {
                int aantalBeschikbaar = materialen[i].GeefAantalBeschikbaar(startDatum, eindDatum, dagen.ToList(), this);
                if (aantalBeschikbaar == 0 || aantalBeschikbaar < aantal[i])
                {
                    return false;
                }
            }
            return true;
        }
        public string GetDateToString(DateTime startDatum, IEnumerable<DateTime> dagen, DateTimeFormatInfo dtfi)
        {
            string datum;
            if (dagen != null)
            {
                datum = HulpMethode.DatesToString(dagen);
            }
            else
            {
                datum = HulpMethode.DateToString(startDatum);
            }
            return datum;
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

        public abstract string GeefBeschikbaarheid(DateTime startDatum, DateTime eindDaum, IList<DateTime> dagen, Materiaal materiaal);
       

        protected abstract Reservatie MaakReservatieObject(Gebruiker gebruiker, Materiaal mat, string startdatum,int aantal,string[] dagen =null);

    }
}