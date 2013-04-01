using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Barricade.Presentation.Dynamisch
{
    /// <summary>
    /// Interaction logic for Pion.xaml
    /// </summary>
    public partial class Pion : UserControl
    {
        public Logic.Pion Stuk { get; private set; }

        public Pion(Logic.Pion pion)
        {
            Stuk = pion;
            InitializeComponent();
            //Icon.Content = pion.Speler.Name;
            Icon.Fill = new SolidColorBrush(pion.Speler.Kleur);
            BorderBrush = new SolidColorBrush(Color.FromRgb(111, 0, 111));   
        }

        public void WisselLicht(bool b)
        {
            BorderThickness = new Thickness(b ? 5 : 0);
        }
    }
}
