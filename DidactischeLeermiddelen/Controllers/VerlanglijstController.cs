﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
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
        private DateTimeFormatInfo dtfi = CultureInfo.CreateSpecificCulture("be-BE").DateTimeFormat;
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

            if (IsWeekend())
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
            var dateString = dagLijst != null ? HulpMethode.DatesToString(dagLijst) : HulpMethode.DateToString(startDate);
            if (materiaal != null)
            {
                materialen = GeefMaterialenVanId(materiaal);
                materiaalMap = gebruiker.GetMateriaalAantalMap(materiaal, aantal);
                allesBeschikbaar = ControleGeselecteerdMateriaal(gebruiker, materialen, aantal, startDate, eindDate, dagLijst);
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
        public bool ControleGeselecteerdMateriaal(Gebruiker gebruiker, IList<Materiaal> materialen, int[] aantal, DateTime startDatum, DateTime eindDatum, IList<DateTime> dagen)
        {
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

        public JsonResult ReservatieDetailsGrafiek(int id, int week, bool perDag)
        {
            Materiaal materiaal = materiaalRepository.FindById(id);
            DateTime[] dagenVanDeWeek = null;
            if (perDag)
            {
                dagenVanDeWeek = new DateTime[5];
                DateTime maandag = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week);
                dagenVanDeWeek[0] = maandag;
                for (int i = 1; i < dagenVanDeWeek.Length; i++)
                {
                    dagenVanDeWeek[i] = maandag.AddDays(i);
                }
            }
            var startDatumFilter = HulpMethode.FirstDateOfWeekISO8601(DateTime.Now.Year, week == -1 ? HulpMethode.GetIso8601WeekOfYear(DateTime.Now) : week);
            var datumMaandVooruitFilter = startDatumFilter.AddDays(28);
            List<ReservatieDataDTO> reservatieList = MaakLijstReservatieDataInRange(materiaal, startDatumFilter,datumMaandVooruitFilter, dagenVanDeWeek);
            return Json(SerializeObject(reservatieList), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReservatieDetailsGrafiekPerDag(Gebruiker gebruiker, int[] ids, string[] dagen)
        {
            if (ids != null && gebruiker is Lector)
            {
                IList<Materiaal> materialen = ids.Select(i => materiaalRepository.FindById(i)).ToList();
                DateTime[] dagenDateTimes = new DateTime[dagen.Length];
                IList<List<ReservatieDataDTO>> lijstReservatieData = new List<List<ReservatieDataDTO>>();

                for (int i = 0; i < dagen.Length; i++)
                {
                    dagenDateTimes[i] = Convert.ToDateTime(dagen[i]);
                }
                foreach (Materiaal m in materialen)
                {                
                    List<ReservatieDataDTO> reservatieList = MaakLijstReservatieDataInRange(m, DateTime.Now,
                        DateTime.Now, dagenDateTimes);
                    lijstReservatieData.Add(reservatieList);
                }
                return Json(SerializeObject(lijstReservatieData), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        private List<ReservatieDataDTO> MaakLijstReservatieDataInRange(Materiaal materiaal, DateTime startDatum, DateTime datumMaandVooruit, DateTime[] dagenVanWeek)
        {
            Dictionary<DateTime, int[]> reservatieMap;
            if (dagenVanWeek != null)
            {
                reservatieMap = materiaal.MaakLijstReservatieDataSpecifiekeDagen(dagenVanWeek);
            }
            else
            {
                reservatieMap = materiaal.MaakLijstReservatieDataInRange(startDatum, datumMaandVooruit);
            }
            return CreateReservatieDataDtos(reservatieMap);
        }
        private string SerializeObject<T>(T obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }
        private List<ReservatieDataDTO> CreateReservatieDataDtos(Dictionary<DateTime, int[]> reservatieMap)
        {
            List<ReservatieDataDTO> dtoLijst = new List<ReservatieDataDTO>();
            reservatieMap.OrderBy(k => k.Key).ForEach(e =>
            {
                dtoLijst.Add(new ReservatieDataDTO
                {
                    Aantal = e.Value[0],
                    StartDatum = e.Key,
                    MateriaalId = e.Value[1]
                });
            });
            return dtoLijst;

        }

        //private List<ReservatieDataDTO> CreateReservatieDataDtosPerDag(Dictionary<DateTime, int> reservatieMap)
        //{
        //    List<ReservatieDataDTO> dtoLijst = new List<ReservatieDataDTO>();
        //    reservatieMap.ForEach(e =>
        //    {
        //        dtoLijst.Add(new ReservatieDataDTO
        //        {
        //            Aantal = e.Value,
        //            StartDatum = e.Key
        //        });
        //    });
        //    return dtoLijst;
        //}
        private IList<Materiaal> GeefMaterialenVanId(int[] materiaalIds)
        {
            return materiaalIds.Select(id => materiaalRepository.FindById(id)).ToList();
        }

        private bool IsWeekend()
        {
            return (int)DateTime.Now.DayOfWeek == 6 || (int)DateTime.Now.DayOfWeek == 0 ||
                   ((int)DateTime.Now.DayOfWeek == 5 && DateTime.Now.Hour >= 17);
        }

    }
}