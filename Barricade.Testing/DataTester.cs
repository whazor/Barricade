using System;
using Barricade.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Barricade.Testing
{
    [TestClass]
    public class DataTester
    {
        string[] level1 = new[]
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
            };

        string[] level2 = new[]
            {
    "                                    < >                                ",
    "                                     |                                 ",
    "    ( )-( )-( )-( )-( )-( )-( )-( )-[*]-( )-( )-( )-( )-( )-( )-( )-( )",
    "     |                                                               | ",
    "    ( )                                                             ( )",
    "     |                                                               | ",
    "    ( )-( )-( )-( )-( )-( )-( )-( )-[*]-( )-( )-( )-( )-( )-( )-( )-( )",
    "                                     |                                 ",
    "                                    [*]                                ",
    "                                     |                                 ",
    "                            ( )-( )-[*]-( )-( )                        ",
    "                             |               |                         ",
    "                            ( )             ( )                        ",
    "                             |               |                         ",
    "                    ( )-( )-[*]-( )-( )-( )-[*]-( )-( )                ",
    "                     |                               |                 ",
    "                    ( )                             ( )                ",
	"                     |                               |                 ",
    "            ( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )        ",
	"             |               |               |               |         ",
	"            ( )             ( )             ( )             ( )        ",
	"             |               |               |               |         ",
	"    [*]-( )-( )-( )-[*]-( )-( )-( )-[*]-( )-( )-( )-[*]-( )-( )-( )-[*]",
	"     |               |               |               |               | ",	
	"    ( )             ( )             ( )             ( )             ( )",
	"     |               |               |               |               | ",	
	"-   ( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )-( )",
	"             |               |               |               |         ",	
	"            <1>             <2>             <3>             <4>        ",	
	"*1:START,R5",
	"*2:START,G5",
	"*3:START,Y5",
	"*4:START,B5"
            };

        [TestMethod]
        public void LoadGame()
        {
            var loader1 = new Loader(level1);
            loader1.ToArray();

            var loader2 = new Loader(level2);
            loader2.ToArray();
        }

        [TestMethod]
        public void SaveGame()
        {
            var loader1 = new Loader(level1);
            loader1.ToArray();

//            var loader2 = new Loader(level2);
//            loader2.ToArray();

            var output1 = new Saver(loader1).Output();

            var loader2 = new Loader(output1);
            var output2 = new Saver(loader2).Output();
            Assert.AreEqual(output1, output2);
//            Assert.AreEqual(saver.Output(), level1, "Kaart 1 is niet hetzelfde als opgeslagen kaart.");

        }
    }
}
