namespace DidactischeLeermiddelen.Models.Domain
{
    public class BlokkeringLector : Reservatie
    {
        public BlokkeringLector() { }

        public BlokkeringLector(Gebruiker gebruiker, Materiaal materiaal, string startDatum, int aantal):base(gebruiker,materiaal,startDatum,aantal)
        { }
    }
}