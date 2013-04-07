using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barricade.Data;

namespace Barricade.Text
{
    class Program
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

            var saver = new Saver(loader.Kaart);

            Console.WriteLine("");
            Console.WriteLine(@"  .---.           .--.        .--.                             .     ");
            Console.WriteLine(@"      |           |   :       |   )              o             |     ");
            Console.WriteLine(@"      |.  . .-.   |   | .-.   |--:  .-.  .--..--..  .-..-.  .-.| .-. ");
            Console.WriteLine(@"      ;|  |(.-'   |   ;(.-'   |   )(   ) |   |   | (  (   )(   |(.-' ");
            Console.WriteLine(@"  `--' `--`-`--'  '--'  `--'  '--'  `-'`-'   ' -' `-`-'`-'`-`-'`-`--'");
            Console.WriteLine("");
            Console.WriteLine(@"Welk level wilt u spelen?");
            Console.WriteLine("");
            Console.WriteLine(@"- Lang (toets 1)");
            Console.WriteLine(@"- Kort (toets 2)");
            //TODO: verschillende levels inladen

            var uitkomst = Console.ReadKey();
            Console.Clear();
            Console.WriteLine(saver.Output(true));
            Console.ReadLine();
//            loader.ToArray();
        }
    }
}
