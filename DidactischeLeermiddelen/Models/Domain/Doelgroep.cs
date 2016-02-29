using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Doelgroep
    {
        #region fields
        public int DoelgroepId { get; set; }
        public string Naam { get; set; }
        #endregion
    }
}