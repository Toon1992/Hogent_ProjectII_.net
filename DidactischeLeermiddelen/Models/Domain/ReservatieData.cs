using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class ReservatieData
    {
        public long ReservatieDataId { get; set; }
        public int Week { get; set; }
        public int Aantal { get; set; }
    }
}