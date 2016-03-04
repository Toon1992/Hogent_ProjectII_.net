using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class LoginHomeService:ILogin
    {
        public async Task<bool> Login(LoginViewModel model)
        {
            return false;
        }
    }
}