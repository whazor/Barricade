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
	    private const int _wachttijdBot = 2000;
	    private readonly Logic.Spel _logicSpel;
        private readonly IView _game;
	    private int _beurt;
	    private readonly Dictionary<Logic.Speler, ISpeler> spelers;


        public Spel(Logic.Spel logicSpel, IView game)
        {
            _logicSpel = logicSpel;
            _random = _logicSpel.Random;
            _game = game;
            spelers = new Dictionary<Speler, ISpeler>();
            int i = 1;
            foreach (var speler in logicSpel.Spelers)
            {
                if (i % 2 == 0)
                    spelers.Add(speler, new Rusher(speler, logicSpel));
                else
                {
                    spelers.Add(speler, new Vriendelijk(speler, logicSpel));
                }
                i++;
            }
        }

        public Spel(Logic.Spel logicSpel, IView game, ISpeler viewSpeler)
        {
            _logicSpel = logicSpel;
            _random = _logicSpel.Random;
            _game = game;
            spelers = new Dictionary<Speler, ISpeler>();
            int i = 1;
            foreach (var speler in logicSpel.Spelers)
            {
                if (i % 2 == 0)
                    spelers.Add(speler, new Rusher(speler, logicSpel));
                else
                {
                    spelers.Add(speler, new Vriendelijk(speler, logicSpel));
                }
                i++;
            }
            if (viewSpeler != null)
                spelers[spelers.First().Key] = viewSpeler;
	    }

	    public virtual async void Start()
        {
	        try
	        {
	            while (true)
	            {
	                var speler = _logicSpel.Spelers[_beurt++%_logicSpel.Spelers.Count];
	                await VolgendeBeurt(speler, spelers[speler]);
	                await _game.Wacht(_wachttijdBot);
	            }
	        }
	        catch (GewonnenException e)
	        {
                MessageBox.Show("Speler " + e.Speler.Name + " heeft gewonnen!", "Er is een winnaar!", MessageBoxButtons.OK, MessageBoxIcon.Information);
	        }
        }

	    /// <summary>
        /// Doorloop alle stappen van een beurt
        /// </summary>
        private async Task VolgendeBeurt(Speler speler, ISpeler controller)
        {
            //TODO: speler markeren

            // Rol dobbelsteen
            int dobbelwaarde = await controller.DobbelTask();
	        int gedobbeld = dobbelwaarde > 0 ? dobbelwaarde : _random.Next(1, 7);

            // Geef dobbelwaarde neer
            _game.Gedobbeld = gedobbeld;

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

