
using Barricade.Logic.Exceptions;

namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Finishveld : VeldBase
	{

        public override bool Plaats(Pion pion)
        {
            VerwijderPion(pion);
            throw new GewonnenException();
            // TODO: player informatie toevoegen
        }

	}
}

