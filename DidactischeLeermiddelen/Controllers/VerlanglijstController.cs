using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.StateMachine;
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
            DateTime startDatum = new DateTime();
            if (gebruiker.Verlanglijst.Materialen.Count == 0)
                return View("LegeVerlanglijst");

            VerlanglijstMaterialenViewModelFactory vvmf = new VerlanglijstMaterialenViewModelFactory();
            VerlanglijstMaterialenViewModel vm = vvmf.CreateViewModel(null, null, null, DateTime.Now, gebruiker) as VerlanglijstMaterialenViewModel;
            if ((int)DateTime.Now.DayOfWeek == 6 || (int)DateTime.Now.DayOfWeek == 0 || ((int)DateTime.Now.DayOfWeek == 5 && DateTime.Now.Hour >= 17))
            {
                startDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, (HulpMethode.GetIso8601WeekOfYear(DateTime.Now) + 2) % 53);               
            }
            else
            {
                startDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, (HulpMethode.GetIso8601WeekOfYear(DateTime.Now) + 1) % 53);
            }
            DateTime eindDatum = new DateTime();
            if (gebruiker is Lector)
            {
                startDatum = DateTime.Now;
                eindDatum = startDatum.AddDays(1);
            }
            string startD = startDatum.ToString("d", dtfi);
            string eindD = eindDatum.ToString("d", dtfi);
            return Controle(gebruiker, null, null, false, startD, eindD);
        }

        [HttpPost]
        public ActionResult VerwijderUitVerlanglijst(int id, Gebruiker gebruiker)
        {
            Materiaal materiaal = materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id);

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

        public ActionResult Controle(Gebruiker gebruiker, int[] materiaal, int[] aantal, bool knop, string startDatum, string eindDatum)
        {
            //Variabelen
            VerlanglijstMaterialenViewModel vm = null;
            List<Materiaal> materiaalVerlanglijst = gebruiker.Verlanglijst.Materialen;
            List<Materiaal> materialen = new List<Materiaal>();
            Dictionary<int, int> materiaalAantal = new Dictionary<int, int>();
            DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
            DateTime startDate = new DateTime(), eindDate = new DateTime();
            string datum = "";
            int aantalBeschikbaar, aantalGeselecteerd = 0;
            int totaalGeselecteerd = 0;

            var dateFromString = Convert.ToDateTime(startDatum);
            var ww = HulpMethode.GetIso8601WeekOfYear(dateFromString);
            if (gebruiker is Student)
            {
                startDate = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, ww);
                eindDate = startDate.AddDays(4);
                datum = startDate.ToString("d", dtfi);
            }
            if (gebruiker is Lector)
            {
                startDate = Convert.ToDateTime(startDatum);
                eindDate = Convert.ToDateTime(eindDatum);
                datum = startDate.ToString("d", dtfi) + " - " + eindDate.ToString("d", dtfi);
            }


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
            bool allesBeschikbaar = ControleSelecteerdMateriaal(gebruiker, materiaal, aantal, startDate, eindDate);

            if (knop && allesBeschikbaar)
            {
                vm = new VerlanglijstMaterialenViewModel
                {
                    Materialen = materialen.Select(m => new VerlanglijstViewModel
                    {
                        AantalGeselecteerd = materiaalAantal[m.MateriaalId],
                        Naam = m.Naam,
                    }),
                    GeselecteerdeWeek = datum,
                    StartDatum = startDatum,
                    EindDatum = eindDatum,
                    Gebruiker = gebruiker,
                    TotaalGeselecteerd = totaalGeselecteerd
                };
                if (Request.IsAjaxRequest())
                {
                    return PartialView("Confirmatie", vm);
                }
                return View("Index", vm);
            }
            if (gebruiker is Lector)
            {
                vm = new VerlanglijstMaterialenViewModel
                {
                    Materialen = materiaalVerlanglijst.Select(m => new VerlanglijstViewModel
                    {
                        AantalBeschikbaar = aantalBeschikbaar = m.GeefAantalBeschikbaar(Convert.ToDateTime(startDatum), Convert.ToDateTime(eindDatum), true),
                        AantalGeblokkeerd = m.GeefAantalPerStatus(new Geblokkeerd(), startDate, eindDate),
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
                                        string.Format("Niet meer beschikbaar van {0} tot {1}", Convert.ToDateTime(startDatum).ToString("d"), Convert.ToDateTime(eindDatum).ToString("d")) :
                                        aantalBeschikbaar < aantalGeselecteerd ? string.Format("Slechts {0} stuks beschikbaar", aantalBeschikbaar) : "",
                        Naam = m.Naam,
                        Omschrijving = m.Omschrijving,
                    }),
                    GeselecteerdeWeek = datum,
                    Gebruiker = gebruiker
                };
            }
            else if (gebruiker is Student)
            {
                vm = new VerlanglijstMaterialenViewModel
                {
                    Materialen = materiaalVerlanglijst.Select(m => new VerlanglijstViewModel
                    {
                        AantalBeschikbaar = aantalBeschikbaar = m.GeefAantalBeschikbaar(startDate, eindDate, false),
                        Beschikbaar = aantalBeschikbaar == 0,
                        AantalGeblokkeerd = m.GeefAantalPerStatus(new Geblokkeerd(), startDate, eindDate),
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
                                            string.Format("Niet meer beschikbaar van {0} tot {1}", startDate.ToString("d"), eindDate.ToString("d")) :
                                            aantalBeschikbaar < aantalGeselecteerd ? string.Format("Slechts {0} stuks beschikbaar", aantalBeschikbaar) : "",
                        Naam = m.Naam,
                        Omschrijving = m.Omschrijving,
                    }),
                    GeselecteerdeWeek = datum,
                    Gebruiker = gebruiker
                };
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Verlanglijst", vm);
            }
            return View("Index", vm);
        }
        //private bool ControleSelecteerdMateriaal(Gebruiker gebruiker, int[] materiaal, int[] aantal, int week, string startDatum, string eindDatum)
        private bool ControleSelecteerdMateriaal(Gebruiker gebruiker, int[] materiaal, int[] aantal, DateTime startDatum, DateTime eindDatum)
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
                    aantalBeschikbaar = materialen[i].GeefAantalBeschikbaar(startDatum, eindDatum, gebruiker is Lector);

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
            Dictionary<DateTime, ICollection<ReservatieDetailViewModel>> reservaties = new Dictionary<DateTime, ICollection<ReservatieDetailViewModel>>();
            foreach (Reservatie reservatie in week != -1 ? materiaal.Reservaties.Where(r => HulpMethode.GetIso8601WeekOfYear(r.StartDatum).Equals(week) || HulpMethode.GetIso8601WeekOfYear(r.EindDatum).Equals(week)).OrderByDescending(r => r.Gebruiker.GetType().Name).ThenBy(r => r.StartDatum) : materiaal.Reservaties.OrderByDescending(r => r.Gebruiker.GetType().Name).ThenBy(r => r.StartDatum))
            {
                if (reservatie.Gebruiker is Lector)
                {
                    bool overschrijft = false;
                    //Kijken of de blokkeren van de lector een reservatie van de student overschrijft.
                    //Zoja, dan wordt deze in dezelfde week als die van de student geplaatst.
                    foreach (var e in reservaties)
                    {
                        DateTime startStudent = Convert.ToDateTime(e.Key);
                        DateTime eindStudent = startStudent.AddDays(4);
                        overschrijft = reservatie.OverschrijftMetReservatie(startStudent, eindStudent);
                        if (overschrijft)
                        {
                            reservaties[e.Key].Add(new ReservatieDetailViewModel
                            {
                                Aantal = reservatie.Aantal,
                                Email = reservatie.Gebruiker.Email,
                                Type = "Lector",
                                Status = reservatie.ReservatieState.GetType().Name.ToLower(),
                                GeblokkeerdTot = reservatie.EindDatum.ToString("d")
                            });
                            break;
                        }
                    }
                    //Indien de blokkering van de lector niemand overschrijft wordt de blokkering in de map gestoken met een nieuwe key
                    if (!overschrijft)
                    {
                        int ww = HulpMethode.GetIso8601WeekOfYear(reservatie.StartDatum);
                        DateTime date = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, ww);
                        if (reservaties.ContainsKey(date))
                        {
                            reservaties[date].Add(new ReservatieDetailViewModel
                            {
                                Aantal = reservatie.Aantal,
                                Email = reservatie.Gebruiker.Email,
                                Type = "Lector",
                                Status = reservatie.ReservatieState.GetType().Name.ToLower(),
                                GeblokkeerdTot = reservatie.EindDatum.ToString("d")
                            });
                        }
                        else
                        {
                            reservaties.Add(date, CreateReservatieDetail(reservatie));
                        }
                    }
                }
                else if (reservatie.Gebruiker is Student)
                {
                    if (!reservaties.ContainsKey(reservatie.StartDatum))
                    {
                        reservaties.Add(reservatie.StartDatum, CreateReservatieDetail(reservatie));
                    }
                    else
                    {
                        reservaties[reservatie.StartDatum].Add(new ReservatieDetailViewModel { Aantal = reservatie.Aantal, Email = reservatie.Gebruiker.Email, Type = reservatie.Gebruiker is Student ? "Student" : "Lector", Status = reservatie.ReservatieState.GetType().Name.ToLower(), GeblokkeerdTot = reservatie.Gebruiker is Lector ? reservatie.EindDatum.ToString("d") : "" });
                    }
                }
            }
            return PartialView("DetailReservaties", new ReservatiesDetailViewModel { Reservaties = reservaties, Material = materiaal, GeselecteerdeWeek = week != -1 ? HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d", dtfi) : "" });
        }

        public List<ReservatieDetailViewModel> CreateReservatieDetail(Reservatie reservatie)
        {
            return new List<ReservatieDetailViewModel>
            {
                new ReservatieDetailViewModel
                {
                    Aantal = reservatie.Aantal,
                    Email = reservatie.Gebruiker.Email,
                    Status = reservatie.ReservatieState.GetType().Name.ToLower(),
                    Type = reservatie.Gebruiker is Student ? "Student" : "Lector",
                    GeblokkeerdTot = reservatie.Gebruiker is Lector ? reservatie.EindDatum.ToString("d") : ""
                }
            };
        }

        public JsonResult ReservatieDetailsGrafiek(int id, int week)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            var reservaties = materiaal.Reservaties.OrderByDescending(r => r.Gebruiker.GetType().Name).ThenBy(r => r.StartDatum);
            List<Object> reservatieList = new List<object>();
            DateTime datumDateTime= new DateTime();
            DateTime datumMaandVooruit = new DateTime();
            Dictionary<DateTime, bool> checkReservaties = new Dictionary<DateTime, bool>();
            if (week == -1)
            {
                datumDateTime = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, HulpMethode.GetIso8601WeekOfYear(DateTime.Now));
                datumMaandVooruit = datumDateTime.AddDays(28);
            }
            else
            {

                datumDateTime = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week);
                datumMaandVooruit = datumDateTime.AddDays(28);
            }

            foreach (var r in reservaties)
            {
                if (r.StartDatum >= datumDateTime && r.StartDatum<= datumMaandVooruit)
                {
                    var reservatieData = new
                    {
                        Aantal = materiaal.AantalInCatalogus - r.Aantal,
                        StartDatum = r.StartDatum
                    };
                    reservatieList.Add(reservatieData);
                    checkReservaties.Add(r.StartDatum, true);
                }
                
            }

            while (datumDateTime <= datumMaandVooruit)
            {
                if (!checkReservaties.ContainsKey(datumDateTime))
                {
                    reservatieList.Add(new
                    {
                        Aantal = materiaal.AantalInCatalogus,
                        StartDatum = datumDateTime
                    });
                }

                datumDateTime = datumDateTime.AddDays(7);

            }


            JavaScriptSerializer jss = new JavaScriptSerializer();
            string output = jss.Serialize(reservatieList);


            return Json(output, JsonRequestBehavior.AllowGet);
        }


    }
}