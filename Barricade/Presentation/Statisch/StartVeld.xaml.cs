using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Barricade.Logic;

namespace Barricade.Presentation.Statisch
{
    /// <summary>
    /// Interaction logic for StartVeld.xaml
    /// </summary>
    public partial class StartVeld : UserControl, IElement
    {
        private readonly Dictionary<Logic.Pion, Ellipse> _ellipses = new Dictionary<Logic.Pion, Ellipse>(); 
//        private readonly Ellipse[] _ellipses;

        public StartVeld(Logic.Startveld veld)
        {
            InitializeComponent();
            Veld = veld;

            // Maak alle velden in het startveld aan.
//            _ellipses = new Ellipse[_hoeveel];
            foreach (var pion in veld.Speler.Pionnen)
            {
                var ellipse = new Ellipse
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    StrokeThickness = 2
                };
                _ellipses.Add(pion, ellipse);
                Holder.Children.Add(ellipse);   
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // uitrekenen welke lengte aangehouden moet worden
            var size = Math.Min(e.NewSize.Height, e.NewSize.Width);
            Holder.Width = size;
            Holder.Height = size;

            const int smallRadius = 25;
            const int bigRadius = 50;

            MainEllipse.Margin = new Thickness(
                size/2 - bigRadius,
                size/2 - bigRadius,
                0, 0);
            MainEllipse.Width = bigRadius*2;
            MainEllipse.Height = bigRadius*2;

            var i = 0;
            foreach (var ellipse in _ellipses.Values)
            {
                const double kwart = (Math.PI / 2); // ook wel 90 graden
                // afstand tussen de startvelden
                var afstand = (2 * Math.PI) / _ellipses.Count;
                // reken uit hoeveel rondjes er in het grote rondje passen
                var draai = kwart + afstand * (i++);

                var top = Math.Sin(draai) * bigRadius;
                var left = Math.Cos(draai) * bigRadius;
                ellipse.Margin = new Thickness(
                    (size / 2) - smallRadius - left,
                    (size / 2) - smallRadius - top,
                    0, 0);
                ellipse.Width = smallRadius * 2;
                ellipse.Height = smallRadius * 2;
            }
        }

        public UIElement BerekenPunt(Logic.Pion pion)
        {
            return _ellipses[pion];
        }

        public UIElement BerekenPunt(Logic.Barricade barricade)
        {
            return this;
        }

        public void WisselLicht(bool status)
        {
            BorderThickness = new Thickness(status ? 5 : 0);
            BorderBrush = new SolidColorBrush(Color.FromRgb(244, 0, 233));
        }

        public IVeld Veld { get; private set; }
    }
}
