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

        public DynamischeLaag(Grid dynamischGrid, Grid houder, Dictionary<IVeld, IElement> velden)
        {
            _dynamischGrid = dynamischGrid;
            _houder = houder;
            _velden = velden;
        }


        public delegate void PionClickHandler(Dynamisch.Pion item, Logic.Pion sender);

        public event PionClickHandler PionClick;

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
                _poinnen.Add(item.pion, icon);
                _dynamischGrid.Children.Add(icon);

                icon.MouseUp += (o, args) => PionClick((Dynamisch.Pion)o, ((Dynamisch.Pion)o).Stuk);
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

        public void Beweeg(Pion pion, IList<IVeld> velds)
        {
            var icon = _poinnen[pion];
            const int milliseconds = 500;
            var stack = new Stack<IVeld>(velds);
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
    }
}