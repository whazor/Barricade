using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Barricade.Logic;
using Barricade.Presentation.Statisch;

namespace Barricade.Presentation
{
    public class DynamischeLaag
    {
        private readonly Grid _dynamischGrid;
        private readonly Grid _houder;
        private readonly Dictionary<IVeld, IElement> _velden;

        public DynamischeLaag(Grid dynamischGrid, Grid houder, Dictionary<IVeld, IElement> velden)
        {
            _dynamischGrid = dynamischGrid;
            _houder = houder;
            _velden = velden;
        }
        private Pion pion;
        private readonly List<IElement> _opgelicht = new List<IElement>();
        private List<List<IVeld>> _mogelijkePaden = new List<List<IVeld>>();
        public void TekenPionnen(List<Pion> pionnen)
        {
            var list = pionnen.Select(
                         found =>
                         new
                         {
                             pion = found,
                             target =
                         _velden[found.IVeld].BerekenPunt(found)
                                                          .TranslatePoint(new Point(0, 0), _houder)
                         });
            foreach (var item in list)
            {
                var icon = new Dynamisch.Pion(item.pion)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(item.target.X, item.target.Y, 0, 0)
                };
                _dynamischGrid.Children.Add(icon);

                icon.Icon.Click += (o, args) =>
                {
                    pion = item.pion;
                    _mogelijkePaden = item.pion.MogelijkeZetten(4);
                    foreach (var element in _opgelicht)
                    {
                        element.WisselLicht(false);
                    }
                    _opgelicht.Clear();
                    foreach (var zet in _mogelijkePaden.Select(l => l.First()))
                    {
                        _opgelicht.Add(_velden[zet]);
                        _velden[zet].WisselLicht(true);
                    }
                };
            }
        }

        public void TekenBarricades(List<Tuple<IVeld, Logic.Barricade>> barricades)
        {
            //var barricades = lijst.Select(@t => new Tuple<IVeld, Logic.Barricade>(@t.veld1, @t.veld1.Barricade));

            foreach (var icon in
                barricades.Select(
                    barricade =>
                    new
                    {
                        barricade,
                        target =
                    _velden[barricade.Item1].BerekenPunt(barricade.Item2)
                                                         .TranslatePoint(new Point(0, 0), _houder)
                    })
                          .Select(@t => new Dynamisch.Barricade(@t.barricade.Item2)
                          {
                              HorizontalAlignment = HorizontalAlignment.Left,
                              VerticalAlignment = VerticalAlignment.Top,
                              Margin = new Thickness(@t.target.X, @t.target.Y, 0, 0)
                          }))
            {
                _dynamischGrid.Children.Add(icon);
            }
        }
    }
}