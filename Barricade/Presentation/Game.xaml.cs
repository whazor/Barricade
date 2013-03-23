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
                    if (level[i, j] is Logic.Bos)
                    {
                        Grid.SetColumn(vakje, j * 2 - 2);
                        Grid.SetColumnSpan(vakje, 5);
                    }
                    Spelbord.Children.Add(vakje);

                    for (var k = j + 1; k < width; k++)
                    {
                        if (level[i, k] == null)
                        {
                            break;
                        }
                        if (level[i, j].Buren.Contains(level[i, k]))
                        {
                            for (var l = 1; l < (k - j - 1) * 2 + 2; l++)
                            {
                                var line = new Rectangle {Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)), Height = 5};
                                Grid.SetRow(line, i * 2);
                                Grid.SetColumn(line, j * 2 + l);
                                Spelbord.Children.Add(line);
                            }
                        }
                        break;
                    }
                    for (var k = i + 1; k < height; k++)
                    {
                        if (level[k, j] == null)
                        {
                            continue;
                        }
                        if (level[i, j].Buren.Contains(level[k, j]))
                        {
                            for (var l = 1; l < (k - i) * 2; l++)
                            {
                                var line = new Rectangle { Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)), Width = 5 };
                                Grid.SetRow(line, i * 2 + l);
                                Grid.SetColumn(line, j * 2 );
                                Spelbord.Children.Add(line);
                            }
                        }
                        break;
                    }
                
                }
            }
        }
    }
}
