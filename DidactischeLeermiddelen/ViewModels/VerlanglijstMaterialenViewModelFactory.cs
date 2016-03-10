using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class VerlanglijstMaterialenViewModelFactory : ViewModelFactory
    {
        public override  IViewModel CreateViewModel(SelectList doelgroepen, SelectList leergebieden, IEnumerable<Materiaal> lijst = null,
            DateTime startDatum = new DateTime(), Gebruiker gebruiker = null)
        {
            IViewModel vmm = new VerlanglijstMaterialenViewModel()
            {
                VerlanglijstViewModels = gebruiker.Verlanglijst.Materialen.Select(b => new VerlanglijstViewModel(b, startDatum))
            };
            return vmm;
        }
        public VerlanglijstMaterialenViewModel CreateVerlangMaterialenViewModel(List<Materiaal> materialen, List<Materiaal> verlanglijstMaterialen, string datum, DateTime startDatum, DateTime eindDatum, Dictionary<int, int> materiaalAantal, bool naarReserveren, Gebruiker gebruiker)
        {
            int aantalBeschikbaar, aantalGeselecteerd = 0;
            return new VerlanglijstMaterialenViewModel
            {
                VerlanglijstViewModels = (naarReserveren ? materialen : verlanglijstMaterialen).Select(m => new VerlanglijstViewModel
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
                TotaalGeselecteerd = gebruiker.GetAantalGeselecteerdeMaterialen(materiaalAantal),
                Gebruiker = gebruiker
            };

        }
    }
}