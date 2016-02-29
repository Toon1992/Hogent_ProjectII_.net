using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.ViewModels;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Controllers
{
    [CustomAuthorize]
    public class VerlanglijstController : Controller
    {
        private IMateriaalRepository materiaalRepository;
        private IGebruikerRepository gebruikerRepository;
        private DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
        public VerlanglijstController(IMateriaalRepository materiaalRepository, IGebruikerRepository gebruikerRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.gebruikerRepository = gebruikerRepository;
        }

        // GET: Verlanglijst
        public ActionResult Index(Gebruiker gebruiker)
        {        
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeVerlanglijst");

            VerlanglijstMaterialenViewModelFactory vvmf = new VerlanglijstMaterialenViewModelFactory();
            VerlanglijstMaterialenViewModel vm = vvmf.CreateViewModel(null, null, null, DateTime.Now, gebruiker) as VerlanglijstMaterialenViewModel;
            if ((int)DateTime.Now.DayOfWeek == 6 || (int)DateTime.Now.DayOfWeek == 0 || ((int)DateTime.Now.DayOfWeek == 5 && DateTime.Now.Hour < 17))
            {
                vm.GeselecteerdeWeek = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, (HulpMethode.GetIso8601WeekOfYear(DateTime.Now) + 2) % 53).ToString("d", dtfi);
            }
            else
            {
                vm.GeselecteerdeWeek = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, (HulpMethode.GetIso8601WeekOfYear(DateTime.Now) + 1) % 53).ToString("d", dtfi);
            }
            vm.Gebruiker = gebruiker;
            
            return View(vm);
        }

        [HttpPost]
        public ActionResult VerwijderUitVerlanglijst(int id, Gebruiker gebruiker)
        {
            Materiaal materiaal = materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id);
            List<String> lijst = new List<String>() { "bla", "sdsfsfd", "ésdsdsf" };

            if (materiaal != null)
            {
                try
                {
                    gebruiker.VerwijderMateriaalUitVerlanglijst(materiaal);
                    gebruikerRepository.SaveChanges();
                    TempData["Info"] = $"Item {materiaal.Naam} werd verwijderd uit uw verlanglijst";

                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }               
            }
            return RedirectToAction("Index");
        }

        public ActionResult Controle(Gebruiker gebruiker, int[] materiaal, int[] aantal, bool knop, string startDatum, string eindDatum, int week = 0)
        {
            //Variabelen
            VerlanglijstMaterialenViewModel vm = null;
            List<Materiaal> materiaalVerlanglijst = gebruiker.Verlanglijst.Materialen;
            List<Materiaal> materialen = new List<Materiaal>();
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
            int aantalBeschikbaar, aantalGeselecteerd = 0;
            int totaalGeselecteerd = 0;

            if (materiaal != null)
            {
                materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
                //Map maken die de geselecteerde materialen met hun aantallen verbind.

                for (int i = 0; i < materiaal.Length; i++)
                {
                    materiaalAantal.Add(materiaal[i], aantal[i]);
                    totaalGeselecteerd += aantal[i];
                }
            }

            //Wanneer op "Ga naar reservatie werd geklikt wordt eerst gekeken of de gekozen materialen met voldoende
            //aantal stuks beschikbaar zijn, zoniet wordt het verlanglijstscherm terug getoont.
            bool allesBeschikbaar = ControleSelecteerdMateriaal(gebruiker, materiaal, aantal, week, startDatum, eindDatum);

            if (knop && allesBeschikbaar)
            {
                vm = new VerlanglijstMaterialenViewModel
                {
                    Materialen = materialen.Select(m => new VerlanglijstViewModel
                    {
                        AantalGeselecteerd = materiaalAantal[m.MateriaalId],
                        Naam = m.Naam,                  
                    }),
                    GeselecteerdeWeek = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d", dtfi),
                    StartDatum = startDatum,
                    EindDatum = eindDatum,
                    Gebruiker = gebruiker,
                    TotaalGeselecteerd = totaalGeselecteerd
                };
                return PartialView("Confirmatie", vm);
            }         
            if (gebruiker is Lector)
            {
                vm = new VerlanglijstMaterialenViewModel
                {
                    Materialen = materiaalVerlanglijst.Select(m => new VerlanglijstViewModel
                    {
                        AantalBeschikbaar = aantalBeschikbaar = m.GeefAantalBeschikbaarLector(Convert.ToDateTime(startDatum), Convert.ToDateTime(eindDatum)),
                        Beschikbaar = aantalBeschikbaar == 0,
                        Firma = m.Firma,
                        Prijs = m.Prijs,
                        Foto = m.Foto,
                        AantalGeselecteerd = aantalGeselecteerd = materiaalAantal.ContainsKey(m.MateriaalId) ? aantalBeschikbaar == 0 ? 0 : materiaalAantal[m.MateriaalId] : (aantalGeselecteerd == 0 ? aantalGeselecteerd == aantalBeschikbaar ? 0 : 1 : aantalGeselecteerd > aantalBeschikbaar ? aantalBeschikbaar : aantalGeselecteerd),
                        Geselecteerd = aantalBeschikbaar > 0 ? materialen.Any(k => k.MateriaalId.Equals(m.MateriaalId)) : false,
                        Leergebieden = m.Leergebieden as List<Leergebied>,
                        Doelgroepen = m.Doelgroepen as List<Doelgroep>,
                        ArtikelNr = m.ArtikelNr,
                        AantalInCatalogus = m.AantalInCatalogus,
                        MateriaalId = m.MateriaalId,
                        Beschikbaarheid = aantalBeschikbaar == 0 ?
                                        string.Format("Niet meer beschikbaar van {0} tot {1}", Convert.ToDateTime(startDatum), Convert.ToDateTime(eindDatum)) :
                                        aantalBeschikbaar < aantalGeselecteerd ? string.Format("Slechts {0} stuks beschikbaar", aantalBeschikbaar) : "",
                        Naam = m.Naam,
                        Omschrijving = m.Omschrijving,
                    }),
                    GeselecteerdeWeek = startDatum + "-" + eindDatum,
                    Gebruiker = gebruiker
                };
            }
            else if (gebruiker is Student)
            {          
            vm = new VerlanglijstMaterialenViewModel
            {
                Materialen = materiaalVerlanglijst.Select(m => new VerlanglijstViewModel
                {
                    AantalBeschikbaar = aantalBeschikbaar = m.GeefAantalBeschikbaar(HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week)),
                    Beschikbaar = aantalBeschikbaar == 0,
        
                    Firma = m.Firma,
                        Prijs = m.Prijs,
                    Foto = m.Foto,
                        AantalGeselecteerd = aantalGeselecteerd = materiaalAantal.ContainsKey(m.MateriaalId) ? aantalBeschikbaar == 0 ? 0 : materiaalAantal[m.MateriaalId] : (aantalGeselecteerd == 0 ? aantalGeselecteerd == aantalBeschikbaar ? 0 : 1 : aantalGeselecteerd > aantalBeschikbaar ? aantalBeschikbaar : aantalGeselecteerd),
                    Geselecteerd = aantalBeschikbaar > 0 ? materialen.Any(k => k.MateriaalId.Equals(m.MateriaalId)) : false,
                    Leergebieden = m.Leergebieden as List<Leergebied>,
                    Doelgroepen = m.Doelgroepen as List<Doelgroep>,
                    ArtikelNr = m.ArtikelNr,
                    AantalInCatalogus = m.AantalInCatalogus,
                    MateriaalId = m.MateriaalId,
                    Beschikbaarheid = aantalBeschikbaar == 0 ? 
                                        string.Format("Niet meer beschikbaar van {0} tot {1}", HulpMethode.FirstDateOfWeekISO8601(2016, week).ToString("d"), HulpMethode.FirstDateOfWeekISO8601(2016, week).AddDays(5).ToString("d")) :
                                        aantalBeschikbaar < aantalGeselecteerd ? string.Format("Slechts {0} stuks beschikbaar", aantalBeschikbaar) : "",
                    Naam = m.Naam,
                    Omschrijving = m.Omschrijving,
                }),
                    GeselecteerdeWeek = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d", dtfi),
                    Gebruiker = gebruiker
            };
            }

            return PartialView("Verlanglijst", vm);
        }

        private bool ControleSelecteerdMateriaal(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week, string startDatum, string eindDatum)
        {
            //Variabelen
            List<Materiaal> materialen = new List<Materiaal>();
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            int aantalBeschikbaar = 0;

            if (materiaal != null)
            {
                materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
                //Map maken die de geselecteerde materialen met hun aantallen verbind.

                for (int i = 0; i < materiaal.Length; i++)
                {
                    materiaalAantal.Add(materiaal[i], aantal[i]);
                }
            }
            //Melding geven indien niet alle gewenste materialen beschikbaar zijn.
            if (materiaal != null)
            {
                for (int i = 0; i < materiaal.Length; i++)
                {
                    if (gebruiker is Student)
                    {
                        aantalBeschikbaar = materialen[i].GeefAantalBeschikbaar(HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week));
                    }
                    else if (gebruiker is Lector)
                    {
                        aantalBeschikbaar = materialen[i].GeefAantalBeschikbaarLector(Convert.ToDateTime(startDatum),
                                                Convert.ToDateTime(eindDatum));
                    }

                        if (aantalBeschikbaar == 0)
                        {
                        ModelState.AddModelError("", "error");
                        }
                        else if (aantalBeschikbaar < aantal[i])
                        {
                        ModelState.AddModelError("", "error");
                        }
                }
            }
            if (ModelState.IsValid)
            {
                return true;
            }
            return false;
        }

        public ActionResult ReservatieDetails(int id, int week)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            Dictionary<string, ICollection<ReservatieDetailViewModel>> reservaties = new Dictionary<string, ICollection<ReservatieDetailViewModel>>();
            foreach (Reservatie reservatie in week != -1 ? materiaal.Reservaties.Where(r => r.StartDatum.Equals(HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week))) : materiaal.Reservaties)
            {
                if (!reservaties.ContainsKey(reservatie.StartDatum.ToString("d", dtfi)))
                {
                    reservaties.Add(reservatie.StartDatum.ToString("d", dtfi), new List<ReservatieDetailViewModel> { new ReservatieDetailViewModel { Aantal = reservatie.Aantal, Naam = reservatie.Gebruiker.Naam, Status = reservatie.ReservatieState.GetType().BaseType.Name, Type = reservatie.Gebruiker is Student ? "Student" : "Lector", AantalDagenGeblokkeerd = reservatie.Gebruiker is Lector ? reservatie.AantalDagenGeblokkeerd : 0 } });
                }
                else
                {
                    var list = reservaties[reservatie.StartDatum.ToString("d", dtfi)];
                    list.Add(new ReservatieDetailViewModel { Aantal = reservatie.Aantal, Naam = reservatie.Gebruiker.Naam, Type = reservatie.Gebruiker is Student ? "Student" : "Lector", Status = reservatie.ReservatieState.GetType().BaseType.Name, AantalDagenGeblokkeerd = reservatie.Gebruiker is Lector ? reservatie.AantalDagenGeblokkeerd : 0 });
                }
            }
            return PartialView("DetailReservaties", new ReservatiesDetailViewModel { Reservaties = reservaties, Material = materiaal, GeselecteerdeWeek = week != -1 ? HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d", dtfi) : "" });
        }

       
    }
}