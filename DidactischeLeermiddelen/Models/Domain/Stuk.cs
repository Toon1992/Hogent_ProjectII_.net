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
    }
}