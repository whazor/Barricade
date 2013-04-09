using System;
using System.Collections.Generic;
using System.Linq;
using Barricade.Logic;
using Barricade.Logic.Velden;
using Barricade.Utilities;

namespace Barricade.Data
{

	public class Saver
	{
	    #region Properties & variabelen

	    private readonly Spel _spel;
	    private readonly IVeld[,] _points;
        private Dictionary<Startveld, int> _startVelden;
        private int _height;
        private int _width;
        private int _uitzonderingCount;
        private string _uitzonderingen;
	    #endregion

	    #region Constructors

	    public Saver(Spel spel, IVeld[,] points)
	    {
	        _spel = spel;
	        _points = points;
	    }

	    public Saver(Loader loader)
	    {
	        _spel = loader.Spel;
	        _points = loader.Kaart;
	    }

	    #endregion

	    #region Publieke output methodes

        /// <summary>
        /// Geef het level weer, met uitzonderingen.
        /// </summary>
        /// <returns>Het level aan elkaar geplakt</returns>
	    public String Output()
	    {
	        return Output(true);
	    }

        /// <summary>
        /// Geef het level weer.
        /// </summary>
        /// <param name="metUitzonderingen">Wel of niet spelers/bossen meegeven</param>
        /// <returns>Het level aan elkaar geplakt</returns>
	    public String Output(bool metUitzonderingen)
	    {
            // Reset waarden
            _height = _points.GetLength(0);   
            _width = _points.GetLength(1);
            _uitzonderingCount = 0;
            _uitzonderingen = "";

            // Alle startvelden een getal geven
	        _startVelden = new Dictionary<Startveld, int>();
	        foreach (var start in _spel.Spelers.Select(speler => speler.Startveld))
	        {
	            var i = ++_uitzonderingCount;

	            _startVelden[start] = i;
	            _uitzonderingen += "*" + i + ":START,";
	            _uitzonderingen += "" + start.Speler.Name + start.Pionnen.Count;
	            _uitzonderingen += "\r\n";
	        }
            
            // Teken een lege kaart
	        var stringmap = new string[_height*2 - 1];
	        for (var i = 0; i < stringmap.Length; i++)
	        {
	            stringmap[i] += "      ";
	            for (var j = 0; j < _width; j++)
	            {
	                stringmap[i] += "    ";
	            }
	        }

            // Zet alle rij opties neer
            TekenRijOpties(stringmap);

            // Maak alle vakjes aan een krijg uitzonderingen terug.
            TekenVakjes(stringmap);

            // Seed opslaan, werkt nog niet goed:
            _uitzonderingen = "*SEED:" + _spel.Random.Seed + "," + _spel.Random.Counter + "\r\n" + _uitzonderingen;

            // Teken alle connecties op de kaart.
	        TekenLijntjes(stringmap);

            // Geef alles vastgeplakt aan elkaar terug
	        return String.Join("\r\n", stringmap) + "\r\n\r\n" + (metUitzonderingen ? _uitzonderingen : "");
	    }

	    #endregion

	    #region Methodes voor het tekenen
	    /// <summary>
        /// Zet alle vakjes neer op de meegegeven bestemming.
        /// </summary>
        /// <param name="stringmap">bestemming van vakjes</param>
	    private void TekenVakjes(string[] stringmap)
	    {
	        for (var i = 0; i < _height; i++)
	        {
	            var rij = i*2;

	            for (var j = 0; j < _width; j++)
	            {
	                if (_points[i, j] == null)
	                    continue;
	                var pos = 6 + j*4;

	                var inhoud = " ";
	                if (_points[i, j].Pionnen.Count > 0)
	                {
	                    inhoud = ZetInhoudOm(_points[i, j]);
	                }
	                var veld = ZetVeldOm(_points[i, j], inhoud);

	                stringmap[rij] = stringmap[rij].Replace(pos, veld);
	            }
	        }
	    }

	    /// <summary>
	    /// Zet op elke rij neer welke vakjes er moeten komen
	    /// </summary>
        /// <param name="stringmap">bestemming</param>
	    private void TekenRijOpties(string[] stringmap)
        {
	        for (var i = 0; i < _height; i++)
	        {
	            var rij = i*2;
	            var isDorp = false;
	            var barricadeVrij = false;
	            // Haal de rijopties op
	            for (var veld = 0; veld < _width; veld++)
	            {
	                if (_points[i, veld] == null) continue;

	                isDorp = isDorp || _points[i, veld].IsDorp;

	                if (_points[i, veld] is Veld)
	                    barricadeVrij = barricadeVrij || (_points[i, veld] as Veld).IsBeschermd;
	            }

	            // Teken de rijopties
	            var hoeveelsteOptie = 0;
	            if (barricadeVrij)
	                stringmap[rij] = stringmap[rij].Replace(hoeveelsteOptie++, "-");
	            if (isDorp)
	                stringmap[rij] = stringmap[rij].Replace(hoeveelsteOptie, "D");
	        }
        }

        /// <summary>
        /// Alle connecties tussen de velden tekent.
        /// </summary>
        /// <param name="stringmap">bestemming</param>
	    private void TekenLijntjes(string[] stringmap)
	    {
            // Horizontale connecties
	        for (var i = 0; i < _height; i++)
	        {
	            var rij = i * 2;
	            for (var j = 0; j < _width; j++)
	            {
	                if (_points[i, j] == null)
	                    continue;

	                for (var k = j + 1; k < _width; k++)
	                {
	                    if (_points[i, k] == null)
	                        continue;

	                    if (_points[i, j].Buren.Contains(_points[i, k]))
	                    {
	                        var pos1 = j * 4 + 6 + 3;
	                        var pos2 = k * 4 + 6;

	                        for (int l = pos1; l < pos2; l++)
	                        {
	                            stringmap[rij] = stringmap[rij].Replace(l, "-");
	                        }
	                    }
	                    break;
	                }
	            }
	        }
            // Verticale connecties
	        for (var j = 0; j < _width; j++)
	        {
	            var pos = j * 4 + 1 + 6;
	            for (var i = 0; i < _height; i++)
	            {
	                if (_points[i, j] == null)
	                    continue;

	                for (var k = i + 1; k < _height; k++)
	                {
	                    if (_points[k, j] == null)
	                        continue;

	                    if (_points[i, j].Buren.Contains(_points[k, j]))
	                    {
	                        var pos1 = i * 2 + 1; // j * 4 + 6 + 3;
	                        var pos2 = k * 2; //k * 4 + 6;

	                        for (int l = pos1; l < pos2; l++)
	                        {
	                            stringmap[l] = stringmap[l].Replace(pos, "|");
	                        }
	                    }
	                    break;
	                }
	            }
	        }
	    }

	    #endregion

	    #region Methodes voor objecten om te zetten
        /// <summary>
        /// Zet een veld om naar tekst
        /// </summary>
        /// <param name="iveld">het veld</param>
        /// <param name="inhoud">inhoud van veld</param>
        /// <returns>een textueel veld</returns>
	    private string ZetVeldOm(IVeld iveld, string inhoud)
	    {
	        string veld;
            if (iveld is Rustveld)
	        {
	            veld = "{" + inhoud + "}";
	        }
            else if (iveld is Finishveld)
	        {
	            veld = "< >";
	        }
            else if (iveld is Startveld || iveld is Bos)
	        {
                if (iveld is Startveld)
	            {
                    veld = "<" + _startVelden[(iveld as Startveld)] + ">";
	            }
	            else
	            {
	                veld = "<" + (++_uitzonderingCount) + ">";
                    _uitzonderingen += "*" + _uitzonderingCount + ":BOS,";
	                // voeg spelers toe
                    _uitzonderingen = iveld.Pionnen.Aggregate(_uitzonderingen,
	                                                               (current, point) =>
	                                                               current + (point.Speler.Name + ""));
                    _uitzonderingen += "\r\n";
	            }
	        }
            else if (iveld is Veld)
	        {
                var loopveld = iveld as Veld;
	            if (loopveld.Barricade != null)
	            {
	                inhoud = "*";
	            }
	            veld = loopveld.StandaardBarricade ? "[" + inhoud + "]" : "(" + inhoud + ")";
	        }
	        else
	        {
	            throw new Exception("Ik ken dit veld niet.");
	        }
	        return veld;
	    }

        /// <summary>
        /// Zet de inhoud van een veld om tot tekst
        /// </summary>
        /// <param name="veld">veld</param>
        /// <returns>textuele inhoud</returns>
	    private static string ZetInhoudOm(IVeld veld)
	    {
	        var inhoud = veld.Pionnen.First().Speler.Name + "";
	        return inhoud;
	    }

	    #endregion
	}
}

