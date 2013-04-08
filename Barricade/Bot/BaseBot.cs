using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barricade.Logic;
#pragma warning disable 1998
namespace Barricade.Bot
{
    /// <summary>
    /// Abstracte bot, deze bevat alle benodige methodes en sorteert velden op score.
    /// </summary>
    abstract class BaseBot : Process.ISpeler
    {
        protected readonly Speler Speler;
        protected readonly Spel Spel;

        public int Gedobbeld { get; set; }
        public Speler AanDeBeurt { get; set; }

        protected BaseBot(Speler speler, Spel spel)
        {
            Speler = speler;
            Spel = spel;
        }

        public async Task<IVeld> VerplaatsBarricade(Func<IVeld, bool> magBarricade)
        {
            return ZoekBarricadePlaats(magBarricade);
        }

        public async Task<Pion> KiesPion(ICollection<Pion> pionnen)
        {
            return pionnen.ToList()
                .OrderBy(pion =>
                    {
                        var tmp = pion
                        .MogelijkeZetten(Gedobbeld)
                        .OrderBy(veld => ZoekVeld(pion, veld))
                        .First();
                        return ZoekVeld(pion, tmp);
                    })
                .First();
        }

        public async Task<IVeld> VerplaatsPion(Pion gekozen, ICollection<IVeld> mogelijk)
        {
            return mogelijk.ToList().OrderBy(veld => ZoekVeld(gekozen, veld)).First();
        }

        abstract protected IVeld ZoekBarricadePlaats(Func<IVeld, bool> magBarricade);
        abstract protected int ZoekVeld(Pion gekozen, IVeld veld);
        public async Task<int> DobbelTask()
        {
            return -1;
        }
    }
}
#pragma warning restore 1998