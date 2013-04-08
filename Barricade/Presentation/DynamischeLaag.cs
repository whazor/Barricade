using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Barricade.Logic;
using Barricade.Logic.Velden;
using Barricade.Presentation.Statisch;

namespace Barricade.Presentation
{
    public class DynamischeLaag
    {
        private readonly Grid _dynamischGrid;
        private readonly Grid _houder;
        private readonly Dictionary<IVeld, IElement> _velden;
        private readonly Dictionary<Logic.Pion, Dynamisch.Pion> _poinnen = new Dictionary<Pion, Dynamisch.Pion>();
        private readonly Dictionary<Logic.Barricade, Dynamisch.Barricade> _barricades = new Dictionary<Logic.Barricade, Dynamisch.Barricade>();

        const int milliseconds = 500;

        public DynamischeLaag(Grid dynamischGrid, Grid houder, Dictionary<IVeld, IElement> velden)
        {
            _dynamischGrid = dynamischGrid;
            _houder = houder;
            _velden = velden;
        }


        public delegate void PionClickHandler(Logic.Pion sender);

        public event PionClickHandler PionKlik;

        public void TekenPionnen(List<Pion> pionnen)
        {
            foreach (var pion in pionnen)
            {
                // Kijk waar de pion heen moet
                var target = _velden[pion.IVeld].BerekenPunt(pion).TranslatePoint(new Point(0, 0), _houder);

                var icon = new Dynamisch.Pion(pion)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(target.X, target.Y, 0, 0)
                };
                // Toon barricade
                _poinnen.Add(pion, icon);
                _dynamischGrid.Children.Add(icon);

                // Kijk naar wijzingen
                pion.PositieWijziging += Beweeg;
                icon.MouseUp += (o, args) => PionKlik(((Dynamisch.Pion)o).Stuk);
            }
        }

        public void TekenBarricades(List<Tuple<IVeld, Logic.Barricade>> barricades)
        {
            foreach (var tuple in barricades)
            {
                var barricade = tuple.Item2;
                var plaats = tuple.Item1;
                // Kijk waar de barricade heen moet
                var target = _velden[plaats].BerekenPunt(barricade).TranslatePoint(new Point(0, 0), _houder);

                // Maak een barricade aan
                var icon = new Dynamisch.Barricade(barricade)
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(target.X, @target.Y, 0, 0)
                    };

                // Toon barricade
                _barricades.Add(barricade, icon);
                _dynamischGrid.Children.Add(icon);

                // Kijk naar wijzingen
                barricade.PositieWijziging += Beweeg;
            }
        }

        private void Beweeg(Logic.Barricade barricade, Veld bestemming)
        {
            _barricades[barricade].Beweeg(_velden[bestemming].BerekenPunt(barricade).TranslatePoint(new Point(0.0, 0.0), _houder));
        }

        public void Beweeg(Pion pion, IVeld bestemming)
        {
            var icon = _poinnen[pion];

            List<IVeld> list;
            if (pion.Paden != null && pion.Paden.ContainsKey(bestemming))
            {
                list = new List<IVeld>(pion.Paden[bestemming]);
            }
            else
            {
                list = new List<IVeld> {bestemming};
            }
            var stack = list.Select(veld => _velden[veld].BerekenPunt(pion).TranslatePoint(new Point(0.0, 0.0), _houder));
            //IEnumerable<Point> stack = Enumerable.Select<IElement, Point>(list, icon.BerekenPunt(pion).TranslatePoint(new Point(0.0, 0.0), _houder))));
            icon.Beweeg(stack);
//            Beweeg(pion, stack, icon, milliseconds);
        }


        public UserControl Zoek(Logic.Barricade barricade)
        {
            return _barricades[barricade];
        }

        public void Highlight(IEnumerable<Pion> pionnen, bool status)
        {
            foreach (var pion in pionnen)
            {
                _poinnen[pion].WisselLicht(status);
            }
        }
    }
}