using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.DAL;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class ViewModelFactory
    {
        public static IViewModel CreateViewModel(String type, IEnumerable<Materiaal> lijst = null,
            IDoelgroepRepository doelgroepRepository =null, ILeergebiedRepository leergebiedRepository=null, Gebruiker gebruiker = null)
        {
            switch (type)
            {
                case "MaterialenViewModel":
                    IViewModel vm = new MaterialenViewModel()
                    {
                        Materialen = lijst.Select(b => new MateriaalViewModel(b)),
                    };
                    return vm;
                case "VerlanglijstMaterialenViewModel":
                    IViewModel vmm = new VerlanglijstMaterialenViewModel()
                    {
                        Materialen = gebruiker.Verlanglijst.Materialen.Select(b => new VerlanglijstViewModel(b))
                    };
                    return vmm;
            }
            return null;
        }
    }
}