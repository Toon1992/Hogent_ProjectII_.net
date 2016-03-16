namespace DidactischeLeermiddelen.Models.Domain
{
    public class ReservatieStudent : Reservatie
    {
        public ReservatieStudent() { }

        public ReservatieStudent(Gebruiker gebruiker, Materiaal materiaal, string startDatum, int aantal):base(gebruiker,materiaal,startDatum,aantal)
        { }
    }
}