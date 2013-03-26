using System.Windows;
using System.Windows.Controls;

namespace Barricade.Presentation.Blocks
{
    /// <summary>
    /// Interaction logic for FinishVeld.xaml
    /// </summary>
    public partial class FinishVeld : UserControl, IBlock
    {
        public FinishVeld()
        {
            InitializeComponent();
        }

        public UIElement PionHead(Logic.Pion pion)
        {
            return this;
        }
    }
}
