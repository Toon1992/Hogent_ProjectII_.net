using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace DidactischeLeermiddelen.Models.Domain
{
    public abstract class Gebruiker
    {
        public string Email { get; set; }
        public string Naam { get; set; }
        public virtual Verlanglijst Verlanglijst { get; set; }
        public virtual IList<Reservatie> Reservaties { get; set; }
        public abstract void VoegMateriaalAanVerlanglijstToe(Materiaal materiaal);
        public abstract void VerwijderMateriaalUitVerlanglijst(Materiaal materiaal);
        public abstract void VoegReservatieToe(Materiaal materiaal, DateTime startDatum);
    }
}