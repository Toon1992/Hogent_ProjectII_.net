using System;
using System.Collections.Generic;
using System.Linq;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class VerlanglijstMaterialenViewModelFactory : ViewModelFactory
    {
        public override IViewModel CreateVerlanglijstMaterialenViewModel(IList<Materiaal> materialen, List<Materiaal> verlanglijstMaterialen, string datum, DateTime startDatum, DateTime eindDatum, Dictionary<int, int> materiaalAantal, bool naarReserveren, IList<DateTime> dagen ,Gebruiker gebruiker)
        {
            int aantalBeschikbaar, aantalGeselecteerd = 0;
            return new VerlanglijstMaterialenViewModel
            {
                VerlanglijstViewModels = (naarReserveren ? materialen : verlanglijstMaterialen).Select(m => new VerlanglijstViewModel
                {
                    AantalBeschikbaar = aantalBeschikbaar = m.GeefAantalBeschikbaar(startDatum, eindDatum, dagen, gebruiker),
                    AantalGeblokkeerd = m.GeefAantalPerStatus(new Geblokkeerd(), startDatum, eindDatum),
                    Beschikbaar = aantalBeschikbaar == 0,
                    Firma = m.Firma,
                    Prijs = m.Prijs,
                    Foto = m.Foto,
                    AantalGeselecteerd = aantalGeselecteerd = m.GeefAantalGeselecteerd(materiaalAantal, aantalBeschikbaar, aantalGeselecteerd),
                    Geselecteerd = aantalBeschikbaar > 0 && materialen.Any(k => k.MateriaalId.Equals(m.MateriaalId)),
                    Leergebieden = m.Leergebieden as List<Leergebied>,
                    Doelgroepen = m.Doelgroepen as List<Doelgroep>,
                    ArtikelNr = m.ArtikelNr,
                    AantalInCatalogus = m.AantalInCatalogus,
                    MateriaalId = m.MateriaalId,
                    Beschikbaarheid = aantalBeschikbaar == 0 ? gebruiker.GeefBeschikbaarheid(startDatum, eindDatum, dagen, m): "",
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