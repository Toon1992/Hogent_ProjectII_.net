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
        public abstract void Reserveer();
        public abstract void Blokkeer();
        public abstract void Overruul();

    }
}