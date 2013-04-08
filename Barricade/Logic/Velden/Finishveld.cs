
using Barricade.Logic.Exceptions;

namespace Barricade.Logic.Velden
{
    public class Finishveld : VeldBase
	{
        public override bool MagBarricade { get { return false; } }
        public override bool MagPionErlangs { get { return true; } }

	    public override bool Plaats(Pion pion)
        {
            Pionnen.Add(pion);
            throw new GewonnenException(pion.Speler);
        }
	}
}

