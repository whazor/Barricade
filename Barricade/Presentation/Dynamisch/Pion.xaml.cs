using System.Windows.Controls;

namespace Barricade.Presentation.Dynamisch
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
