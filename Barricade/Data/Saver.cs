using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barricade.Logic;
using Barricade.Logic.Velden;

namespace Barricade.Data
{

	public class Saver
	{
	    private readonly Spel _spel;
	    private readonly IVeld[,] _points;

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

	    public String Output(bool metUitzonderingen)
        {
            var result = "";
            var height = _points.GetLength(0);
            var width = _points.GetLength(1);

            var uitzonderingCount = 0;
            var uitzondering = "*SEED:"+_spel.Random.Seed+","+_spel.Random.Counter+"\r\n";
	        var startVelden = new Dictionary<Startveld, int>();
            foreach (var start in _spel.Spelers.Select(speler => speler.Startveld))
            {
                var i = ++uitzonderingCount;

                startVelden[start] = i;
                uitzondering += "*" + i + ":START,";
                uitzondering += "" + start.Speler.Name + start.Pionnen.Count;
                uitzondering += "\r\n";
            }


            for (var i = 0; i < height; i++)
            {
                var isDorp = false;
                var barricadeVrij = false;

                for (var j = 0; j < width; j++)
                {
                    if (_points[i, j] == null) continue;

                    isDorp = isDorp || _points[i, j].IsDorp;

                    if(_points[i,j] is Veld)
                        barricadeVrij = barricadeVrij || (_points[i, j] as Veld).IsBeschermd;
                }

                var spaties = 4;

                if (barricadeVrij)
                {
                    result += "-";
                    spaties--;
                }
                if (isDorp)
                {
                    result += "D";
                    spaties--;
                }

                for (var j = 0; j < spaties; j++)
                {
                    result += " ";
                }

                for (var j = 0; j < width; j++)
                {
                    if (_points[i, j] == null)
                    {
                        result += "    ";
                    }
                    else
                    {
                        var inhoud = " ";
                        if (_points[i, j].Pionnen.Count > 0)
                        {
                            inhoud = TekenInhoud(_points[i, j]);
                        }

                        if (_points[i, j] is Rustveld)
                        {
                            result += "{"+inhoud+"}";
                        }
                        else if (_points[i, j] is Finishveld)
                        {
                            result += "< >";
                        }
                        else if (_points[i, j] is Startveld || _points[i, j] is Bos)
                        {
                            
                            if (_points[i, j] is Startveld)
                            {
                                result += "<" + startVelden[(_points[i, j] as Startveld)] + ">";
                            }
                            else
                            {
                                result += "<" + (++uitzonderingCount) + ">";
                                uitzondering += "*" + uitzonderingCount + ":BOS,";
                                // voeg spelers toe
                                uitzondering = _points[i, j].Pionnen.Aggregate(uitzondering,
                                                                               (current, point) =>
                                                                               current + (point.Speler.Name + ""));
                                uitzondering += "\r\n";
                            }
                        } 
                        else if(_points[i,j] is Veld)
                        {
                            var veld = _points[i, j] as Veld;
                            if (veld.Barricade != null)
                            {
                                inhoud = "*";
                            }
                            result += veld.StandaardBarricade ? "[" + inhoud + "]" : "(" + inhoud + ")";
                        } 
                        else
                        {
                            throw new Exception("Ik ken dit veld niet.");
                        }

                        var k = j + 1;

                        if (k >= width) break;

                        if (_points[i, k] == null)
                        {
                            result += " ";
                        }
                        if (_points[i, j].Buren.Contains(_points[i, k]))
                        {
                            for (var l = -1; l < (k - j - 1) * 3; l++)
                            {
                                result += "-";
                            }
                        }
                    }
                }
                result += "\r\n";

                result += "    ";
                for (var j = 0; j < width; j++)
                {
                    if (_points[i, j] == null)
                    {
                        result += "    ";
                    }
                    else
                    {
                        result += " ";
                        var add = false;
                        for (var k = i + 1; k < height; k++)
                        {
                            if (_points[k, j] == null) continue;
                            add = add || _points[i, j].Buren.Contains(_points[k, j]);
                        }
                        result += add ? "|" : " ";
                        result += "  ";
                    }
                }

                result += "\r\n";
            }

            return result + (metUitzonderingen ? uitzondering : "");

        }

	    private static string TekenInhoud(IVeld veld)
	    {
	        var inhoud = veld.Pionnen.First().Speler.Name + "";
	        return inhoud;
	    }

	    public String Output()
	    {
	        return Output(true);
	    }
	}
}

