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

namespace Barricade.Presentation.Images
{
    /// <summary>
    /// Interaction logic for StartVeld.xaml
    /// </summary>
    public partial class StartVeld : UserControl
    {
        const int Hoeveel = 5;
        private readonly Ellipse[] _ellipses;

        public StartVeld()
        {
            InitializeComponent();

            // Maak alle velden in het startveld aan.
            _ellipses = new Ellipse[Hoeveel];
            for (var i = 0; i < Hoeveel; i++)
            {
                // positie en uiterlijk van veld
                _ellipses[i] = new Ellipse() {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    StrokeThickness = 2
                    
                };
                Holder.Children.Add(_ellipses[i]);
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

            var radius = size / 2 - bigRadius;

            for (var i = 0; i < Hoeveel; i++)
            {
                const double kwart = (Math.PI / 2); // ook wel 90 graden
                // reken uit hoeveel rondjes er in het grote rondje passen
                var draai = kwart + ((2*Math.PI)/Hoeveel)*i;

                var top = Math.Sin(draai) * bigRadius;
                var left = Math.Cos(draai) * bigRadius;

                _ellipses[i].Margin = new Thickness(
                    (size / 2) - smallRadius - left,
                    (size / 2) - smallRadius - top,
                    0, 0);
                _ellipses[i].Width = smallRadius * 2;
                _ellipses[i].Height = smallRadius * 2;
            }
        }
    }
}
