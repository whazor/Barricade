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
using Logic;

namespace Barricade.Presentation
{
    /// <summary>
    /// Interaction logic for Vakje.xaml
    /// </summary>
    public partial class Vakje : UserControl
    {
        public Vakje(IVeld veld)
        {
            InitializeComponent();
            BurenCount.Content = veld.Buren.Count + "";
            if (!(veld is Bos)) return;
            Width = 100;
            Background = new SolidColorBrush(Color.FromRgb(100, 235, 100));
        }
    }
}
