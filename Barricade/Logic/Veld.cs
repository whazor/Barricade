
using Barricade.Logic.Exceptions;

namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Veld : VeldBase
	{
	    public bool StandaardBarricade = false;

		public virtual bool MagBarricade
		{
			get;
			set;
		}

		public virtual Barricade Barricade
		{
			get;
			set;
		}

        public override bool Plaats(Pion pion)
        {
            if (Barricade != null)
            {
                throw new BarricadeVerplaatsException(this, Barricade);
            }

            VerwijderPion(pion);

            //staat al iets op, sla deze pion
            if (Pionnen.Any())
            {
                foreach (var vorig in Pionnen.ToArray())
                {
                    var nieuwVeld = IsDorp ? (IVeld)pion.Speler.Spel.Bos : vorig.Speler.Startveld;
                    vorig.Verplaats(nieuwVeld);
                }

                Pionnen.Clear();
            }

            Pionnen.Add(pion);
            return true;
        }

	    public bool Plaats(Barricade pion)
	    {
	        if (Barricade != null || !MagBarricade) return false;
	        Barricade = pion;
	        return true;
	    }
	}
}

