using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.ViewModels
{
    public abstract class ViewModelFactory
    {
        public virtual IViewModel CreateMateriaalViewModel(Materiaal materiaal)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateFirmaViewModel(Materiaal materiaal)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateMaterialenViewModel(SelectList doelgroepen, SelectList leergebieden,
            IEnumerable<Materiaal> lijst)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateVerlanglijstMaterialenViewModel(IList<Materiaal> materialen, List<Materiaal> verlanglijstMaterialen, string datum, DateTime startDatum, DateTime eindDatum, Dictionary<int, int> materiaalAantal, bool naarReserveren, IList<DateTime> dagen, Gebruiker gebruiker)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateReservatieMaterialenViewModel(Gebruiker gebruiker)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateReservatieDetailViewModel(Reservatie reservatie)
        {
            throw new NotImplementedException();
        }

        public virtual IViewModel CreateReservatiesViewModel(
            Dictionary<DateTime, IEnumerable<ReservatieDetailViewModel>> map, Materiaal materiaal, int week,
            DateTimeFormatInfo dtfi)
        {
            throw new NotImplementedException();
        }
    }

   
}