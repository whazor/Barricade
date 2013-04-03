using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Barricade.Logic;

namespace Barricade.Bot
{
    static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }


    class Rusher : Process.ISpeler
    {
        private readonly Speler _speler;
        private readonly Spel _spel;

        public Rusher(Speler speler, Spel spel)
        {
            _speler = speler;
            _spel = spel;
        }


//        public Task<Pion> KiesPion(Func<Pion, bool> mogelijk)
//        {
//            throw new NotImplementedException();
//        }
//
        public async Task<IVeld> KiesVeld(Func<IVeld, bool> mogelijk)
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
                        foreach (var buur in velden.SelectMany(veld => veld.Buren.OrderBy(a => a.Score).Where(mogelijk)))
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

        //readonly Random random = new Random();
        private int VanafVeld(Pion pion, IVeld veld)
        {
            if (veld is Finishveld) return int.MinValue;
            return veld.Score - pion.IVeld.Score - veld.Pionnen.Count == 1 ? 4 : 0;
            //return random.Next();
        }

        private Pion _pion;
#pragma warning disable 1998
        public async Task<Pion> KiesPion(ICollection<Pion> pionnen)
#pragma warning restore 1998
        {
            _pion = pionnen.ToList()
                .OrderBy(pion =>
                    {
                        var tmp = pion
                        .MogelijkeZetten(Gedobbeld)
                        .OrderBy(veld => VanafVeld(pion, veld))// veld.Score /*- pion.IVeld.Score*/ - veld.Pionnen.Count == 1 ? 2 : 0)
                        .First();
                        return VanafVeld(pion, tmp); // tmp.Score /*- pion.IVeld.Score*/ - tmp.Pionnen.Count == 1 ? 2 : 0;
                    })
                .First();
            return _pion;
        }

#pragma warning disable 1998
        public async Task<IVeld> KiesVeld(ICollection<IVeld> velden)
#pragma warning restore 1998
        {
            return velden.ToList().OrderBy(veld => VanafVeld(_pion, veld)).First();
        }

        public int Gedobbeld { get; set; }
        public Speler AanDeBeurt { get; set; }
    }
}
