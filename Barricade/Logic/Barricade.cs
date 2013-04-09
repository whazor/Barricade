using Barricade.Logic.Velden;

namespace Barricade.Logic
{
    public class Barricade
	{
        public delegate void PositieWijzigingEvent(Barricade barricade, Veld nieuwVeld);
        public event PositieWijzigingEvent PositieWijziging;

        #region properties
        public virtual Veld Veld
		{
			get;
			set;
		}
        #endregion

        #region methods
        /// <summary>
        /// Verplaatst het barricade
        /// </summary>
        /// <param name="bestemming">bestemming</param>
        /// <returns>ja of ne</returns>
        public virtual bool Verplaats(Veld bestemming)
		{
            if (!bestemming.Plaats(this)) return false;
            if (Veld != null) Veld.Barricade = null;
		    Veld = bestemming;
		    if (PositieWijziging != null) PositieWijziging(this, bestemming);
		    return true;
        }
        #endregion

    }
}

