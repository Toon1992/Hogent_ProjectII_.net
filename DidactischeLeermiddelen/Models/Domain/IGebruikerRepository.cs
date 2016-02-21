using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IGebruikerRepository
    {
        IGebruiker FindByName(string email);
        void SaveChanges();
        void AddGebruiker(IGebruiker gebruiker);
    }
}