
namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public abstract class VeldBase : IVeld
	{
	    public bool IsDorp { get; set; }
	    public bool IsPlaatsbaar { get; set; }
	    public List<IVeld> Buren { get; private set; }
	    public List<Pion> Pionnen { get; protected set; }

        public VeldBase()
        {
            Buren = new List<IVeld>();
            Pionnen = new List<Pion>();
        }

        protected bool VerwijderPion(Pion pion)
        {
            return pion.IVeld.Pionnen.Remove(pion);
        }

        public bool PlaatsPion(Pion pion)
        {
            //meerdere pionnen op dit veld toegestaan
            VerwijderPion(pion);
            Pionnen.Add(pion);
            return true;
        }
	}
}

