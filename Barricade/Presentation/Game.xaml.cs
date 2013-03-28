using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Barricade.Logic;
using Barricade.Presentation.Statisch;
using Bos = Barricade.Presentation.Statisch.Bos;
using FinishVeld = Barricade.Presentation.Statisch.FinishVeld;
using Pion = Barricade.Logic.Pion;
using StartVeld = Barricade.Presentation.Statisch.StartVeld;

namespace Barricade.Presentation
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        private readonly Spel _spel;

        // IVeld komt uit domeinlaag, IElement uit presentatielaag
        private readonly Dictionary<IVeld, IElement> _velden = new Dictionary<IVeld, IElement>(); 

        public Game(Data.Loader loader)
        {
            InitializeComponent();
            _spel = loader.Spel;

            var height = loader.Kaart.GetLength(0);
            var width = loader.Kaart.GetLength(1);

            const int nodeSize = 50;
            var pathSize = (Math.Max(width, height) > 15 ? 5 : 30);

            var gameWidth = (nodeSize + pathSize)*width - pathSize;
            var gameHeight = (nodeSize + pathSize) * (height - 1) + (200 - pathSize);

            Spelbord.Width = gameWidth;
            Spelbord.Height = gameHeight;
            for (var i = 0; i < width; i++)
            {
                var size = new GridLength(nodeSize, GridUnitType.Pixel);
                var nodeColumnDefinition = new ColumnDefinition {Width = size};
                var pathDefinition = new ColumnDefinition
                    {
                        Width = new GridLength(pathSize, GridUnitType.Pixel)
                    };
                Spelbord.ColumnDefinitions.Add(nodeColumnDefinition);
                if (i < width - 1)
                    Spelbord.ColumnDefinitions.Add(pathDefinition);
            }
            for (var i = 0; i < height - 1; i++)
            {
                var size = new GridLength(nodeSize, GridUnitType.Pixel);
                var nodeRowDefinition = new RowDefinition {Height = size};
                var pathDefinition = new RowDefinition
                    {
                        Height = new GridLength(pathSize, GridUnitType.Pixel)
                    };
                Spelbord.RowDefinitions.Add(nodeRowDefinition);
                if (i < height)
                    Spelbord.RowDefinitions.Add(pathDefinition);
            }

            var startDefinition = new RowDefinition
            {
                Height = new GridLength(200 - pathSize, GridUnitType.Pixel)
            };
            Spelbord.RowDefinitions.Add(startDefinition);

            var level = loader.Kaart;
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var veld = level[i, j];
                    if (level[i, j] == null) continue;

                    UserControl vakje = null;

                    if (veld is Logic.Bos)
                    {
                        vakje = new Bos(level[i, j]);
                        Grid.SetRow(vakje, i*2);
                        Grid.SetColumn(vakje, j*2 - 2);
                        Grid.SetColumnSpan(vakje, 5);
                    }
                    else if (veld is Logic.Startveld)
                    {
                        vakje = new StartVeld(veld as Logic.Startveld);
                        Grid.SetRow(vakje, i * 2 + 2);
                        Grid.SetRowSpan(vakje, 1);
                        Grid.SetColumn(vakje, j * 2 - 2);
                        Grid.SetColumnSpan(vakje, 5);
                    }
                    else if (veld is Logic.Finishveld)
                    {
                        vakje = new FinishVeld();
                        Grid.SetRow(vakje, i * 2);
                        Grid.SetColumn(vakje, j * 2);
                    }
                    else if (veld is Logic.Rustveld)
                    {
                        vakje = new LoopVeld()
                            {
                                IsRustVeld = true
                            };
                        Grid.SetRow(vakje, i*2);
                        Grid.SetColumn(vakje, j*2);
                    }
                    else if (veld is Logic.Veld)
                    {
                        vakje = new LoopVeld()
                            {
                                IsBarricadeVeld = (veld as Logic.Veld).StandaardBarricade
                            };
                        Grid.SetRow(vakje, i*2);
                        Grid.SetColumn(vakje, j*2);
                    }

                    if (vakje != null)
                    {
                        Spelbord.Children.Add(vakje);
                        _velden[veld] = vakje as IElement;
                    }
                    GenereerLijntjes(j, width, level, i, height);
                }
            }

//            var pion = new Images.Pion();
//            Pionnen.Children.Add(pion);
//            pion.Arrange(new Rect(new Point(20, 20), pion.DesiredSize));
        }

        private readonly List<IElement> _opgelicht = new List<IElement>(); 
        private void Viewbox_Loaded(object sender, RoutedEventArgs e)
        {
            var enumerable = from speler in _spel.Spelers select speler.Pionnen;
            var list = enumerable.SelectMany(i => i)
                      .Select(
                          pion =>
                          new
                              {
                                  pion,
                                  target = _velden[pion.IVeld].BerekenPunt(pion).TranslatePoint(new Point(0, 0), Houder)
                              });
            foreach (var item in list)
            {
                var icon =  new Dynamisch.Pion(item.pion)
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(item.target.X, item.target.Y, 0, 0)
                    };
                var item1 = item;
                Pionnen.Children.Add(icon);
                icon.Icon.Click += (o, args) =>
                {
                    var zetten = item1.pion.MogelijkeZetten(4);
                    foreach (var element in _opgelicht)
                    {
                        element.WisselLicht(false);
                    }
                    _opgelicht.Clear();
                    foreach (var zet in zetten)
                    {
                        _opgelicht.Add(_velden[zet]);
                        _velden[zet].WisselLicht(true);
                    }
                };
            }

            var barricades = from veld in _velden.Keys 
                             let veld1 = veld as Veld
                             where veld1 != null && veld1.Barricade != null
                             select new Tuple<IVeld, Logic.Barricade>(veld1, veld1.Barricade);

            foreach (var icon in
                barricades.Select(
                    barricade =>
                    new
                        {
                            barricade,
                            target =
                        _velden[barricade.Item1].BerekenPunt(barricade.Item2).TranslatePoint(new Point(0, 0), Houder)
                        })
                          .Select(@t => new Dynamisch.Barricade(@t.barricade.Item2)
                              {
                                  HorizontalAlignment = HorizontalAlignment.Left,
                                  VerticalAlignment = VerticalAlignment.Top,
                                  Margin = new Thickness(@t.target.X, @t.target.Y, 0, 0)
                              }))
            {
                Pionnen.Children.Add(icon);
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
                    for (var l = 1; l < (k - j - 1)*2 + 2; l++)
                    {
                        var line = new Rectangle
                            {
                                Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                                Height = 5,
                                Margin = new Thickness(10, 0, 10, 0)
                            };
                        Grid.SetRow(line, i*2);
                        Grid.SetColumn(line, j*2 + l - 1);
                        Grid.SetColumnSpan(line, 3);
                        Grid.SetZIndex(line, -1);
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
                    for (var l = 1; l < (k - i)*2; l++)
                    {
                        var line = new Rectangle
                            {
                                Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                                Width = 5,
                                Margin = new Thickness(0, 10, 0, 10)
                            };
                        Grid.SetRow(line, i*2 + l - 1);
                        Grid.SetRowSpan(line, 3);
                        if (k == height - 1)
                        {
                            line.Margin = new Thickness(0, 10, 0, 130);
                        }
                        Grid.SetColumn(line, j*2);
                        Grid.SetZIndex(line, -1);
                        Spelbord.Children.Add(line);
                    }
                }
                break;
            }
        }

    }
}