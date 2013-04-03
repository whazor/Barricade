
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

		public virtual bool IsBeschermd
		{
			get;
			set;
		}

		public virtual Barricade Barricade
		{
			get;
			set;
		}

	    public override bool MagBarricade
	    {
            get { return !IsBeschermd && Barricade == null && Pionnen.Count == 0; }
	    }

	    public override bool Plaats(Pion pion)
        {
            CheckBarricade();

            //staat al iets op, sla deze pion
            if (Pionnen.Any())
            {
                foreach (var vorig in Pionnen.ToArray())
                {
                    //TODO: fixen!!!! bos wordt niet gevonden, spel bestaat niet in speler
                    // misschien moet dit static worden??
                    var nieuwVeld = IsDorp ? (IVeld) pion.Speler.Spel.Bos : vorig.Speler.Startveld;
                    vorig.Verplaats(nieuwVeld);
                }
                 
                
                Pionnen.Clear();
            }

            Pionnen.Add(pion);
            
            return true;
        }

        [System.Diagnostics.DebuggerHidden()]
	    private void CheckBarricade()
	    {
	        if (Barricade != null)
	        {
	            throw new BarricadeVerplaatsException(this, Barricade);
	        }
	    }

	    public bool Plaats(Barricade bar)
	    {
            if (!MagBarricade) return false;
	        Barricade = bar;
	        return true;
	    }
	}
}

