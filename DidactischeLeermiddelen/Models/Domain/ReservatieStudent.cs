using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class ReservatieStudent : Reservatie
    {
        public ReservatieStudent() { }

        public ReservatieStudent(Gebruiker gebruiker, Materiaal materiaal, string startDatum, string eindDatum, int aantal):base(gebruiker,materiaal,startDatum,eindDatum,aantal)
        { }
    }
}