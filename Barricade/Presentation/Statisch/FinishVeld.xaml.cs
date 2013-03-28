using System.Windows;
using System.Windows.Controls;

namespace Barricade.Presentation.Statisch
{
    /// <summary>
    /// Interaction logic for FinishVeld.xaml
    /// </summary>
    public partial class FinishVeld : UserControl, IElement
    {
        public FinishVeld()
        {
            InitializeComponent();
        }

        public UIElement BerekenPunt(Logic.Pion pion)
        {
            return this;
        }
    }
}
