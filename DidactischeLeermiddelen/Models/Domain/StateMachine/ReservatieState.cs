namespace DidactischeLeermiddelen.Models.Domain.StateMachine
{
    public abstract class ReservatieState
    {
        public virtual Reservatie Reservatie { get; set;}
        protected ReservatieState(Reservatie reservatie)
        {
            Reservatie = reservatie;
        }
        public ReservatieState() { }
        
        public virtual void Blokkeer() {}
        public virtual void Overruul() {}

    }
}