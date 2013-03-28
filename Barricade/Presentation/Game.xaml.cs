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

        
        private readonly StatischGrid _statischGrid;

        public Game(Data.Loader loader)
        {
            InitializeComponent();
            _spel = loader.Spel;
            _statischGrid = new StatischGrid(Spelbord, loader.Kaart);
            Loaded += Game_Loaded;
        }

        private Pion pion;
        private readonly List<IElement> _opgelicht = new List<IElement>();
        private List<List<IVeld>> _mogelijkePaden = new List<List<IVeld>>();
        private void Game_Loaded(object sender, RoutedEventArgs e)
        {
            DynamischGrid.Children.Clear();
            TekenPionnen();
            TekenBarricades();
        }

        private void TekenPionnen()
        {
            var enumerable = from speler in _spel.Spelers select speler.Pionnen;
            var list = enumerable.SelectMany(i => i)
                                 .Select(
                                     found =>
                                     new
                                         {
                                             pion = found,
                                             target =
                                         _statischGrid.Velden[found.IVeld].BerekenPunt(found)
                                                                          .TranslatePoint(new Point(0, 0), Houder)
                                         });
            foreach (var item in list)
            {
                var icon = new Dynamisch.Pion(item.pion)
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(item.target.X, item.target.Y, 0, 0)
                    };
                var item1 = item;
                DynamischGrid.Children.Add(icon);
                icon.Icon.Click += (o, args) =>
                    {
                        pion = item1.pion;
                        _mogelijkePaden = item1.pion.MogelijkeZetten(4);
                        foreach (var element in _opgelicht)
                        {
                            element.WisselLicht(false);
                        }
                        _opgelicht.Clear();
                        foreach (var zet in _mogelijkePaden.Select(l => l.First()))
                        {
                            _opgelicht.Add(_statischGrid.Velden[zet]);
                            _statischGrid.Velden[zet].WisselLicht(true);
                        }
                    };
            }
        }

        private void TekenBarricades()
        {
            var barricades = from veld in _statischGrid.Velden.Keys
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
                    _statischGrid.Velden[barricade.Item1].BerekenPunt(barricade.Item2)
                                                         .TranslatePoint(new Point(0, 0), Houder)
                    })
                          .Select(@t => new Dynamisch.Barricade(@t.barricade.Item2)
                          {
                              HorizontalAlignment = HorizontalAlignment.Left,
                              VerticalAlignment = VerticalAlignment.Top,
                              Margin = new Thickness(@t.target.X, @t.target.Y, 0, 0)
                          }))
            {
                DynamischGrid.Children.Add(icon);
            }
        }

    }
}