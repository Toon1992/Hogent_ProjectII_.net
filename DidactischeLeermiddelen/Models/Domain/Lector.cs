using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Lector:Gebruiker
    {
        public override Verlanglijst Verlanglijst { get; set; }
        public override IList<Reservatie> Reservaties { get; set; }
    }
}