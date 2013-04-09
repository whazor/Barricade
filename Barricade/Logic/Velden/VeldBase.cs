
using System.Collections.Generic;
using System.Linq;

namespace Barricade.Logic.Velden
{
    public abstract class VeldBase : IVeld
    {
        #region properties en variablen
        public bool IsDorp { get; set; }
	    public bool IsPlaatsbaar { get; set; }
	    public List<IVeld> Buren { get; private set; }
	    public List<Pion> Pionnen { get; protected set; }
	    public abstract bool MagBarricade { get; }
        public abstract bool MagPionErlangs { get; }
        public int Score { get; set; }
        #endregion

        #region methods
        /// <summary>
        /// Kijkt of er een pion op staat of er een pion geslagen kan worden
        /// </summary>
        /// <param name="pion">Pion</param>
        /// <returns>ja of nee</returns>
        public virtual bool MagPion(Pion pion)
	    {
	        return Pionnen.Count == 0 || !Pionnen.Select(other => other.Speler).Contains(pion.Speler);
	    }

	    protected VeldBase()
        {
            Buren = new List<IVeld>();
            Pionnen = new List<Pion>();
        }

	    public abstract bool Plaats(Pion pion);
        #endregion

    }
}

