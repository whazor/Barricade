
using System.Linq;

namespace Barricade.Logic.Velden
{
    public class Rustveld : VeldBase
	{
        public override bool MagBarricade { get { return false; } }
        public override bool MagPionErlangs { get { return true; } }

        public override bool MagPion(Pion pion)
        {
            return Pionnen.Count == 0;
        }

        public override bool Plaats(Pion pion)
        {
            //staat al iets op
            if (Pionnen.Any()) return false;

            Pionnen.Add(pion);
            return true;
        }
	}
}

