using System.Windows;
using System.Windows.Controls;
using Barricade.Logic;

namespace Barricade.Presentation.Blocks
{
    /// <summary>
    /// Interaction logic for Bos.xaml
    /// </summary>
    public partial class Bos : UserControl, IBlock
    {
        public Bos(IVeld vakje) 
        {
            InitializeComponent();
        }

        public UIElement PionHead(Logic.Pion pion)
        {
            return this;
        }
    }
}