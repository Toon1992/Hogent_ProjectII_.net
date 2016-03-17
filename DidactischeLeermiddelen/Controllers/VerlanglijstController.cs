using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.DtoObjects;
using DidactischeLeermiddelen.ViewModels;
using WebGrease.Css.Extensions;

namespace DidactischeLeermiddelen.Controllers
{
    [Authorize]
    public class VerlanglijstController : Controller
    {
        private IMateriaalRepository materiaalRepository;
        private IGebruikerRepository gebruikerRepository;
        private DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat;
        private ViewModelFactory factory;
        public VerlanglijstController(IMateriaalRepository materiaalRepository, IGebruikerRepository gebruikerRepository)
        {
            this.materiaalRepository = materiaalRepository;
            this.gebruikerRepository = gebruikerRepository;
        }

        // GET: Verlanglijst
        public ActionResult Index(Gebruiker gebruiker)
        {
            DateTime startDatum;
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
            }
            string startD = startDatum.ToString("d", dtfi);
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
            IList<Materiaal> materialen = new List<Materiaal>();
            Dictionary<int, int> materiaalMap = new Dictionary<int, int>();
            factory = new VerlanglijstMaterialenViewModelFactory();

            DateTime startDate = HulpMethode.GetStartDatum(startDatum);
            if (startDate < DateTime.Now)
            {
                startDate = DateTime.Today;
            }
            DateTime eindDate = HulpMethode.GetEindDatum(startDatum);

            //Indien er materialen geselecteerd zijn wordt er gekeken of er voor dat materiaal voldoende beschikbaar zijn
            //voor de gekozen periode.
            IList<DateTime> dagLijst = dagen?.Select(Convert.ToDateTime).ToList();
            var dateString = dagLijst != null? HulpMethode.DatesToString(dagLijst): HulpMethode.DateToString(startDate);
            if (materiaal != null)
            {
                materialen = GeefMaterialenVanId(materiaal);
                materiaalMap = gebruiker.GetMateriaalAantalMap(materiaal, aantal);
                allesBeschikbaar = ControleSelecteerdMateriaal(gebruiker, materiaal, aantal, startDate, eindDate, dagLijst);               
            }
            
            var vm = factory.CreateVerlanglijstMaterialenViewModel(materialen, gebruiker.Verlanglijst.Materialen, dateString,
                startDate, eindDate, materiaalMap, allesBeschikbaar && naarReserveren, dagLijst, gebruiker) as VerlanglijstMaterialenViewModel;
            
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
        public bool ControleSelecteerdMateriaal(Gebruiker gebruiker, int[] materiaal, int[] aantal, DateTime startDatum, DateTime eindDatum, IList<DateTime> dagen)
        {
            IList<Materiaal> materialen = GeefMaterialenVanId(materiaal);
            if (dagen != null)
            {
                //Wanneer de lector verschillende data selecteerd kijken of het materiaal elke dag beschikbaar is
                //Zoniet, return false
                return gebruiker.ControleGeselecteerdMateriaal(materialen, aantal,DateTime.Now,DateTime.Now, dagen);
                    //dagen.Select(dag => gebruiker.ControleGeselecteerdMateriaal(materialen, aantal, dag, dag)).All(beschikbaar => beschikbaar);
            }
            return gebruiker.ControleGeselecteerdMateriaal(materialen, aantal, startDatum, eindDatum, dagen);
        }

        public ActionResult ReservatieDetails(Gebruiker gebruiker, int id, int week)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            factory = new ReservatieDetailViewModelFactory();
            var map = materiaal.ReservatieDetails(week);
            //Map converteren
            Dictionary<DateTime, IEnumerable<ReservatieDetailViewModel>> vmMap = 
                map.ToDictionary
                (
                    d => d.Key,
                    d => d.Value.Select(v => factory.CreateReservatieDetailViewModel(v) as ReservatieDetailViewModel)
                );
            return PartialView("DetailReservaties", factory.CreateReservatiesViewModel(vmMap, materiaal, week, dtfi) as ReservatiesDetailViewModel);
        }

        public JsonResult ReservatieDetailsGrafiek(int id, int week)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);

            var startDatumFilter = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week == -1 ? HulpMethode.GetIso8601WeekOfYear(DateTime.Now) : week);
            var datumMaandVooruitFilter = startDatumFilter.AddDays(28);

            Dictionary<DateTime, int> resrevatieMap = materiaal.MaakLijstReservatieDataInRange(startDatumFilter,datumMaandVooruitFilter);
            List<ReservatieDataDTO> reservatieList = CreateReservatieDataDtos(resrevatieMap);

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string output = jss.Serialize(reservatieList);

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReservatieDetailsGrafiekPerDag(int id, string[] dagen)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            DateTime[] dagenDateTimes = new DateTime[dagen.Length];

            for(int i = 0;i<dagen.Length;i++)
            {
                dagenDateTimes[i] = Convert.ToDateTime(dagen[i]);
            }

            Dictionary<DateTime, int> resrevatieMap = materiaal.MaakLijstReservatieDataSpecifiekeDagen(dagenDateTimes);
            List<ReservatieDataDTO> reservatieList = CreateReservatieDataDtos(resrevatieMap);

            JavaScriptSerializer jss = new JavaScriptSerializer();
            string output = jss.Serialize(reservatieList);

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        private List<ReservatieDataDTO> CreateReservatieDataDtos(Dictionary<DateTime, int> reservatieMap)
        {
            List<ReservatieDataDTO> dtoLijst = new List<ReservatieDataDTO>();
            reservatieMap.ForEach(e =>
            {
                dtoLijst.Add(new ReservatieDataDTO
                {
                    Aantal = e.Value,
                    StartDatum = e.Key              
                });
            });
            return dtoLijst;
        }  
        private IList<Materiaal> GeefMaterialenVanId(int[] materiaalIds)
        {
            return materiaalIds.Select(id => materiaalRepository.FindById(id)).ToList();
        }

    }
}