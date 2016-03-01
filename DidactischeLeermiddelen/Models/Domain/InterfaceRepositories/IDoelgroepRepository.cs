using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DidactischeLeermiddelen.Models.Domain
{
    public interface IDoelgroepRepository
    {
        IQueryable<Doelgroep> FindAll();
        Doelgroep FindById(int id);
        void SaveChanges();
    }
}