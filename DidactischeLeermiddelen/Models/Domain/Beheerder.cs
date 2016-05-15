using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class Beheerder
    {
        public int GebruikersId { get; set; }
        public string Email { get; set; }
        public bool IsHoofd { get; set; }
        public Beheerder(string email, bool admin)
        {
            Email = email;
            IsHoofd = admin;
        }
        
    }
}