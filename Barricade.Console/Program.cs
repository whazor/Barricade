using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Barricade.Bot;
using Barricade.Data;
using Barricade.Logic;
using Barricade.Logic.Velden;
using Barricade.Process;
using Spel = Barricade.Process.Spel;

namespace Barricade.Shell
{
    class Program : IView
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(100, 50);
            new Program();
        }

        public Program()
        {
            Logo();
            var result = KiesLevel();
            Console.Clear();
            var drawer = new Drawer(result);

            var thread = new Thread(drawer.Start);
            thread.Start();
            while (!thread.IsAlive) { }

            var spelers = new Dictionary<Speler, ISpeler>();

            foreach (var speler in result.Spel.Spelers)
            {
                spelers[speler] = new Rusher(speler, result.Spel);
            }

            var spel = new Spel(result.Spel, this, spelers);
            spel.Start();
            Console.ReadLine();
        }

        private static Loader KiesLevel()
        {
            Console.WriteLine(@"Welk level wilt u spelen?");
            Console.WriteLine("");
            var i = 1;
            var mapping = new Dictionary<string, string>();
            foreach (var game in Levels.Lijst())
            {
                Console.WriteLine(@"- {0} (toets {1})", game, i);
                mapping[i + ""] = game;
                i++;
            }
            var keuze = "?";
            while (!mapping.ContainsKey(keuze))
            {
                keuze = Console.ReadKey().KeyChar + "";
            }
            return new Loader(Levels.Open(mapping[keuze]));
        }

        private static void Logo()
        {
            Console.WriteLine("");
            Console.WriteLine(@"  .---.           .--.        .--.                             .     ");
            Console.WriteLine(@"      |           |   :       |   )              o             |     ");
            Console.WriteLine(@"      |.  . .-.   |   | .-.   |--:  .-.  .--..--..  .-..-.  .-.| .-. ");
            Console.WriteLine(@"      ;|  |(.-'   |   ;(.-'   |   )(   ) |   |   | (  (   )(   |(.-' ");
            Console.WriteLine(@"  `--' `--`-`--'  '--'  `--'  '--'  `-'`-'   ' -' `-`-'`-'`-`-'`-`--'");
            Console.WriteLine("");
        }

        public async Task Wacht(int wachttijdBot)
        {
            Thread.Sleep(1000);
        }

//        public Task<IVeld> VerplaatsBarricade(Logic.Barricade barricade, Func<IVeld, bool> magBarricade)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<Pion> KiesPion(ICollection<Pion> pionnen, int gedobbeld)
//        {
//            int pion = -1;
//            while (pion < 0 || pion >= pionnen.Count)
//            {
//                int.TryParse(Console.ReadKey().KeyChar + "", out pion);
//            }
//            int i = 0;
//            foreach (var current in pionnen)
//            {
//                if (i == pion)
//                    return current;
//                i++;
//            }
//            Console.WriteLine();
//            return null;
//        }
//
//        public Task<IVeld> VerplaatsPion(Pion gekozen, ICollection<IVeld> mogelijk)
//        {
//            throw new NotImplementedException();
//        }

        public int Gedobbeld { get; set; }
        public Speler IsAanBeurt { get; set; }
        public void Gewonnen(Speler speler)
        {
            Console.WriteLine(@"Speler " + speler.Name + @" heeft gewonnen!");
        }

        public Speler AanDeBeurt { get; set; }
        public async Task<int> DobbelTask()
        {
            return 0;
        }
    }
}
