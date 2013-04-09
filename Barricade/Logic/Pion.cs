using Barricade.Logic.Velden;

namespace Barricade.Logic
{
	using Process;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Pion
    {
        #region events
        public delegate void PositieWijzigingEvent(Pion pion, IVeld nieuwVeld);
        public event PositieWijzigingEvent PositieWijziging;
        #endregion

        #region properties
        public Dictionary<IVeld, List<IVeld>> Paden { get; private set; }
        public virtual IVeld IVeld
        {
            get;
            set;
        }

        public virtual Speler Speler
        {
            get;
            private set;
        }
        #endregion

        #region constructor
        public Pion(Speler speler)
	    {
	        Speler = speler;
	    }
        #endregion

        #region methods
        /// <summary>
        /// Geeft mogelijke zetten terug als list
        /// </summary>
        /// <param name="stappen">aantal stappen</param>
        /// <returns>list met velden</returns>
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

        /// <summary>
        /// Recursieve functie die de mogelijke zeten berkend.
        /// </summary>
        /// <param name="vorige">het vorige veld</param>
        /// <param name="begin">het begin veld</param>
        /// <param name="stappen">aantal stappen</param>
        /// <returns>geeft een lijst met velden terug</returns>
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
                    if(!veld.MagPionErlangs) continue;

                    // Zoek verdere zetten op
                    var nieuw = MogelijkeZetten(begin, veld, stappen);
                    foreach (var lijst in nieuw)
                    {
                        lijst.Add(veld);
                        lijsten.Add(lijst);
                    }
                }
                else
                {
                    if (veld.MagPion(this))
                    {
                        lijsten.Add(new List<IVeld> { veld });
                    }
                }
            }
            // Filter de lege lijsten eruit
            return lijsten.Where(lijst => lijst.Any()).ToList();
        }

        /// <summary>
        /// Pakt de pion op
        /// </summary>
        public virtual void Oppakken()
        {
            if (IVeld == null) return;
            IVeld.Pionnen.Remove(this);
            IVeld = null;
        }

        /// <summary>
        /// Verplaatst pion op bestemming
        /// </summary>
        /// <param name="bestemming">bestemming als in een iveld</param>
        /// <return>ja of nee</returns>
	    public virtual bool Verplaats(IVeld bestemming)
	    {
            Oppakken();
            if (bestemming.Plaats(this))
            {
                IVeld = bestemming;
                if (PositieWijziging != null) 
                    PositieWijziging(this, bestemming);

                Paden = null;
                return true;
            }
	        return false;
        }
        #endregion
    }
}

