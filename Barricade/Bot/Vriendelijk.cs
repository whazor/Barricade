using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barricade.Logic;
using Barricade.Utilities;

namespace Barricade.Bot
{
    class Vriendelijk : BaseBot
    {
        private readonly Random _random;

        public Vriendelijk(Speler speler, Spel spel) : base(speler, spel)
        {
            _random = spel.Random;
        }

        protected override IVeld ZoekBarricadePlaats(Func<IVeld, bool> magBarricade)
        {
            var concurrentie =
                Spel.Spelers
                     .OrderBy(speler => speler.Pionnen.OrderBy(pion => pion.IVeld.Score).First().IVeld.Score)
                     .ToList();
            concurrentie.Shuffle(_random);

            var following = 1;
            while (true)
            {
                foreach (
                    var velden in from speler in concurrentie from poin in speler.Pionnen select new List<IVeld> {poin.IVeld})
                {
                    for (var i = 0; i < following; i++)
                    {
                        foreach (var buur in velden.SelectMany(veld => veld.Buren.OrderBy(a => a.Score).Where(magBarricade)))
                        {
                            return buur;
                        }
                        var kopie = velden.ToList();
                        velden.Clear();

                        velden.AddRange(kopie.SelectMany(veld => veld.Buren));
                    }
                }
                following++;
            }
        }

        protected override int ZoekVeld(Pion pion, IVeld veld)
        {
            if (veld is Finishveld) return int.MinValue;
            var score = veld.Score - pion.IVeld.Score;

            if (veld.Score - pion.IVeld.Score < 0) score += 2;

            if (veld.Pionnen.Count == 1) score += 20;

            return score;
        }
    
    }
}
