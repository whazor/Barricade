using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Barricade.Bot;
using Barricade.Data;
using Barricade.Logic;
using Barricade.Presentation;
using Barricade.Utilities;
using Barricade.Process;

namespace Barricade
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Loader loader;
        public MainWindow()
        {
            InitializeComponent();
            MaxWidth = SystemParameters.WorkArea.Width;
            MaxHeight = SystemParameters.WorkArea.Height;

            Closing += OnClosing;
            LevelPicker.ItemsSource = Levels.Lijst();
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Environment.Exit(0);
        }

        private void SpeelButton_Click(object sender, RoutedEventArgs e)
        {
            var spelers = new Dictionary<Speler, ISpeler>();

            foreach (Speler speler in SpelersChooser.Items)
            {
                var item = SpelersChooser.ItemContainerGenerator.ContainerFromItem(speler);

                var combo = item.FindVisualChild<System.Windows.Controls.ComboBox>();

                Debug.Assert(combo != null, "combo != null");

                switch (((ComboBoxItem) combo.SelectedItem).Content.ToString())
                {
                    case "Handmatig":
                        spelers.Add(speler, null);
                        break;
                    case "Rusher":
                        spelers.Add(speler, new Rusher(speler, loader.Spel));
                        break;
                    case "Vriendelijk":
                        spelers.Add(speler, new Vriendelijk(speler, loader.Spel));
                        break;
                    case "Willekeurig":
                        spelers.Add(speler, new Willekeurig(speler, loader.Spel));
                        break;
                }
            }

            var game = new Game(loader, spelers, this);
            game.Show();
        }

        private void Afsluiten_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog {Filter = "Barricade save games (*.bar)|*.bar"};

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            var bestand = new System.IO.StreamReader(dialog.FileName).ReadToEnd();
            {
                loader = new Loader(bestand);
                SpelersChooser.ItemsSource = loader.Spel.Spelers;
                SpeelButton.IsEnabled = true;
            }
        }

        private void LevelPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loader = new Loader(Levels.Open(LevelPicker.SelectedItem as String));
            SpelersChooser.ItemsSource = loader.Spel.Spelers;
            SpeelButton.IsEnabled = true;
        }

    }
}
