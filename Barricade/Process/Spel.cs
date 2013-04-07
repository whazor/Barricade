using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Barricade.Bot;
using Barricade.Logic;
using System.Linq;
using Barricade.Presentation;
using System.Collections;

namespace Barricade.Process
{
	public class Spel
	{
	    private const int _wachttijdBot = 2000;
	    private readonly Logic.Spel _logicSpel;
	    private readonly Game _game;
	    private int _beurt;
	    private readonly Dictionary<Logic.Speler, ISpeler> spelers;

	    public Spel(Logic.Spel logicSpel, Game game)
	    {
	        _logicSpel = logicSpel;
            _random = _logicSpel.Random;
	        _game = game;
            spelers = new Dictionary<Speler, ISpeler>();
	        int i = 1;
	        foreach (var speler in logicSpel.Spelers)
	        {
                if(i % 2 == 0)
	                spelers.Add(speler, new Rusher(speler, logicSpel));
                else
                {
                    spelers.Add(speler, new Vriendelijk(speler, logicSpel));
                }
	            i++;
	        }
	        //spelers[spelers.First().Key] = _game;
	    }

	    public virtual async void Start()
        {
	        while (true)
	        {
	            var speler = _logicSpel.Spelers[_beurt++%_logicSpel.Spelers.Count];
                //TODO BUG!!!
	            await VolgendeBeurt(speler, spelers[speler]);
                await _game.Wacht(_wachttijdBot);
	        }
        }

        readonly Random _random;
        
        /// <summary>
        /// Doorloop alle stappen van een beurt
        /// </summary>
        private async Task VolgendeBeurt(Speler speler, ISpeler controller)
        {

            //TODO: speler markeren

            // Rol dobbelsteen
            int gedobbeld = _random.Next(1, 7);

            controller.Gedobbeld = gedobbeld;
            //TODO: dobbelsteen weergeven

            // Selecteer alle pionnen die kunnen lopen
            var pionnen = speler.Pionnen.Where(pion => pion.MogelijkeZetten(gedobbeld).Count != 0).ToList();

            // Wanneer er geen zetten mogelijk zijn doorspelen
            if (!pionnen.Any())
            {
                //TODO: kijken of deze slaapwaarde goed is
                // Tijdelijk de dobbelsteenwaarde laten zien
                //Thread.Sleep(4000);
                //_game.wait; //voorlater
                return;
            }
            
            // Markeer alle kiesbare pionnen
            _game.Highlight(pionnen, true);

            // Laat speler een pion kiezen
            var gekozen = await controller.KiesPion(pionnen);

            // Markeer nu alleen de gekozen pion
            _game.Highlight(pionnen, false);
            _game.Highlight(new[]{gekozen}, true);

            // Markeer alle kiesbare velden
            var mogelijk = gekozen.MogelijkeZetten(gedobbeld);
            _game.Highlight(mogelijk, true);

            // Laat speler een veld kiezen
            var veld = await controller.VerplaatsPion(gekozen, mogelijk);

            // Markeer nu niks meer
            _game.Highlight(mogelijk, false);
            _game.Highlight(new[] { gekozen }, false);

            // Het spel de zet laten zetten
            while(true)
            {
                Logic.Barricade barricade;
                try
                {
                    gekozen.Verplaats(veld);
                    return;
                }
                catch (Logic.Exceptions.GewonnenException e)
                {
                    //TODO: Naam aan speler hangen en spel afsluiten.
                    MessageBox.Show("Speler "+e.Speler.Name+" heeft gewonnen!", "Er is een winnaar!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                catch (Logic.Exceptions.BarricadeVerplaatsException e)
                {
                    barricade = e.Barricade;
                }
                // Barricade verplaatsen
                if (barricade != null)
                {
                    _game.Klem(barricade, true);
                    var target = await controller.VerplaatsBarricade(a => a.MagBarricade);
                    _game.Klem(barricade, false);

                    barricade.Verplaats(target as Veld);
                }
            }
        }
	}
}

