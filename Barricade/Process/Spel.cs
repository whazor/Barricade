using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Barricade.Bot;
using Barricade.Logic;
using System.Linq;
using Barricade.Logic.Exceptions;
using Barricade.Logic.Velden;
using Barricade.Presentation;
using System.Collections;
using Barricade.Utilities;

namespace Barricade.Process
{
	public class Spel
	{
        readonly CountedRandom _random;
	    private const int WachttijdBot = 2000;
	    private readonly Logic.Spel _logicSpel;
        private readonly IView _game;
	    private int _beurt;
	    private readonly Dictionary<Logic.Speler, ISpeler> _spelers;


        public Spel(Logic.Spel logicSpel, IView game)
        {
            _logicSpel = logicSpel;
            _random = _logicSpel.Random;
            _game = game;
            _spelers = new Dictionary<Speler, ISpeler>();
            int i = 1;
            foreach (var speler in logicSpel.Spelers)
            {
                if (i % 2 == 0)
                    _spelers.Add(speler, new Rusher(speler, logicSpel));
                else
                {
                    _spelers.Add(speler, new Vriendelijk(speler, logicSpel));
                }
                i++;
            }
        }

        public Spel(Logic.Spel logicSpel, IView game, ISpeler viewSpeler)
        {
            _logicSpel = logicSpel;
            _random = _logicSpel.Random;
            _game = game;
            _spelers = new Dictionary<Speler, ISpeler>();
            int i = 1;
            foreach (var speler in logicSpel.Spelers)
            {
                if (i % 2 == 0)
                    _spelers.Add(speler, new Rusher(speler, logicSpel));
                else
                {
                    _spelers.Add(speler, new Vriendelijk(speler, logicSpel));
                }
                i++;
            }
            if (viewSpeler != null)
                _spelers[_spelers.First().Key] = viewSpeler;
	    }

	    public virtual async void Start()
        {
	        try
	        {
	            while (true)
	            {
	                var speler = _logicSpel.Spelers[_beurt++%_logicSpel.Spelers.Count];

	                _game.IsAanBeurt = speler;
                    
                    // Rol dobbelsteen
                    int gedobbeld = _random.Next(1, 7);

                    // Cheatfunctionaliteit
                    var cheat = await _spelers[speler].DobbelTask(speler, gedobbeld);
                    // Beurt verplaatsen
                    while (speler != cheat.Item1)
                    {
                        speler = _logicSpel.Spelers[_beurt++ % _logicSpel.Spelers.Count];
                    }
	                gedobbeld = cheat.Item2;

	                await VolgendeBeurt(gedobbeld, speler, _spelers[speler]);
	                await _game.Wacht(WachttijdBot);
	            }
	        }
	        catch (GewonnenException e)
	        {
	            _game.Gewonnen(e.Speler);
	        }
        }

	    /// <summary>
        /// Doorloop alle stappen van een beurt
        /// </summary>
        private async Task VolgendeBeurt(int gedobbeld, Speler speler, ISpeler controller)
        {
            // Geef dobbelwaarde neer
            _game.Gedobbeld = gedobbeld;
            controller.Gedobbeld = gedobbeld;

            // Selecteer alle pionnen die kunnen lopen
            var pionnen = speler.Pionnen.Where(pion => pion.MogelijkeZetten(gedobbeld).Count != 0).ToList();

            // Wanneer er geen zetten mogelijk zijn doorspelen
            if (!pionnen.Any())
            {
                return;
            }
            
            // Laat speler een pion kiezen
            var gekozen = await controller.KiesPion(pionnen, gedobbeld);

            // Markeer alle kiesbare velden
            var mogelijk = gekozen.MogelijkeZetten(gedobbeld);

            // Laat speler een veld kiezen
            var veld = await controller.VerplaatsPion(gekozen, mogelijk);

            // Het spel de zet laten zetten
            gekozen.Oppakken();

            while(true)
            {
                Logic.Barricade barricade;
                try
                {
                    gekozen.Verplaats(veld);
                    return;
                }
                catch (BarricadeVerplaatsException e)
                {
                    barricade = e.Barricade;
                }
                // Barricade verplaatsen
                if (barricade == null) continue;
                var target = await controller.VerplaatsBarricade(barricade, a => a.MagBarricade);
                barricade.Verplaats(target as Veld);
            }
        }
	}
}

