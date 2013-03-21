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

namespace Barricade.Presentation
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        public Game(Data.Loader loader)
        {
            InitializeComponent();

            var height = loader.Kaart.GetLength(0);
            var width = loader.Kaart.GetLength(1);

            const int nodeSize = 50;
            const int pathSize = 5;

            var gameWidth = (nodeSize + pathSize) * width - pathSize;
            var gameHeight = (nodeSize + pathSize) * height - pathSize;

            Spelbord.Width = gameWidth;
            Spelbord.Height = gameHeight;

            for (var i = 0; i < width; i++)
            {
                var size = new GridLength(nodeSize, GridUnitType.Pixel);
                var nodeColumnDefinition = new ColumnDefinition { Width = size };
                var pathDefinition = new ColumnDefinition
                {
                    Width = new GridLength(pathSize, GridUnitType.Pixel)
                };
                Spelbord.ColumnDefinitions.Add(nodeColumnDefinition);
                if(i < width - 1)
                    Spelbord.ColumnDefinitions.Add(pathDefinition);
            }
            for (var i = 0; i < height; i++)
            {
                var size = new GridLength(nodeSize, GridUnitType.Pixel);
                var nodeRowDefinition = new RowDefinition { Height = size };
                var pathDefinition = new RowDefinition
                {
                    Height = new GridLength(pathSize, GridUnitType.Pixel)
                };
                Spelbord.RowDefinitions.Add(nodeRowDefinition);
                if (i < height - 1)
                    Spelbord.RowDefinitions.Add(pathDefinition);
            }
            var level = loader.Kaart;
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if(level[i, j] == null) continue;

                    var vakje = new Vakje(level[i, j]);
                    Grid.SetRow(vakje, i * 2);
                    Grid.SetColumn(vakje, j * 2);
                    Spelbord.Children.Add(vakje);

//                    

                }
            }
        }
    }
}
