using System.Windows;
using System.Windows.Controls;

namespace Barricade.Presentation.Statisch
{
    /// <summary>
    /// Interaction logic for Pion.xaml
    /// </summary>
    public partial class Pion : UserControl {
        public Pion(Logic.Pion pion)
        {
            InitializeComponent();

            Icon.Content = pion.Speler.Name;
        }
    }
}
