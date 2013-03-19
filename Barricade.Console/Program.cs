using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barricade.Data;

namespace Barricade.Shell
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
                    "*2:START,RRRR",
                    "*3:START,GGGG",
                    "*4:START,YYYY",
                    "*5:START,BBBB"
                }
            );

            var saver = new Saver(loader.Kaart);

            Console.WriteLine(saver.Output());
            Console.Read();
//            loader.ToArray();
        }
    }
}
