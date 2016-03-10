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
using DidactischeLeermiddelen.Models.Domain.DtoObjects;
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
            DateTime startDatum;
            DateTime eindDatum = new DateTime();
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

        public ActionResult Controle(Gebruiker gebruiker, int[] materiaal, int[] aantal, bool naarReserveren, string startDatum, string eindDatum)
        {
            //Variabelen
            bool allesBeschikbaar = false;
            List<Materiaal> materialen = new List<Materiaal>();

            DateTime startDate = gebruiker.GetStartDatum(startDatum, eindDatum);
            DateTime eindDate = gebruiker.GetEindDatum(startDatum, eindDatum);

            //Indien er materialen geselecteerd zijn wordt er gekeken of er voor dat materiaal voldoende beschikbaar zijn
            //voor de gekozen periode.
            if (materiaal != null)
            {
                materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
                allesBeschikbaar = ControleSelecteerdMateriaal(gebruiker, materiaal, aantal, startDate, eindDate);               
            }
            VerlanglijstMaterialenViewModel vm = gebruiker.CreateVerlanglijstMaterialenVm(materialen, materiaal, aantal, startDate, eindDate, allesBeschikbaar && naarReserveren);
            if (Request.IsAjaxRequest())
            {
                if (naarReserveren && allesBeschikbaar)
                {
                    return PartialView("Confirmatie", vm);
                }
                return PartialView("Verlanglijst", vm);
            }
            return View("Index", vm);
        }
        private bool ControleSelecteerdMateriaal(Gebruiker gebruiker, int[] materiaal, int[] aantal, DateTime startDatum, DateTime eindDatum)
        { 
            List<Materiaal> materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
            return gebruiker.ControleGeselecteerdMateriaal(materialen, aantal, startDatum, eindDatum);
        }

        public ActionResult ReservatieDetails(Gebruiker gebruiker, int id, int week)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            var map = materiaal.ReservatieDetails();
            return PartialView("DetailReservaties", new ReservatiesDetailViewModel { ReservatieMap = map, Material = materiaal, GeselecteerdeWeek = week != -1 ? HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week).ToString("d", dtfi) : "" });
        }

        public JsonResult ReservatieDetailsGrafiek(int id, int week)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            var reservaties = materiaal.Reservaties.OrderByDescending(r => r.Gebruiker.GetType().Name).ThenBy(r => r.StartDatum);
            List<ReservatieDataDTO> reservatieList = new List<ReservatieDataDTO>();
            DateTime datumDateTime= new DateTime();
            DateTime datumMaandVooruit = new DateTime();
            Dictionary<int, bool> checkReservaties = new Dictionary<int, bool>();
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
                    ReservatieDataDTO reservatieData = new ReservatieDataDTO
                    {
                        Aantal = materiaal.AantalInCatalogus - r.Aantal,
                        StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, HulpMethode.GetIso8601WeekOfYear(r.StartDatum))
                    };
                    
                    if (checkReservaties.ContainsKey(HulpMethode.GetIso8601WeekOfYear(r.StartDatum)))
                    {
                        var reservatie =
                            reservatieList.FirstOrDefault(
                                p =>
                                    p.StartDatum.Equals(HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year,
                                        HulpMethode.GetIso8601WeekOfYear(r.StartDatum))));
                        var aantal = reservatie.Aantal - r.Aantal;
                        reservatie.Aantal = aantal < 0 ? 0 : aantal;
                    }
                    else
                    {
                        checkReservaties.Add(HulpMethode.GetIso8601WeekOfYear(r.StartDatum), true);
                        reservatieList.Add(reservatieData);
                    }                    
                }                
            }
            while (datumDateTime <= datumMaandVooruit)
            {
                if (!checkReservaties.ContainsKey(HulpMethode.GetIso8601WeekOfYear(datumDateTime)))
                {
                    reservatieList.Add(new ReservatieDataDTO
                    {
                        Aantal = materiaal.AantalInCatalogus,
                        StartDatum = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, HulpMethode.GetIso8601WeekOfYear(datumDateTime))
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