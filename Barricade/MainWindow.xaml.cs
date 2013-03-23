using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Barricade.Data;
using Barricade.Presentation;

namespace Barricade
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonLang_Click(object sender, RoutedEventArgs e)
        {
            var level2 = new[]
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
	"*1:START,RRRRR",
	"*2:START,GGGGG",
	"*3:START,YYYYY",
	"*4:START,BBBBB"
            };
            var game = new Game(new Loader(level2));
            Content = game;
        }

        private void ButtonKort_Click(object sender, RoutedEventArgs e)
        {
            var level2 = new[]
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
            };
            var game = new Game(new Loader(level2));
            Content = game;
        }
    }
}
