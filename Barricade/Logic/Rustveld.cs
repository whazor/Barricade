
namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Rustveld : VeldBase
	{

        public new bool PlaatsPion(Pion pion)
        {
            VerwijderPion(pion);

            //staat al iets op
            if (Pionnen.Any()) return false;

            Pionnen.Add(pion);
            return true;
        }

	}
}

