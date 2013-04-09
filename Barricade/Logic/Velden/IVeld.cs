using System.Collections.Generic;

namespace Barricade.Logic.Velden
{
    public interface IVeld
    {
        #region variablen en properties
        List<IVeld> Buren { get; }
		List<Pion> Pionnen { get; }

        // Dit komt van buitenaf
        bool IsDorp { get; set; }
        bool IsPlaatsbaar { get; set; }
        int Score { get; set; }

        // Dit is afleidbaar
        bool MagBarricade { get; }
        bool MagPionErlangs { get; }
        #endregion

        #region methodes
        bool Plaats(Pion pion);
	    bool MagPion(Pion pion);
        #endregion
    }
}

