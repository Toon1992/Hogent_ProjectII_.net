using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;
using Microsoft.AspNet.Identity;

namespace DidactischeLeermiddelen.Infrastructure
{
    public class GebruikerModelBinder : IModelBinder
    {
        private const string VerlanglijstSessionKey = "gebruiker";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                IGebruikerRepository repos = (IGebruikerRepository)DependencyResolver.Current.GetService(typeof(IGebruikerRepository));
                Gebruiker gebruiker = repos.FindByName(controllerContext.HttpContext.User.Identity.Name);
                if (gebruiker == null)
                {
                    if (controllerContext.HttpContext.User.Identity.Name.Contains("@student.hogent"))
                    {
                        gebruiker = new Student()
                        {
                            Naam = "Student",
                            Email = controllerContext.HttpContext.User.Identity.Name
                        };
                    }
                    else
                    {
                        gebruiker = new Lector
                        {
                            Naam = "Lector",
                            Email = controllerContext.HttpContext.User.Identity.Name,
                        };
                    }
                    gebruiker.Verlanglijst = new Verlanglijst();
                    gebruiker.Reservaties = new List<Reservatie>();
                    repos.AddGebruiker(gebruiker);
                    repos.SaveChanges();
                }
            
                controllerContext.HttpContext.Session[VerlanglijstSessionKey] = gebruiker;
                // Op basis van controllerContext.HttpContext.User.Identity.Name kunnen we niet weten of de gebruiker
                // al dan niet een lector is... Hier moet nog een oplossing voor gezocht worden.
                return gebruiker;
            }
            return null;
        }
    }
}