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
    public partial class Game : Page
    {
        private readonly Spel _spel;

        private readonly StatischeLaag _statischeLaag;
        private DynamischeLaag _dynamischeLaag;

        private Pion _pion;
        private readonly List<IElement> _opgelicht = new List<IElement>();
        private Dictionary<IVeld, List<IVeld>> _mogelijkePaden = new Dictionary<IVeld, List<IVeld>>();

        public Game(Data.Loader loader)
        {
            InitializeComponent();
            _spel = loader.Spel;
            _statischeLaag = new StatischeLaag(Spelbord, loader.Kaart);
            Loaded += Game_Loaded;
        }

        private void Game_Loaded(object sender, RoutedEventArgs e)
        {
            _dynamischeLaag = new DynamischeLaag(DynamischGrid, Houder, _statischeLaag.Velden);
            var pionnen = new List<Pion>(_spel.Spelers.Select(speler => speler.Pionnen).SelectMany(i => i));
            var barricades = (
                    _statischeLaag.Velden.Keys.Select(veld => veld as Veld)
                                 .Where(veld => veld != null && veld.Barricade != null)
                                 .Select(veld => new Tuple<IVeld, Logic.Barricade>(veld, veld.Barricade)));
            _dynamischeLaag.TekenPionnen(pionnen);
            _dynamischeLaag.TekenBarricades(barricades.ToList());
            _dynamischeLaag.PionClick += _dynamischeLaag_PionClick;
        }

        void _dynamischeLaag_PionClick(Pion item)
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
                element.MouseUp += OnMouseUp;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Spelbord.Cursor = null;
            DynamischGrid.Opacity = 1;
            DynamischGrid.IsHitTestVisible = true;

            _dynamischeLaag.Beweeg(_pion, _mogelijkePaden[((IElement) sender).Veld]);
            
            foreach (var item in _opgelicht)
            {
                item.WisselLicht(false);
                var element = (UserControl)item;
                element.Cursor = null;
                element.MouseUp -= OnMouseUp;
            }
        }
    }
}