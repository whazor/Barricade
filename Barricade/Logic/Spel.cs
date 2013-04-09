using Barricade.Logic.Velden;
using Barricade.Utilities;

namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Spel
	{
        public List<Speler> Spelers
        {
            get; private set;
        }
	    public List<Finishveld> Finishvelden
	    {
	        get; private set;
	    }

        public Bos Bos { get; set; }

        public CountedRandom Random { get; private set; }

	    public Spel(int seed, int counter)
        {
            Spelers = new List<Speler>();
            Finishvelden = new List<Finishveld>();
            Random = new CountedRandom(seed, counter);
        }

        public void BerekenScores()
        {
            foreach (IVeld vanaf in Finishvelden)
            {
                // tijdelijk alles op -1
                vanaf.Score = -1;
                GaVeldAf(vanaf, 1);   
            }
            // terug naar 0
            foreach (IVeld vanaf in Finishvelden)
            {
                vanaf.Score = 0;
            }

        }

        private static void GaVeldAf(IVeld vanaf, int score)
        {
            foreach (var veld in vanaf.Buren.Where(veld => veld.Score == 0 || veld.Score > score))
            {
                veld.Score = score;
                GaVeldAf(veld, score + 1);
            }
        }
	}
}

