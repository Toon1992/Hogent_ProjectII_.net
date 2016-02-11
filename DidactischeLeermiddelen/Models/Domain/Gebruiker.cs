using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Gebruiker
    {
        public string Naam { get; set; }

        public bool IsLector { get; set; }

        public int GebruikersId { get; set; }

        public string Email { get; set; }

        public List<Reservatie> Reservaties { get; set; }

        public Verlanglijst Verlanglijst { get; set; }

        public void VoegMateriaalAanVerlanglijstToe()
        {
            throw new System.NotImplementedException();
        }

        public void VoegReservatieToe()
        {
            throw new System.NotImplementedException();
        }
    }
}