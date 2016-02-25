using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class CustomPrincipal : IPrincipal
    {
        
        public IIdentity Identity { get; private set; }
        public string WebserviceToken { get; set; }
        public CustomPrincipal(string email, string webServiceToke)
        {
            Identity = new GenericIdentity(email);
            WebserviceToken = webServiceToke;
        }

        public CustomPrincipal(string email)
        {
            Identity = new GenericIdentity(email);
        }
        public bool IsInRole(string role)
        {
            return role.Equals("student") || role.Equals("lector");
        }
    }
}