using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.DtoObjects;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Controllers
{
    [Authorize]
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
            return Controle(gebruiker, null, null, false, startD, null);
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

        public ActionResult Controle(Gebruiker gebruiker, int[] materiaal, int[] aantal, bool naarReserveren, string startDatum, string[] dagen)
        {
            //Variabelen
            bool allesBeschikbaar = false;
            IEnumerable<DateTime> dagLijst = null;
            List<Materiaal> materialen = new List<Materiaal>();
            
            DateTime startDate = gebruiker.GetStartDatum(startDatum);
            DateTime eindDate = gebruiker.GetEindDatum(startDatum);

            //Indien er materialen geselecteerd zijn wordt er gekeken of er voor dat materiaal voldoende beschikbaar zijn
            //voor de gekozen periode.
            if (materiaal != null)
            {
                materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
                dagLijst = dagen?.Select(Convert.ToDateTime);
                allesBeschikbaar = ControleSelecteerdMateriaal(gebruiker, materiaal, aantal, startDate, eindDate, dagLijst);               
            }
            VerlanglijstMaterialenViewModel vm = gebruiker.CreateVerlanglijstMaterialenVm(materialen, materiaal, aantal, startDate, eindDate,dagLijst, allesBeschikbaar && naarReserveren);
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
        private bool ControleSelecteerdMateriaal(Gebruiker gebruiker, int[] materiaal, int[] aantal, DateTime startDatum, DateTime eindDatum, IEnumerable<DateTime> dagen)
        { 
            List<Materiaal> materialen = materiaal.Select(id => materiaalRepository.FindAll().FirstOrDefault(m => m.MateriaalId == id)).ToList();
            if (dagen != null)
            {
                //Wanneer de lector verschillende data selecteerd kijken of het materiaal elke dag beschikbaar is
                //Zoniet, return false
                return dagen.Select(dag => gebruiker.ControleGeselecteerdMateriaal(materialen, aantal, dag, dag)).All(beschikbaar => beschikbaar);
            }
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

            var startDatumFilter = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week == -1 ? HulpMethode.GetIso8601WeekOfYear(DateTime.Now) : week);
            var datumMaandVooruitFilter = startDatumFilter.AddDays(28);

            List<ReservatieDataDTO> reservatieList = materiaal.MaakLijstReservatieDataInRange(startDatumFilter,
                datumMaandVooruitFilter);

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string output = jss.Serialize(reservatieList);

            return Json(output, JsonRequestBehavior.AllowGet);
        }
    }
}