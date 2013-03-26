
using Barricade.Logic.Exceptions;

namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Finishveld : VeldBase
	{

        public new bool PlaatsPion(Pion pion)
        {
            VerwijderPion(pion);
            throw new GewonnenException();
        }

	}
}

