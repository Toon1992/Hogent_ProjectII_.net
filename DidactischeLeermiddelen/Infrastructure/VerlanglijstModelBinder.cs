using System.Web.Mvc;
using DidactischeLeermiddelen.Models.Domain;

namespace DidactischeLeermiddelen.Infrastructure
{
    public class VerlanglijstModelBinder : IModelBinder
    {
        private const string verlanglijstSessionKey = "verlanglijst";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            Verlanglijst verlanglijst = controllerContext.HttpContext.Session[verlanglijstSessionKey] as Verlanglijst;
            if (verlanglijst == null)
            {
                verlanglijst = new Verlanglijst();
                controllerContext.HttpContext.Session[verlanglijstSessionKey] = verlanglijst;
            }
            return verlanglijst;
        }
    }
}