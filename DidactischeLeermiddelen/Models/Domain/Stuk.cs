using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Stuk
    {
        public int StukId { get; set; }
        public virtual List<StatusData> StatusData { get; set; }
        public Status HuidigeStatus { get; set; }

        public void VoegNieuweStatusDataToe(int week, Status status)
        {
            StatusData temp = new StatusData() { Status = status, Week = week };
        }

        public void WordtGereserveerd()
        {
            HuidigeStatus = Status.Gereserveerd;
        }

        public void WordtBeschikbaar()
        {
            HuidigeStatus =Status.Beschikbaar;
        }
    }
}