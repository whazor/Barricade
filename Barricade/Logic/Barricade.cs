namespace Barricade.Logic
{
    public class Barricade
	{
        public delegate void PositieWijzigingEvent(Veld nieuwVeld);
        public event PositieWijzigingEvent PositieWijziging;

		public virtual Veld Veld
		{
			get;
			set;
		}

		public virtual void Verplaats(Veld bestemming)
		{
            Veld = bestemming;
		    Veld.Plaats(this);
            if (PositieWijziging != null) PositieWijziging(bestemming);
		}

	}
}

