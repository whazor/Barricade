using Barricade.Logic;

namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Bos : IVeld
	{
	    public bool IsDorp { get; set; }
	    public bool IsPlaatsbaar { get; set; }
	    public List<IVeld> Buren { get; private set; }
	    public List<Pion> Pionnen { get; set; }

        public Bos()
        {
            Buren = new List<IVeld>();
            Pionnen = new List<Pion>();
        }

        public bool Plaats(Pion pion)
        {
            //verwijder pion van vorige veld
            pion.IVeld.Pionnen.Remove(pion);

            //meerdere pionnen op dit veld toegestaan
            Pionnen.Add(pion);
            return true;
        }
	}


}

