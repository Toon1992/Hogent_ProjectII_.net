﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DidactischeLeermiddelen.Models.DAL;
using DidactischeLeermiddelen.ViewModels;

namespace DidactischeLeermiddelen.Models.Domain
{
    public class ViewModelFactory
    {
        public static IViewModel CreateViewModel(String type, SelectList doelgroepen, SelectList leergebieden, IEnumerable<Materiaal> lijst = null, Gebruiker gebruiker = null)
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