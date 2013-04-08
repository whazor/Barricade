using System.Collections.Generic;

namespace Barricade.Logic.Velden
{
    public class Bos : IVeld
	{
	    public bool IsDorp { get; set; }
	    public bool IsPlaatsbaar { get; set; }
	    public List<IVeld> Buren { get; private set; }
	    public List<Pion> Pionnen { get; set; }
	    public bool MagBarricade { get { return false; } }
        public bool MagPionErlangs { get { return true; } }
	    public bool MagPion(Pion pion)
	    {
	        return true;
	    }

	    public Bos()
        {
            Buren = new List<IVeld>();
            Pionnen = new List<Pion>();
        }
	    public bool Plaats(Pion pion)
        {
            //meerdere pionnen op dit veld toegestaan
            Pionnen.Add(pion);
            return true;
        }

	    public int Score { get; set; }
	}


}

