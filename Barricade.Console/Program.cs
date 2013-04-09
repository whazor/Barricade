using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Barricade.Data;
using Barricade.Logic;
using Barricade.Logic.Velden;
using Barricade.Process;
using Spel = Barricade.Process.Spel;

namespace Barricade.Shell
{
    class Program : IView, ISpeler
    {
        static void Main(string[] args)
        {
            var loader = new Loader(new[]
                {
                    "                        < >                    ",
                    "                         |                     ",
                    "D       ( )-( )-( )-( )-[*]-( )-( )-( )-( )    ",
                    "         |                               |     ",
                    "D       ( )-( )-( )-{ }-[*]-{ }-( )-( )-( )    ",
                    "                         |                     ",
                    "D           ( )-( )-[*]-( )-[*]-( )-( )        ",
                    "             |                       |         ",
                    "            { }-[*]-( )-( )-( )-[*]-{ }        ",
                    "                         |                     ",
                    "                ( )-( )-{ }-( )-( )            ",
                    "                 |       |       |             ",
                    "                 |      <1>      |             ",
                    "                 |               |             ",
                    "    { }-( )-{ }-( )-( )-{ }-( )-( )-{ }-( )-{ }",
                    "     |       |           |           |       | ",
                    "-   ( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )",
                    "         |       |               |       |     ",
                    "        <2>     <3>             <4>     <5>    ",
                    "*1:BOS,",
                    "*2:START,R4",
                    "*3:START,G4",
                    "*4:START,Y4",
                    "*5:START,B4"
                }
            );

            Console.SetWindowSize(100, 50);
            var dictionary = new Dictionary<string, Loader>();
            dictionary["Kort"] = loader;
            new Program(dictionary);
        }

        public Program(IReadOnlyDictionary<string, Loader> levels)
        {
            Logo();
            var result = KiesLevel(levels);
            Console.Clear();
            var drawer = new Drawer(result);

            var thread = new Thread(drawer.Start);
            thread.Start();
            while (!thread.IsAlive) { }

            var spel = new Spel(result.Spel, this);
            spel.Start();
            Console.ReadLine();
        }

        private static Loader KiesLevel(IReadOnlyDictionary<string, Loader> levels)
        {
            Console.WriteLine(@"Welk level wilt u spelen?");
            Console.WriteLine("");
            var i = 1;
            var mapping = new Dictionary<string, string>();
            foreach (var game in levels)
            {
                Console.WriteLine(@"- {0} (toets {1})", game.Key, i);
                mapping[i + ""] = game.Key;
                i++;
            }
            var keuze = "?";
            while (!mapping.ContainsKey(keuze))
            {
                keuze = Console.ReadKey().KeyChar + "";
            }
            return levels[mapping[keuze]];
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

        public Task<IVeld> VerplaatsBarricade(Logic.Barricade barricade, Func<IVeld, bool> magBarricade)
        {
            throw new NotImplementedException();
        }

        public async Task<Pion> KiesPion(ICollection<Pion> pionnen, int gedobbeld)
        {
            int pion = -1;
            while (pion < 0 || pion >= pionnen.Count)
            {
                int.TryParse(Console.ReadKey().KeyChar + "", out pion);
            }
            int i = 0;
            foreach (var current in pionnen)
            {
                if (i == pion)
                    return current;
                i++;
            }
            Console.WriteLine();
            return null;
        }

        public Task<IVeld> VerplaatsPion(Pion gekozen, ICollection<IVeld> mogelijk)
        {
            throw new NotImplementedException();
        }

        public int Gedobbeld { get; set; }
        public Speler AanDeBeurt { get; set; }
        public async Task<int> DobbelTask()
        {
            return 0;
        }
    }
}
