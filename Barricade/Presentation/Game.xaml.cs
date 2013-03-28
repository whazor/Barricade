using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        }
    }
}