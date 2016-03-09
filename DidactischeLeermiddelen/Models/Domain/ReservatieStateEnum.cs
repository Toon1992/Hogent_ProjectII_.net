using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace DidactischeLeermiddelen
{
    public enum ReservatieStateEnum
    {
        Gereserveerd,
        Geblokkeerd,
        TeLaat,
        Opgehaald,
        Overrulen
    }
}