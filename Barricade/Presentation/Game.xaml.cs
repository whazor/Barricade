using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Barricade.Logic;
using Barricade.Presentation.Statisch;
using Pion = Barricade.Logic.Pion;

namespace Barricade.Presentation
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
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
        private TaskCompletionSource<Pion> _pionCompletion = new TaskCompletionSource<Pion>();
        private TaskCompletionSource<IVeld> _veldCompletion = new TaskCompletionSource<IVeld>();

        // Voor het inladen van het spel (klaar met laden)
        public delegate void ShowableEvent(object sender, RoutedEventArgs e);
        public event ShowableEvent Showable;

        /// <summary>
        /// Maak een spelview aan.
        /// </summary>
        /// <param name="loader">desbetreffend spel</param>
        public Game(Data.Loader loader)
        {
            InitializeComponent();

            _logicSpel = loader.Spel;
            _processSpel = new Process.Spel(_logicSpel, this);

            _statischeLaag = new StatischeLaag(Spelbord, loader.Kaart);

            // Inladen moet later voor het opzoeken van veldposities
            Loaded += LaadDynamischeLaag;
            MouseMove += OnMove;
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
            _pionCompletion.TrySetResult(item);
        }

        private void VeldKlik(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var bestemming = ((IElement) sender).Veld;
            _veldCompletion.TrySetResult(bestemming);
        }

        public async Task<Pion> KiesPion(List<Pion> mogelijk)
        {
            return await KiesPion(mogelijk.Contains);
        }

        public async Task<Pion> KiesPion(Func<Pion, bool> mogelijk)
        {   
            while (true)
            {
                var pion = await _pionCompletion.Task;
                _pionCompletion = new TaskCompletionSource<Pion>();
                if (mogelijk(pion))
                    return pion;
            }
        }

        public async Task<IVeld> KiesVeld(Func<IVeld, bool> mogelijk)
        {
            DynamischGrid.IsHitTestVisible = false;
            while (true)
            {
                var veld = await _veldCompletion.Task;
                _veldCompletion = new TaskCompletionSource<IVeld>();

                if (!mogelijk(veld))
                {    
                    continue;
                }

                DynamischGrid.IsHitTestVisible = true;
                return veld;
            }
        }

        public async Task<IVeld> KiesVeld(List<IVeld> mogelijk)
        {
            return await KiesVeld(mogelijk.Contains);
        }
    }
}