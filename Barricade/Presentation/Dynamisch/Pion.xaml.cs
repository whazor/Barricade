using System.Collections.Generic;
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
            
        }
    }
}
