﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface ILeergebiedRepository
    {
        IQueryable<Leergebied> FindByLeergebiedList(Leergebied leergebied);
        void SaveChanges();
    }
}