using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Barricade.Logic;

namespace Barricade.Presentation.Statisch
{
    /// <summary>
    /// Interaction logic for FinishVeld.xaml
    /// </summary>
    public partial class FinishVeld : UserControl, IElement
    {
        public FinishVeld(IVeld vakje)
        {
            Veld = vakje;
            InitializeComponent();
        }

        public UIElement BerekenPunt(Logic.Pion pion)
        {
            return this;
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
