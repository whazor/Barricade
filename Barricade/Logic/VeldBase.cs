
namespace Logic
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
	    public List<Pion> Pionen { get; private set; }

        public VeldBase()
        {
            Buren = new List<IVeld>();
            Pionen = new List<Pion>();
        }
	}
}

