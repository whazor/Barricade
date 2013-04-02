using System;
using System.Threading;
using System.Windows.Forms;
using Barricade.Logic;
using System.Linq;
using Barricade.Presentation;

namespace Barricade.Process
{
	public class Spel
	{
	    private readonly Logic.Spel _logicSpel;
	    private readonly Game _game;
	    private int _beurt;

	    public Spel(Logic.Spel logicSpel, Game game)
	    {
	        _logicSpel = logicSpel;
	        _game = game;
	    }

	    public virtual void Start()
        {
            VolgendeBeurt();
        }

        readonly Random _random = new Random();
        
        /// <summary>
        /// Doorloop alle stappen van een beurt
        /// </summary>
        private async void VolgendeBeurt()
        {
            var speler = _logicSpel.Spelers[_beurt++ % _logicSpel.Spelers.Count];

            //TODO: speler markeren

            // Rol dobbelsteen
            int gedobbeld = _random.Next(1, 7);

            //TODO: dobbelsteen weergeven

            // Selecteer alle pionnen die kunnen lopen
            var pionnen = speler.Pionnen.Where(pion => pion.MogelijkeZetten(gedobbeld).Count != 0).ToList();

            // Wanneer er geen zetten mogelijk zijn doorspelen
            if (!pionnen.Any())
            {
                //TODO: kijken of deze slaapwaarde goed is
                // Tijdelijk de dobbelsteenwaarde laten zien
                Thread.Sleep(4000);
                VolgendeBeurt();
                return;
            }
            
            // Markeer alle kiesbare pionnen
            _game.Highlight(pionnen, true);

            // Laat speler een pion kiezen
            var gekozen = await _game.KiesPion(pionnen);

            // Markeer nu alleen de gekozen pion
            _game.Highlight(pionnen, false);
            _game.Highlight(new[]{gekozen}, true);

            // Markeer alle kiesbare velden
            var mogelijk = gekozen.MogelijkeZetten(gedobbeld);
            _game.Highlight(mogelijk, true);

            // Laat speler een veld kiezen
            var veld = await _game.KiesVeld(mogelijk);

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
                    VolgendeBeurt();
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
                    var target = await _game.KiesVeld(a => a.MagBarricade);
                    _game.Klem(barricade, false);

                    barricade.Verplaats(target as Veld);
                }
            }
        }
	}
}

