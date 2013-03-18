
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
	    public IEnumerable<IVeld> Buren { get; private set; }
	    public IEnumerable<Pion> Pion { get; set; }
	}
}

