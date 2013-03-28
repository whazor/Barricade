using System.Windows;
using System.Windows.Controls;
using Barricade.Logic;

namespace Barricade.Presentation.Statisch
{
    /// <summary>
    /// Interaction logic for Bos.xaml
    /// </summary>
    public partial class Bos : UserControl, IElement
    {
        public Bos(IVeld vakje) 
        {
            InitializeComponent();
        }

        public UIElement BerekenPunt(Logic.Pion pion)
        {
            return this;
        }
    }
}