
using Barricade.Logic.Exceptions;

namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Finishveld : VeldBase
	{
        public override bool MagBarricade { get { return false; } }

        public override bool Plaats(Pion pion)
        {
            Pionnen.Add(pion);
            pion.IVeld = this;
            throw new GewonnenException(pion.Speler);
        }

	}
}

