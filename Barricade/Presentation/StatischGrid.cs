using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Barricade.Logic;
using Barricade.Presentation.Statisch;
using Bos = Barricade.Logic.Bos;

namespace Barricade.Presentation
{
    public class StatischGrid
    {
        private readonly Grid _grid;
        // IVeld komt uit domeinlaag, IElement uit presentatielaag
        public readonly Dictionary<IVeld, IElement> Velden = new Dictionary<IVeld, IElement>();

        public StatischGrid(Grid grid, IVeld[,] kaart)
        {
            _grid = grid;

            var height = kaart.GetLength(0);
            var width = kaart.GetLength(1);

            const int nodeSize = 50;
            var pathSize = (Math.Max(width, height) > 15 ? 5 : 30);

            var gameWidth = (nodeSize + pathSize) * width - pathSize;
            var gameHeight = (nodeSize + pathSize) * (height - 1) + (200 - pathSize);

            grid.Width = gameWidth;
            grid.Height = gameHeight;
            for (var i = 0; i < width; i++)
            {
                var size = new GridLength(nodeSize, GridUnitType.Pixel);
                var nodeColumnDefinition = new ColumnDefinition { Width = size };
                var pathDefinition = new ColumnDefinition
                {
                    Width = new GridLength(pathSize, GridUnitType.Pixel)
                };
                grid.ColumnDefinitions.Add(nodeColumnDefinition);
                if (i < width - 1)
                    grid.ColumnDefinitions.Add(pathDefinition);
            }
            for (var i = 0; i < height - 1; i++)
            {
                var size = new GridLength(nodeSize, GridUnitType.Pixel);
                var nodeRowDefinition = new RowDefinition { Height = size };
                var pathDefinition = new RowDefinition
                {
                    Height = new GridLength(pathSize, GridUnitType.Pixel)
                };
                grid.RowDefinitions.Add(nodeRowDefinition);
                if (i < height)
                    grid.RowDefinitions.Add(pathDefinition);
            }

            var startDefinition = new RowDefinition
            {
                Height = new GridLength(200 - pathSize, GridUnitType.Pixel)
            };
            grid.RowDefinitions.Add(startDefinition);

            var level = kaart;
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var veld = level[i, j];
                    if (level[i, j] == null) continue;

                    UserControl vakje = null;

                    if (veld is Logic.Bos)
                    {
                        vakje = new Statisch.Bos(level[i, j]);
                        Grid.SetRow(vakje, i * 2);
                        Grid.SetColumn(vakje, j * 2 - 2);
                        Grid.SetColumnSpan(vakje, 5);
                    }
                    else if (veld is Logic.Startveld)
                    {
                        vakje = new Statisch.StartVeld(veld as Logic.Startveld);
                        Grid.SetRow(vakje, i * 2 + 2);
                        Grid.SetRowSpan(vakje, 1);
                        Grid.SetColumn(vakje, j * 2 - 2);
                        Grid.SetColumnSpan(vakje, 5);
                    }
                    else if (veld is Logic.Finishveld)
                    {
                        vakje = new Statisch.FinishVeld();
                        Grid.SetRow(vakje, i * 2);
                        Grid.SetColumn(vakje, j * 2);
                    }
                    else if (veld is Logic.Rustveld)
                    {
                        vakje = new Statisch.LoopVeld()
                        {
                            IsRustVeld = true
                        };
                        Grid.SetRow(vakje, i * 2);
                        Grid.SetColumn(vakje, j * 2);
                    }
                    else if (veld is Logic.Veld)
                    {
                        vakje = new Statisch.LoopVeld()
                        {
                            IsBarricadeVeld = (veld as Logic.Veld).StandaardBarricade
                        };
                        Grid.SetRow(vakje, i * 2);
                        Grid.SetColumn(vakje, j * 2);
                    }

                    if (vakje != null)
                    {
                        grid.Children.Add(vakje);
                        Velden[veld] = vakje as IElement;
                    }
                    GenereerLijntjes(j, width, level, i, height);
                }
            }
        }


        private void GenereerLijntjes(int j, int width, IVeld[,] level, int i, int height)
        {
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
                        var line = new Rectangle
                        {
                            Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                            Height = 5,
                            Margin = new Thickness(10, 0, 10, 0)
                        };
                        Grid.SetRow(line, i * 2);
                        Grid.SetColumn(line, j * 2 + l - 1);
                        Grid.SetColumnSpan(line, 3);
                        Panel.SetZIndex(line, -1);
                        _grid.Children.Add(line);
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
                        var line = new Rectangle
                        {
                            Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                            Width = 5,
                            Margin = new Thickness(0, 10, 0, 10)
                        };
                        Grid.SetRow(line, i * 2 + l - 1);
                        Grid.SetRowSpan(line, 3);
                        if (k == height - 1)
                        {
                            line.Margin = new Thickness(0, 10, 0, 130);
                        }
                        Grid.SetColumn(line, j * 2);
                        Panel.SetZIndex(line, -1);
                        _grid.Children.Add(line);
                    }
                }
                break;
            }
        }
    }
}