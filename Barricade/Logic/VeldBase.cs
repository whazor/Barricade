
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
	    public abstract bool MagBarricade { get; }

	    protected VeldBase()
        {
            Buren = new List<IVeld>();
            Pionnen = new List<Pion>();
        }

	    public abstract bool Plaats(Pion pion);

	    public int Score { get; set; }
	}
}

