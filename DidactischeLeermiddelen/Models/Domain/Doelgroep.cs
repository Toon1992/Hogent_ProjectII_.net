namespace DidactischeLeermiddelen.Models.Domain
{
    public class Doelgroep
    {
        #region fields
        public int DoelgroepId { get; set; }
        public string Naam { get; set; }
        public virtual List<Materiaal> Materialen { get; set; }
        #endregion
    }
}