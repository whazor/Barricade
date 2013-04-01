using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Barricade.Logic;
using Barricade.Presentation.Statisch;
using Bos = Barricade.Presentation.Statisch.Bos;
using FinishVeld = Barricade.Presentation.Statisch.FinishVeld;
using Pion = Barricade.Logic.Pion;
using Spel = Barricade.Process.Spel;
using StartVeld = Barricade.Presentation.Statisch.StartVeld;

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

        // Properties voor het oplichten van mogelijkheden
        private Pion _pion;
        private readonly List<IElement> _opgelicht = new List<IElement>();

        // Voor het inladen van het spel (klaar met laden)
        public delegate void ShowableEvent(object sender, RoutedEventArgs e);
        public event ShowableEvent Showable;

        public Game(Data.Loader loader)
        {
            InitializeComponent();

            _logicSpel = loader.Spel;
            _processSpel = new Process.Spel(_logicSpel);

            _statischeLaag = new StatischeLaag(Spelbord, loader.Kaart);
            _processSpel.BeurtWijziging += ProcessSpelOnBeurtWijziging;
            _processSpel.BarricadeVerplaatsing += ProcessSpelOnBarricadeVerplaatsing;

            // Inladen moet later voor het opzoeken van veldposities
            Loaded += LaadDynamischeLaag;
        }

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

            // Geef een event af voor het laadscherm
            if(Showable != null) Showable(sender, e);

            // Start spel
            _processSpel.Start();
        }


        private void ProcessSpelOnBeurtWijziging(Speler speler, int dobbel)
        {
            _dynamischeLaag.OntsteekLicht(speler);
        }

        private UserControl _currentBarricade;
        private Logic.Barricade _barricade;
        private Spel.Neerzetten _zetOp;

        private void ProcessSpelOnBarricadeVerplaatsing(Logic.Barricade barricade, Spel.Neerzetten zetOp)
        {
            _barricade = barricade;
            _zetOp = zetOp;
            _currentBarricade = _dynamischeLaag.Zoek(barricade);
            MouseMove += OnMove;
            foreach (var veld in _statischeLaag.Velden.Where(a => a.Key.MagBarricade))
            {
                var element = (UserControl) veld.Value;
                element.Cursor = Cursors.Hand;
                element.MouseUp += VeldKlikBarricade;
            }
        }

        private void OnMove(object sender, MouseEventArgs args)
        {
            var position = args.GetPosition(DynamischGrid);
            _currentBarricade.Margin = new Thickness(Math.Min(position.X + 20, DynamischGrid.ActualWidth - 50), Math.Min(position.Y + 20, DynamischGrid.ActualHeight - 50), 0, 0);
        }

        private void VeldKlikBarricade(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MouseMove -= OnMove;

            foreach (var veld in _statischeLaag.Velden)
            {
                var element = (UserControl)veld.Value;
                element.Cursor = null;
                element.MouseUp -= VeldKlikBarricade;
            }
            var bestemming = ((IElement)sender).Veld;
            _zetOp(_barricade, bestemming);
        }

        void PionKlik(Dynamisch.Pion icon, Pion item)
        {
            if (item.Speler != _processSpel.AanDeBeurt) return;

            _dynamischeLaag.DoofLicht();
            icon.WisselLicht(true);

            Spelbord.Cursor = Cursors.No;
            DynamischGrid.IsHitTestVisible = false;
            DynamischGrid.Opacity = .7;

            _pion = item;
            var _mogelijkePaden = item.MogelijkeZetten(_processSpel.Plaatsen);
            foreach (var element in _opgelicht)
            {
                element.WisselLicht(false);
            }
            _opgelicht.Clear();
            foreach (var zet in _mogelijkePaden)
            {
                _opgelicht.Add(_statischeLaag.Velden[zet]);
                _statischeLaag.Velden[zet].WisselLicht(true);
                var element = (UserControl) _statischeLaag.Velden[zet];
                element.Cursor = Cursors.Hand;
                element.MouseUp += VeldKlik;
            }
        }

        private void VeldKlik(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var bestemming = ((IElement) sender).Veld;
            Spelbord.Cursor = null;
            DynamischGrid.Opacity = 1;
            DynamischGrid.IsHitTestVisible = true;

            _processSpel.Verplaats(_pion, bestemming);
            
            foreach (var item in _opgelicht)
            {
                item.WisselLicht(false);
                var element = (UserControl)item;
                element.Cursor = null;
                element.MouseUp -= VeldKlik;
            }
            _opgelicht.Clear();
        }
    }
}