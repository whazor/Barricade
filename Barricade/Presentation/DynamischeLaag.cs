using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Barricade.Logic;
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

        private void Beweeg(Logic.Barricade barricade, Veld nieuwveld)
        {
            var icon = _barricades[barricade];
            var target = _velden[nieuwveld].BerekenPunt(barricade).TranslatePoint(new Point(0, 0), _houder);
            var thickness = new Thickness(target.X, target.Y, 0, 0);
            var moveAnimation = new ThicknessAnimation(icon.Margin, thickness, TimeSpan.FromMilliseconds(milliseconds));
            icon.BeginAnimation(FrameworkElement.MarginProperty, moveAnimation);
        }

        public void Beweeg(Pion pion, IVeld bestemming)
        {
            var icon = _poinnen[pion];

            Stack<IVeld> stack;
            if (pion.Paden != null && pion.Paden[bestemming] != null)
            {
                stack = new Stack<IVeld>(pion.Paden[bestemming]);
            }
            else
            {
                stack = new Stack<IVeld>();
                stack.Push(bestemming);                
            }
            Beweeg(pion, stack, icon, milliseconds);
        }

        private void Beweeg(Pion pion, Stack<IVeld> stack, FrameworkElement icon, int milliseconds)
        {
            if (!stack.Any()) return;
            
            var target = _velden[stack.Pop()].BerekenPunt(pion).TranslatePoint(new Point(0, 0), _houder);
            var thickness = new Thickness(target.X, target.Y, 0, 0);
            var moveAnimation = new ThicknessAnimation(icon.Margin, thickness, TimeSpan.FromMilliseconds(milliseconds));
            moveAnimation.Completed += (sender, args) => Beweeg(pion, stack, icon, milliseconds);
            icon.BeginAnimation(FrameworkElement.MarginProperty, moveAnimation);
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