using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Infrastructure
{
    public class GebruikerModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                Gebruiker gebruiker = new Gebruiker
                {
                    Naam = controllerContext.HttpContext.User.Identity.Name,
                    Email = controllerContext.HttpContext.User.Identity.Name
                };
                // Op basis van controllerContext.HttpContext.User.Identity.Name kunnen we niet weten of de gebruiker
                // al dan niet een lector is... Hier moet nog een oplossing voor gezocht worden.
                return gebruiker; 
            }
            return null;
        }
    }
}