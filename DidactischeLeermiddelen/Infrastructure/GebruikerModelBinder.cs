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
                Gebruiker gebruiker = controllerContext.HttpContext.Session[VerlanglijstSessionKey] as Gebruiker;
                IGebruikerRepository repos = (IGebruikerRepository)DependencyResolver.Current.GetService(typeof(IGebruikerRepository));
                if (gebruiker == null)
                {
                    gebruiker = repos.FindByName(controllerContext.HttpContext.User.Identity.Name);
                }
                if (gebruiker == null)
                {
                    gebruiker = new Gebruiker
                    {
                        Naam = controllerContext.HttpContext.User.Identity.Name,
                        Email = controllerContext.HttpContext.User.Identity.Name,
                        IsLector = controllerContext.HttpContext.User.Identity.Name.Contains("@student.hogent")? false : true,
                        GebruikersId = controllerContext.HttpContext.User.Identity.GetUserId(),
                    };
                    gebruiker.Verlanglijst.VerlanglijstId = gebruiker.GebruikersId;
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