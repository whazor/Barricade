using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Barricade.Logic;
using Barricade.Utilities;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace Barricade.Presentation
{
    /// <summary>
    /// Interaction logic for UserInterface.xaml
    /// </summary>
    public partial class UserInterface : UserControl
    {
        private readonly Game _game;
        private int _gedobbeld;
        private Speler _speler;
        Waiter<Tuple<Speler, int>> _wachter = new Waiter<Tuple<Speler, int>>();
        private Speler _isAanBeurt;

        public UserInterface(Game game, IEnumerable<Speler> spelers)
        {
            _game = game;
            InitializeComponent();

            SpelerKiezer.ItemsSource = spelers;
        }

        public int Gedobbeld
        {
            get { return _gedobbeld; }
            set
            {
                _gedobbeld = value;
                GetalLabel.Text = value + "";
            }
        }

        public Speler IsAanBeurt
        {
            get { return _isAanBeurt; }
            set
            {
                _isAanBeurt = value;
                KnopRand.Stroke = new SolidColorBrush(value.Kleur);
                SpelerKiezer.SelectedItem = value;
            }
        }

        private void SpelerKiezer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsAanBeurt = (Speler) SpelerKiezer.SelectedItem;
        }

        private void GetalLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            int.TryParse(GetalLabel.Text, out result);
            if (result > 0 && result <= 9)
            {
                _gedobbeld = result;
            }
        }

        private void DobbelKnop_Click(object sender, RoutedEventArgs e)
        {
            _wachter.Return(new Tuple<Speler, int>((Speler) SpelerKiezer.SelectedItem, _gedobbeld));
        }

        private void AfsluitKnop_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Weet je zeker dat je het huidige spel wilt verlaten?", "Spel afbreken",
                                MessageBoxButton.YesNo).ToString() == "Yes") 
                Environment.Exit(0);
        }

        private void AfbreekKnop_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Weet je zeker dat je het huidige spel wilt verlaten?", "Spel afbreken",
                                MessageBoxButton.YesNo).ToString() == "Yes")
            {
                _game.Close();
            }
        }

        private void OpslaanKnop_Click(object sender, RoutedEventArgs e)
        {
            _game.Opslaan();

        }

        public async Task<Tuple<Speler, int>> DobbelTask(Speler speler, int gedobbeld)
        {
            _speler = speler;
            _gedobbeld = gedobbeld;

            DobbelKnop.IsEnabled = true;
            GetalLabel.IsEnabled = true;
            SpelerKiezer.IsEnabled = true;
            KnopRand.Visibility = Visibility.Visible;
            GetalLabel.Clear();

            var result = await _wachter.Wait();

            DobbelKnop.IsEnabled = false;
            GetalLabel.IsEnabled = false;
            SpelerKiezer.IsEnabled = false;
            KnopRand.Visibility = Visibility.Hidden;

            return result;
        }
    }
}
