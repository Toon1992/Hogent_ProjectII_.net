using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Leergebied
    {
        #region fields
        public int LeergebiedId { get; set; }
        public string Naam { get; set; }
        #endregion
    }
}