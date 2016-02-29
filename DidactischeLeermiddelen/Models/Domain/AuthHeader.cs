using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class AuthHeader : SoapHeader
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}