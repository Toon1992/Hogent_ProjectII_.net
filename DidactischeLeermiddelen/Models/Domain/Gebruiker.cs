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

        protected abstract void VoegReservatieToe(Materiaal materiaal, int aantal, string startdatum, string eindDatum);
       
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

        public VerlanglijstMaterialenViewModel CreateVerlanglijstMaterialenVm(List<Materiaal> materialen, int[] materiaalIds, int[] aantallen, DateTime startDatum, DateTime eindDatum,IEnumerable<DateTime> dagen, bool naarReserveren)
        {
            string datum;
            if (dagen != null)
            {
                datum = DatesToString(dagen, CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat);
            }
            else
            {
                datum = DateToString(startDatum, CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat);
            }
            
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            if (materiaalIds != null)
            {
                materiaalAantal = GetMateriaalAantalMap(materiaalIds, aantallen);
            }
            VerlanglijstMaterialenViewModelFactory facotry = new VerlanglijstMaterialenViewModelFactory();
            return facotry.CreateVerlangMaterialenViewModel(materialen,Verlanglijst.Materialen, datum, startDatum, eindDatum, materiaalAantal, naarReserveren, this);
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
       

        public string DateToString(DateTime startDatum, DateTimeFormatInfo format)
        {
            return startDatum.ToString("d", format);
        }

        public string DatesToString(IEnumerable<DateTime> dagen, DateTimeFormatInfo format)
        {
            return string.Join(",", dagen.Select(d => d.ToString("d", format)).ToArray());
        }

        protected abstract Reservatie MaakReservatieObject(Gebruiker gebruiker, Materiaal mat, string startdatum,
            string eindDatum,
            int aantal);

    }
}