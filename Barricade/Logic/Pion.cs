namespace Barricade.Logic
{
	using Process;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Pion
	{
        public delegate void PositieWijzigingEvent(Pion pion, IVeld nieuwVeld);
        public event PositieWijzigingEvent PositieWijziging;
	    public Dictionary<IVeld, List<IVeld>> Paden { get; private set; }
	    private int aantalStappen;

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

		public virtual List<IVeld> MogelijkeZetten(int stappen = 1)
		{
            // Kijk welke zetten er mogelijk zijn
		    var mogelijk = MogelijkeZetten(IVeld, IVeld, stappen);
            // Groepeer per bestemming
		    var perBestemming = mogelijk.GroupBy(lijst => lijst.First());
            // Selecteer een willekeurig pad
		    var willkeurigUniek =
		        perBestemming.Select(a => a.Count() == 1 ? a.First() : a.Skip(new Random().Next(0, a.Count())).Take(1).First()).ToList();
            // Sla alle paden tijdelijk op
		    Paden = willkeurigUniek.ToDictionary(lijst => lijst.First(), lijst => lijst);
            // Geef alle mogelijkheden terug
            return willkeurigUniek.Select(lijst => lijst.First()).ToList();
		}

        private IEnumerable<List<IVeld>> MogelijkeZetten(IVeld vorige, IVeld begin, int stappen)
        {
            stappen--;
            var lijsten = new List<List<IVeld>>();
            // Ga alle buren af
            foreach (var veld in begin.Buren.Where(veld => veld != vorige))
            {
                // Wanneer de pion meer dan 1 stappen mag zetten moeten de buren bezocht worden.
                if (stappen >= 1)
                {
                    // Kijk of er een barricade opstaat
                    if (veld is Veld && (veld as Veld).Barricade != null) continue;

                    var nieuw = MogelijkeZetten(begin, veld, stappen);
                    foreach (var lijst in nieuw)
                    {
                        lijst.Add(veld);
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
            // Filter de lege lijsten eruit
            return lijsten.Where(lijst => lijst.Any()).ToList();
        }

	    public virtual bool Verplaats(IVeld bestemming)
	    {
	        var vorig = IVeld;
            if (bestemming.Plaats(this))
            {
                vorig.Pionnen.Remove(this);
                IVeld = bestemming;

                if (PositieWijziging != null) PositieWijziging(this, bestemming);
                Paden = null;
                return true;
            }

	        return false;
	    }
	}
}

