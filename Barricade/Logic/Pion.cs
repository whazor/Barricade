namespace Barricade.Logic
{
	using Process;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Pion
	{
	    public Pion(Speler speler)
	    {
	        Speler = speler;
	    }

	    public virtual IVeld IVeld
		{
			get;
			set;
		}

		public virtual Speler Speler
		{
			get; private set;
		}

		public virtual List<List<IVeld>> MogelijkeZetten(int stappen = 1)
		{
            return MogelijkeZetten(IVeld, IVeld, stappen).GroupBy(lijst => lijst.First()).Select(a => a.First()).ToList();
		}

        private IEnumerable<List<IVeld>> MogelijkeZetten(IVeld vorige, IVeld begin, int stappen)
        {
            stappen--;
            var lijsten = new List<List<IVeld>>();
            foreach (var veld in begin.Buren.Where(veld => veld != vorige))
            {
                if (stappen >= 1)
                {
                    if (veld is Veld && (veld as Veld).Barricade != null) continue;

                    var nieuw = MogelijkeZetten(begin, veld, stappen);
                    foreach (var lijst in nieuw)
                    {
                        lijst.Add(veld);
//                        lijst.Add(begin);
                        lijsten.Add(lijst);
                    }
                }
                else
                {
                    // mag niet terug naar startveld
                    if (veld is Startveld) continue;
                    // bezette rustvelden worden oververslagen
                    if (veld is Rustveld && veld.Pionnen.Count > 0) continue;
                    // wanneer het veld geen bos is, maar wel een speler opstaat
                    if (!(veld is Bos) && veld.Pionnen.Count > 0 && veld.Pionnen.First().Speler == Speler) continue;

                    lijsten.Add(new List<IVeld> {veld});
                }
            }
            return lijsten.Where(lijst => lijst.Any()).ToList();
        }

	    public virtual bool Verplaats(IVeld bestemming)
		{
			throw new System.NotImplementedException();
		}
	}
}

