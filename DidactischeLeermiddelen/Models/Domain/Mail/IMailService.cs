using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public interface IMailService
    {

        void VerzendMail(IDictionary<Materiaal,int> reservaties, string startDatum,string eindDatum,Gebruiker gebruiker );
    }
}
