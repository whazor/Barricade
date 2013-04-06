using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Barricade.Data;
using Barricade.Logic;
using Barricade.Presentation.Statisch;
using Barricade.Process;
using Barricade.Utilities;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Pion = Barricade.Logic.Pion;
using UserControl = System.Windows.Controls.UserControl;

namespace Barricade.Presentation
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window, ISpeler
    {
        public static MainWindow mainWindow;

        // Logic spel
        private readonly Logic.Spel _logicSpel;
        private readonly Process.Spel _processSpel;

        // De lagen
        private readonly StatischeLaag _statischeLaag;
        private DynamischeLaag _dynamischeLaag;

        // Properties voor het slepen van barricades
        private UserControl _sleepTarget;
        private bool _sleepende;

        // Dit is voor de async methodes
        private readonly Waiter<Pion> _pionCompletion = new Waiter<Pion>();
        private readonly Waiter<IVeld> _veldCompletion = new Waiter<IVeld>();
        private Waiter wachter = new Waiter();
        private int _gedobbeld;
        private Loader _loader;


        // Voor het inladen van het spel (klaar met laden)
        public delegate void ShowableEvent(object sender, RoutedEventArgs e);
        public event ShowableEvent Showable;

        public int Gedobbeld
        {
            get { return _gedobbeld; }
            set
            {
                GetalLabel.Content = value;
                _gedobbeld = value;
            }
        }

        public Speler AanDeBeurt { get; set; }

        /// <summary>
        /// Maak een spelview aan.
        /// </summary>
        /// <param name="loader">desbetreffend spel</param>
        public Game(Loader loader, MainWindow main)
        {
            InitializeComponent();

            mainWindow = main;

            _loader = loader;
            _logicSpel = loader.Spel;
            _processSpel = new Process.Spel(_logicSpel, this);

            _statischeLaag = new StatischeLaag(Spelbord, loader.Kaart);

            // Inladen moet later voor het opzoeken van veldposities
            Loaded += LaadDynamischeLaag;
            MouseMove += OnMove;

            Closing += OnClosing;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            mainWindow.Show();
        }

        /// <summary>
        /// Omdat de dynamische laag relatief geposioneerd is moet dit in een later stadium gebeuren.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LaadDynamischeLaag(object sender, RoutedEventArgs e)
        {
            // Niet dubbel laden
            Loaded -= LaadDynamischeLaag;
            // Zoek alle poinnen op
            var pionnen = new List<Pion>(_logicSpel.Spelers.Select(speler => speler.Pionnen).SelectMany(i => i));
            // Zoek alle barricades op
            var barricades = (
                    _statischeLaag.Velden.Keys.Select(veld => veld as Veld)
                                 .Where(veld => veld != null && veld.Barricade != null)
                                 .Select(veld => new Tuple<IVeld, Logic.Barricade>(veld, veld.Barricade)));

            // Maak de dynamische laag aan en teken alles
            _dynamischeLaag = new DynamischeLaag(DynamischGrid, Houder, _statischeLaag.Velden);
            _dynamischeLaag.TekenPionnen(pionnen);
            _dynamischeLaag.TekenBarricades(barricades.ToList());

            // Bij een pion klik wordt er een actie uitgevoerd
            _dynamischeLaag.PionKlik += PionKlik;
            _statischeLaag.VeldKlik += VeldKlik;

            // Geef een event af voor het laadscherm
            if(Showable != null) Showable(sender, e);

            // Start spel
            _processSpel.Start();
        }

        /// <summary>
        /// Licht bepaalde poinnen op
        /// </summary>
        /// <param name="pionnen">de desbetreffende poinnen</param>
        /// <param name="status">aan of uit</param>
        public void Highlight(IEnumerable<Logic.Pion> pionnen, bool status)
        {
            _dynamischeLaag.Highlight(pionnen, status);
        }

        /// <summary>
        /// Licht bepaalde velden op
        /// </summary>
        /// <param name="velden">de desbetreffende velden</param>
        /// <param name="status">aan of uit</param>
        public void Highlight(IEnumerable<Logic.IVeld> velden, bool status)
        {
            _statischeLaag.Highlight(velden, status);
        }

        /// <summary>
        /// Klem een barricade aan de muis vast (of los)
        /// </summary>
        /// <param name="barricade">desbetreffende barricade</param>
        /// <param name="b">vast of los</param>
        public void Klem(Logic.Barricade barricade, bool b)
        {
            _sleepTarget = _dynamischeLaag.Zoek(barricade);
            _sleepende = b;
        }

        /// <summary>
        /// Methode die een usercontrol aan een muis koppelt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMove(object sender, MouseEventArgs args)
        {
            if (!_sleepende) return;
            var position = args.GetPosition(DynamischGrid);
            _sleepTarget.Margin = new Thickness(Math.Min(position.X + 20, DynamischGrid.ActualWidth - 50), Math.Min(position.Y + 20, DynamischGrid.ActualHeight - 50), 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void PionKlik(Pion item)
        {
            _pionCompletion.Return(item);
        }

        private void VeldKlik(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var bestemming = ((IElement) sender).Veld;
            _veldCompletion.Return(bestemming);
        }

        public async Task<Pion> KiesPion(ICollection<Pion> pionnen)
        {
            while (true)
            {
                var pion = await _pionCompletion.Wait();
                if (pionnen.Contains(pion))
                    return pion;
            }
        }

        public async Task<IVeld> VerplaatsBarricade(Func<IVeld, bool> mogelijk)
        {
            DynamischGrid.IsHitTestVisible = false;
            while (true)
            {
                var veld = await _veldCompletion.Wait();

                if (!mogelijk(veld))
                {    
                    continue;
                }

                DynamischGrid.IsHitTestVisible = true;
                return veld;
            }
        }

        public async Task<IVeld> VerplaatsPion(Pion gekozen, ICollection<IVeld> mogelijk)
        {
            DynamischGrid.IsHitTestVisible = false;
            while (true)
            {
                var veld = await _veldCompletion.Wait();

                if (!mogelijk.Contains(veld))
                {
                    continue;
                }

                DynamischGrid.IsHitTestVisible = true;
                return veld;
            }
        }

        public async Task DobbelTask()
        {
            await wachter.Wait();
        }

        public async Task Wacht(int p)
        {
            Waiter wachter = new Waiter();
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += (sender, args) => wachter.Return();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(p);
            dispatcherTimer.Start();

            await wachter.Wait();
            dispatcherTimer.Stop();
        }

        private void DobbelKnop_Click(object sender, RoutedEventArgs e)
        {
            wachter.Return();
        }

        private void AfsluitKnop_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Weet je zeker dat je het huidige spel wilt verlaten?", "Spel afbreken",
                                MessageBoxButton.YesNo).ToString() == "Yes") Environment.Exit(0);
        }

        private void AfbreekKnop_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show("Weet je zeker dat je het huidige spel wilt verlaten?", "Spel afbreken",
                                MessageBoxButton.YesNo).ToString() == "Yes")
            {
                Close();
                mainWindow.Show();
            }
        }

        private void OpslaanKnop_Click(object sender, RoutedEventArgs e)
        {
            String huidigeSpel = new Saver(_loader.ToArray()).Output();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Barricade save games (*.bar)|*.bar";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                try
                {
                    var sw = new StreamWriter(dialog.FileName);
                    sw.Write(huidigeSpel);
                    sw.Close();
                }
                catch (IOException)
                {
                    MessageBox.Show("Kan bestand niet wegschrijven", "Fout!", MessageBoxButton.OK);
                }
        }

    }
}