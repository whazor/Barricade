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

            if (veld is Bos)
            {
                Width = 110;
                Plaatje.Source = (ImageSource) FindResource("Bos");
            }
            else if (veld is Finishveld)
            {
                Plaatje.Source = (ImageSource) FindResource("Finish");
            }
            else if (veld is Rustveld)
            {
                Plaatje.Source = (ImageSource)FindResource("Rust");
            }
            else if (veld is Veld)
            {
                if ((veld as Veld).StandaardBarricade)
                {
                    Plaatje.Source = (ImageSource)FindResource("Barricade");    
                }
            }
            //Background = new SolidColorBrush(Color.FromRgb(100, 235, 100));
        }
    }
}
