using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DidactischeLeermiddelen.Models.Domain
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string cookieName = FormsAuthentication.FormsCookieName;
            //if (!httpContext.User.Identity.IsAuthenticated ||
            //    httpContext.Request.Cookies == null ||
            //    httpContext.Request.Cookies[cookieName] == null)
            //{
            //    return false;
            //}

            var authCookie = httpContext.Request.Cookies[cookieName];
            if (httpContext.Request.IsAuthenticated)
            {
                return true;
            }
            if (authCookie == null)
            {
                return false;
            }
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            // This is where you can read the userData part of the authentication
            // cookie and fetch the token
            string webServiceToken = authTicket.UserData;

            IPrincipal userPrincipal = new CustomPrincipal(authTicket.Name, webServiceToken);
            //...create some custom implementation and store the web service token as property

            // Inject the custom principal in the HttpContext
            httpContext.User = userPrincipal;
            return base.AuthorizeCore(httpContext);
        }
    }
}