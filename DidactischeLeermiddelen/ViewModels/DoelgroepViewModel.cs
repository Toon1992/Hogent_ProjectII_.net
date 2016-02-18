using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DidactischeLeermiddelen.ViewModels
{
    public class DoelgroepViewModel : IViewModel
    {
        // This property contains the available options
        public SelectList DoelgroepenLijst { get; set; }

        // This property contains the selected options
        //public IEnumerable<string> SelectedSources { get; set; }

        public DoelgroepViewModel(SelectList doelgroepen)
        {
            DoelgroepenLijst = doelgroepen;

            //SelectedSources = new List<String>();
        }
    }

    public class LeergebiedViewModel : IViewModel
    {
        public SelectList LeergebiedenSelectList { get; set; }

        public LeergebiedViewModel(SelectList leergebieden)
        {
            LeergebiedenSelectList = leergebieden;
        }
    }
}