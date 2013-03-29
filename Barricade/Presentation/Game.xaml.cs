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
        private Dictionary<IVeld, List<IVeld>> _mogelijkePaden = new Dictionary<IVeld, List<IVeld>>();

        public Game(Data.Loader loader)
        {
            InitializeComponent();

            _logicSpel = loader.Spel;
            _processSpel = new Process.Spel(_logicSpel);
            

            _statischeLaag = new StatischeLaag(Spelbord, loader.Kaart);
            Loaded += LaadDynamischeLaag;
        }

        public delegate void ShowableEvent(object sender, RoutedEventArgs e);
        public event ShowableEvent Showable;

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
            _dynamischeLaag.PionKlik += VeldKlik;

            // Geef een event af voor het laadscherm
            if(Showable != null) Showable(sender, e);
        }

        void VeldKlik(Dynamisch.Pion icon, Pion item)
        {
            Spelbord.Cursor = Cursors.No;
            DynamischGrid.IsHitTestVisible = false;
            DynamischGrid.Opacity = .7;

            _pion = item;
            _mogelijkePaden = item.MogelijkeZetten(4).ToDictionary(list => list.First());
            foreach (var element in _opgelicht)
            {
                element.WisselLicht(false);
            }
            _opgelicht.Clear();
            foreach (var zet in _mogelijkePaden.Keys)
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

            _pion.Verplaats(bestemming);

            _dynamischeLaag.Beweeg(_pion, _mogelijkePaden[bestemming]);
            _mogelijkePaden.Clear();
            
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