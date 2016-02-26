using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class ReservatieData
    {
        public long ReservatieDataId { get; set; }
        public Status Status { get; set; }
    }
}