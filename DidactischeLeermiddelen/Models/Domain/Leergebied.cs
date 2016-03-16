namespace DidactischeLeermiddelen.Models.Domain
{
    public class Leergebied
    {
        #region fields
        public int LeergebiedId { get; set; }
        public string Naam { get; set; }
        public virtual List<Materiaal> Materialen { get; set; }
        #endregion
    }
}