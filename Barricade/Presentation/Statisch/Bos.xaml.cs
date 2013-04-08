using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Barricade.Logic;
using Barricade.Logic.Velden;

namespace Barricade.Presentation.Statisch
{
    /// <summary>
    /// Interaction logic for Bos.xaml
    /// </summary>
    public partial class Bos : UserControl, IElement
    {
        private int i;
        private List<Rectangle> rectangles = new List<Rectangle>();
        public Bos(IVeld vakje)
        {
            Veld = vakje;
            InitializeComponent();

            for (int j = 0; j < 6; j++)
            {
                var rectangle = new Rectangle
                {
                    Margin = new Thickness(40 * i++, -50, 0, 0),
                    Width = 10,
                    Height = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Targets.Children.Add(rectangle);
                rectangles.Add(rectangle);
            }
        }

        private int k;
        public UIElement BerekenPunt(Logic.Pion pion)
        {
            return rectangles[k++ % 6];
        }

        public UIElement BerekenPunt(Logic.Barricade barricade)
        {
            return this;
        }

        public void WisselLicht(bool status)
        {
            BorderThickness = new Thickness(status ? 5 : 0);
            BorderBrush = new SolidColorBrush(Color.FromRgb(244,0, 233));
        }

        public IVeld Veld { get; private set; }
    }
}