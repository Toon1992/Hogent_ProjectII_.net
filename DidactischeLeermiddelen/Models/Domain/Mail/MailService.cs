﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidactischeLeermiddelen.Models.Domain.Mail
{
    public abstract class MailService
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public abstract void VerzendMail(IDictionary<Materiaal,int> reservaties, string startDatum,string eindDatum,Gebruiker gebruiker );
    }
}
