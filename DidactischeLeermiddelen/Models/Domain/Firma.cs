using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Firma
    {

        public string Naam { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
        public string Contactpersoon { get; set; }
        public string Website { get; set; }

        public Firma(string naam,string email,string website,string adres="Niet gekend",string contactpersoon="Niet gekend")
        {
            Naam = naam;
            Email = email;
            Website = website;
            Adres = adres;
            Contactpersoon = contactpersoon;

        }

        public Firma()
        {
            
        }
    }
}