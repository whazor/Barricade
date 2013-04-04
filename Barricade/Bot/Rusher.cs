using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Barricade.Logic;
using Barricade.Utilities;

namespace Barricade.Bot
{
// zet foutmelding over async uit.
#pragma warning disable 1998
    class Rusher : Process.ISpeler
    {
        private readonly Speler _speler;
        private readonly Spel _spel;

        public Rusher(Speler speler, Spel spel)
        {
            _speler = speler;
            _spel = spel;
        }

        public int Gedobbeld { get; set; }
        public Speler AanDeBeurt { get; set; }


        public async Task<IVeld> VerplaatsBarricade(Func<IVeld, bool> magBarricade)
        {
            var concurrentie = _spel.Spelers.Where(speler => speler != _speler).OrderBy(speler => speler.Pionnen.OrderBy(pion => pion.IVeld.Score).First().IVeld.Score).ToList();
            concurrentie.Shuffle();

            var following = 3;
            while (true)
            {
                foreach (var velden in from speler in concurrentie from poin in speler.Pionnen select new List<IVeld>{poin.IVeld})
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

        private static int VanafVeld(Pion pion, IVeld veld)
        {
            if (veld is Finishveld) return int.MinValue;
            return veld.Score - pion.IVeld.Score - veld.Pionnen.Count == 1 ? 4 : 0;
        }

        public async Task<Pion> KiesPion(ICollection<Pion> pionnen)
        {
            return pionnen.ToList()
                .OrderBy(pion =>
                    {
                        var tmp = pion
                        .MogelijkeZetten(Gedobbeld)
                        .OrderBy(veld => VanafVeld(pion, veld))
                        .First();
                        return VanafVeld(pion, tmp);
                    })
                .First();
        }

        public async Task<IVeld> VerplaatsPion(Pion gekozen, ICollection<IVeld> mogelijk)
        {
            return mogelijk.ToList().OrderBy(veld => VanafVeld(gekozen, veld)).First();
        }

        public async Task DobbelTask()
        {
        }
    }
#pragma warning restore 1998
}