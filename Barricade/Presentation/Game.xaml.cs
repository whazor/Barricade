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
using Barricade.Logic.Velden;
using Barricade.Presentation.Statisch;
using Barricade.Process;
using Barricade.Utilities;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Pion = Barricade.Logic.Pion;
using UserControl = System.Windows.Controls.UserControl;

namespace Barricade.Presentation
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window, ISpeler, IView
    {
        public static MainWindow MainWindow;
        private readonly UserInterface _userControls;

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
        private readonly Loader _loader;


        // Voor het inladen van het spel (klaar met laden)
        public delegate void ShowableEvent(object sender, RoutedEventArgs e);
        public event ShowableEvent Showable;

        public int Gedobbeld
        {
            get { return _userControls.Gedobbeld; }
            set
            {
                _userControls.Gedobbeld = value;
            }
        }

        public Speler IsAanBeurt { get { return _userControls.IsAanBeurt; } set { _userControls.IsAanBeurt = value; } }

        public void Gewonnen(Speler speler)
        {
            MessageBox.Show("Speler " + speler.Name + " heeft gewonnen!", "Er is een winnaar!", MessageBoxButton.OK,
                            MessageBoxImage.Information);

        }

        public Speler AanDeBeurt { get; set; }

        /// <summary>
        /// Maak een spelview aan.
        /// </summary>
        /// <param name="loader">desbetreffend spel</param>
        /// <param name="main"></param>
        public Game(Loader loader, MainWindow main)
        {
            InitializeComponent();

            MainWindow = main;

            _loader = loader;
            _logicSpel = loader.Spel;
            _processSpel = new Process.Spel(_logicSpel, this, this);

            _statischeLaag = new StatischeLaag(Spelbord, loader.Kaart);

            // Inladen moet later voor het opzoeken van veldposities
            Loaded += LaadDynamischeLaag;
            MouseMove += OnMove;

            Closing += OnClosing;

            _userControls = new UserInterface(this, _logicSpel.Spelers) {HorizontalAlignment = HorizontalAlignment.Left };
            GameHolder.Children.Add(_userControls);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            MainWindow.Show();
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
            _dynamischeLaag.PionKlik += _pionCompletion.Return;
            _statischeLaag.VeldKlik += _veldCompletion.Return;

            // Geef een event af voor het laadscherm
            if(Showable != null) Showable(sender, e);

            // Start spel
            _processSpel.Start();
        }

        /// <summary>
        /// Klem een barricade aan de muis vast (of los)
        /// </summary>
        /// <param name="barricade">desbetreffende barricade</param>
        /// <param name="b">vast of los</param>
        private void Klem(Logic.Barricade barricade, bool b)
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

        public async Task<Pion> KiesPion(ICollection<Pion> pionnen, int gedobbeld)
        {
            _dynamischeLaag.Highlight(pionnen, true);
            while (true)
            {
                var pion = await _pionCompletion.Wait();
                if (pionnen.Contains(pion))
                {
                    _dynamischeLaag.Highlight(pionnen, false);
                    return pion;
                }
            }
        }

        public async Task<IVeld> VerplaatsBarricade(Logic.Barricade barricade, Func<IVeld, bool> mogelijk)
        {
            Klem(barricade, true);
            DynamischGrid.IsHitTestVisible = false;
            while (true)
            {
                var veld = await _veldCompletion.Wait();

                if (!mogelijk(veld))
                {    
                    continue;
                }

                DynamischGrid.IsHitTestVisible = true;
                Klem(barricade, false);
                return veld;
            }
        }

        public async Task<IVeld> VerplaatsPion(Pion gekozen, ICollection<IVeld> mogelijk)
        {
            _dynamischeLaag.Highlight(new[] { gekozen }, true);
            _statischeLaag.Highlight(mogelijk, true);
            DynamischGrid.IsHitTestVisible = false;
            while (true)
            {
                var veld = await _veldCompletion.Wait();

                if (!mogelijk.Contains(veld))
                {
                    continue;
                }

                DynamischGrid.IsHitTestVisible = true;
                _dynamischeLaag.Highlight(new[] { gekozen }, false);
                _statischeLaag.Highlight(mogelijk, false);
                return veld;
            }
        }

        public Task<Tuple<Speler, int>> DobbelTask(Speler speler, int gedobbeld)
        {
            return _userControls.DobbelTask(speler, gedobbeld);
        }

        public async Task Wacht(int p)
        {
            var wachter = new Waiter();
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += (sender, args) => wachter.Return();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(p);
            dispatcherTimer.Start();

            await wachter.Wait();
            dispatcherTimer.Stop();
        }


        public void Opslaan()
        {
            String huidigeSpel = new Saver(_logicSpel, _loader.ToArray()).Output();
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