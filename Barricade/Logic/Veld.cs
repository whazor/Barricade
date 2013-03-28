
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

        public new bool PlaatsPion(Pion pion)
        {
             VerwijderPion(pion);

            //staat al iets op, sla deze pion
            if (Pionnen.Any())
            {
                IVeld nieuwVeld = (this.IsDorp) ? (IVeld) pion.Speler.Spel.Bos : (IVeld) Pionnen.First().Speler.Startveld;
                Pionnen.First().IVeld = nieuwVeld;
                nieuwVeld.Pionnen.Add(Pionnen.First());
                Pionnen = new List<Pion>();
            }

            Pionnen.Add(pion);
            return true;
        }

	}
}

