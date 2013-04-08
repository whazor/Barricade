using System.Collections.Generic;

namespace Barricade.Logic.Velden
{
    public interface IVeld 
	{
		List<IVeld> Buren { get; }
		List<Pion> Pionnen { get; }
	    bool Plaats(Pion pion);

        // Dit komt van buitenaf
        bool IsDorp { get; set; }
        bool IsPlaatsbaar { get; set; }
        int Score { get; set; }

        // Dit is afleidbaar
        bool MagBarricade { get; }
	    bool MagPionErlangs { get; }
	    bool MagPion(Pion pion);
	}
}

