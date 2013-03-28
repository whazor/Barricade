using System.Windows;
using System.Windows.Controls;

namespace Barricade.Presentation.Statisch
{
    /// <summary>
    /// Interaction logic for LoopVeld.xaml
    /// </summary>
    public partial class LoopVeld : UserControl, IElement
    {
        private bool _isBarricadeVeld = false;
        private bool _isRustVeld;

        public LoopVeld()
        {
            InitializeComponent();
        }

        public bool IsRustVeld
        {
            get { return _isRustVeld; }
            set
            {
                _isRustVeld = value;
                if (_isRustVeld)
                {
                    Overlay.Style = (Style)FindResource("RustStyle");
                }
                else if (!_isBarricadeVeld)
                {
                    Overlay.Style = null;
                }
            }
        }

        public bool IsBarricadeVeld
        {
            get { return _isBarricadeVeld; }
            set
            {
                _isBarricadeVeld = value;
                if (_isBarricadeVeld)
                {
                    Overlay.Style = (Style) FindResource("BarricadeStyle");
                }
                else if (!_isRustVeld)
                {
                    Overlay.Style = null;
                }
            }
        }

        public UIElement BerekenPunt(Logic.Pion pion)
        {
            return this;
        }
    }
}
