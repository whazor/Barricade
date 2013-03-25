using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Barricade.Presentation.Images
{
    /// <summary>
    /// Interaction logic for Veld.xaml
    /// </summary>
    public partial class Veld : UserControl
    {
        private bool _isBarricadeVeld = false;
        private bool _isRustVeld;

        public Veld()
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
    }
}
