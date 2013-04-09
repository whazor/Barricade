using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Barricade.Logic.Velden;
using Barricade.Presentation.Statisch;

namespace Barricade.Presentation
{
    public class StatischeLaag
    {
        private readonly Grid _grid;
        // IVeld komt uit domeinlaag, IElement uit presentatielaag
        public readonly Dictionary<IVeld, IElement> Velden = new Dictionary<IVeld, IElement>();
        public delegate void VeldClickHandler(IVeld sender);
        public event VeldClickHandler VeldKlik;

        public StatischeLaag(Grid grid, IVeld[,] kaart)
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

            PlaatsVakjes(kaart, height, width, nodeSize, pathSize);
        }

        private void PlaatsVakjes(IVeld[,] kaart, int height, int width, int nodeSize, int pathSize)
        {
            var level = kaart;
            for (var i = 0; i < height; i++)
            {
                int first = width;
                int last = 0;
                var isDorp = false;
                for (var j = 0; j < width; j++)
                {
                    var veld = level[i, j];
                    if (level[i, j] == null) continue;
                    if (level[i, j].IsDorp) isDorp = true;

                    first = Math.Min(j, first);
                    last = Math.Max(j, last);

                    UserControl vakje = null;
                    var vakjeWidth = nodeSize;
                    var vakjeHeight = nodeSize;

                    if (veld is Logic.Velden.Bos)
                    {
                        vakje = new Statisch.Bos(level[i, j]);
                        vakjeWidth *= 2;
                    }
                    else if (veld is Startveld)
                    {
                        vakje = new StartVeld(veld as Startveld);
                        vakjeWidth = 150;
                        vakjeHeight = 150;
                    }
                    else if (veld is Finishveld)
                    {
                        vakje = new FinishVeld(veld);
                    }
                    else if (veld is Rustveld)
                    {
                        vakje = new LoopVeld(veld) { IsRustVeld = true };
                    }
                    else if (veld is Veld)
                    {
                        vakje = new LoopVeld(veld) { IsBarricadeVeld = (veld as Veld).StandaardBarricade };
                    }

                    if (vakje != null)
                    {
                        vakje.Width = vakjeWidth;
                        vakje.Height = vakjeHeight;
                        vakje.Margin = new Thickness(
                            nodeSize * j + pathSize * (j-1) + (nodeSize - vakjeWidth) / 2,
                            nodeSize * i + pathSize * i,
                            0, 0);
                        vakje.HorizontalAlignment = HorizontalAlignment.Left;
                        vakje.VerticalAlignment = VerticalAlignment.Top;
                        _grid.Children.Add(vakje);
                        vakje.MouseUp += (sender, args) =>
                            {
                                if (VeldKlik != null && sender != null)
                                {
                                    var target = vakje as IElement;
                                    VeldKlik(target.Veld);
                                }
                                    
                            };
                        Velden[veld] = vakje as IElement;
                    }
                    GenereerLijntjes(j, width, level, i, height, nodeSize, pathSize);
                }
                if (isDorp)
                {
                    var el = new Border
                        {
                            CornerRadius = new CornerRadius(nodeSize/2,nodeSize/2, nodeSize/2, nodeSize/2),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Background = new SolidColorBrush(Color.FromArgb(130, 255, 255, 255)),
                            Width = (nodeSize + pathSize)*(last - first + 1) - pathSize,
                            Height = nodeSize,
                            Margin = new Thickness(nodeSize * first + pathSize * (first - 1), (nodeSize + pathSize) * i, 0, 0)
                        };
                    Panel.SetZIndex(el, -2);
                    _grid.Children.Add(el);
                }

            }
        }

        private void GenereerLijntjes(int j, int width, IVeld[,] level, int i, int height, int nodeSize, int pathSize)
        {
            for (var k = j + 1; k < width; k++)
            {
                if (level[i, k] == null)
                {
                    continue;
                }
                if (level[i, j].Buren.Contains(level[i, k]))
                {
                    for (var l = 1; l < (k - j - 1)*2 + 2; l++)
                    {
                        var line = new Rectangle
                            {
                                Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                                Height = 5,
                                Width = pathSize * (k - j) + nodeSize * (k - j - 1),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top
                            };
                        line.Margin = new Thickness(
                            nodeSize * (j + 1) + pathSize * (j - 1),
                            nodeSize * i + pathSize * (i) + nodeSize / 2 - line.Height / 2,
                            0, 0);

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
                            Height = pathSize * (k - i) + nodeSize * (k - i - 1),
                            //Margin = new Thickness(0, 10, 0, 10),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top
                        };
                        line.Margin = new Thickness(
                            nodeSize * (j) + pathSize * (j - 1) - line.Width / 2 + nodeSize / 2,
                            nodeSize * (i + 1) + pathSize * (i),
                            0, 0);
                        Panel.SetZIndex(line, -1);
                        _grid.Children.Add(line);
                    }
                }
                break;
            }
        }

        public void Highlight(IEnumerable<IVeld> velden, bool status)
        {
            foreach (var veld in velden)
            {
                Velden[veld].WisselLicht(status);
            }
        }

        
    }
}