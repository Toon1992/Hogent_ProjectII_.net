using System;

namespace DidactischeLeermiddelen.Models.Domain.DtoObjects
{
    public class ReservatieDataDTO
    {
        public int Aantal { get; set; }
        public DateTime StartDatum { get; set; }

        public int MateriaalId { get; set; }
    }
}