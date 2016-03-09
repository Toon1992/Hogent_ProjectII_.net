using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using DidactischeLeermiddelen.Models.Domain.Mail;

namespace DidactischeLeermiddelen.Models.DAL.Mapper
{
    public class MailServiceMapper:EntityTypeConfiguration<MailService>
    {
        public MailServiceMapper()
        {
            Map<MailNaReservatie>(m => m.Requires("onderwerp").HasValue("mailNaReservatie"));

        }
    }
}