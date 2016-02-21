using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IGebruiker
    {
        string Email { get; set; }
        string Naam { get; set; }
        Verlanglijst Verlanglijst { get; set; }
        IList<Reservatie> Reservaties { get; set; }
        void VoegMateriaalAanVerlanglijstToe(Materiaal materiaal);
        void VerwijderMateriaalUitVerlanglijst(Materiaal materiaal);
        void VoegReservatieToe(Materiaal materiaal, DateTime startDatum);
    }
}