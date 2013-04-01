namespace Barricade.Logic
{
    public class Barricade
	{
        public delegate void PositieWijzigingEvent(Barricade barricade, Veld nieuwVeld);
        public event PositieWijzigingEvent PositieWijziging;

		public virtual Veld Veld
		{
			get;
			set;
		}

		public virtual bool Verplaats(Veld bestemming)
		{
            if (!bestemming.Plaats(this)) return false;
            if (Veld != null) Veld.Barricade = null;
		    Veld = bestemming;
		    if (PositieWijziging != null) PositieWijziging(this, bestemming);
		    return true;
		}

	}
}

