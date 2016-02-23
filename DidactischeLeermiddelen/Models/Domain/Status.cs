using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace DidactischeLeermiddelen.Models.Domain
{
    public enum Status
    {
        Catalogus,
        TeLaat,
        Geblokkeerd,
        Gereserveerd,
        Beschikbaar,
        Onbeschikbaar
    }
}