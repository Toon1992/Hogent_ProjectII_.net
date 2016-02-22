using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class StatusData
    {
        public long StatusDataId { get; set; }
        public int Week { get; set; }
        public Status Status { get; set; }
    }
}