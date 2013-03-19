using System;
using System.Linq;
using System.Text;
using Logic;

namespace Barricade.Data
{

	public class Saver
	{
	    private readonly IVeld[,] _points;

	    public Saver(IVeld[,] points)
	    {
	        _points = points;
	    }

        public String Output()
        {
            var result = "";
            var height = _points.GetLength(0);
            var width = _points.GetLength(1);

            var uitzonderingCount = 0;
            var uitzondering = "";

//            for (var i = 0; i < height; i++)
//            {
//                for (var j = 0; j < width; j++)
//                {
//                    if (_points[i, j] != null)
//                    {
//                        Console.Write(_points[i, j].Buren.Count);
//                    }
//                    else
//                    {
//                        Console.Write(" ");
//                    }
//                    Console.Write(" ");
//                }
//                Console.Write("\r\n");
//            }

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (_points[i, j] == null)
                    {
                        result += "    ";
                    }
                    else
                    {
                        var inhoud = " ";
                        if (_points[i, j].Pionen.Count > 0)
                        {
                            inhoud = _points[i, j].Pionen.First().Speler.Name + "";
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
                            result += "<"+(++uitzonderingCount)+">";
                            if (_points[i, j] is Startveld)
                                uitzondering += uitzonderingCount + ":START,";
                            else
                                uitzondering += uitzonderingCount+":BOS,";

                            // voeg spelers toe
                            uitzondering = _points[i, j].Pionen.Aggregate(uitzondering, (current, point) => current + (point.Speler.Name + ""));
                            // enter voor de volgende uitzondering
                            uitzondering += "\r\n";
                        } 
                        else if(_points[i,j] is Veld)
                        {
                            var veld = _points[i, j] as Veld;
                            if (veld.Barricade != null)
                            {
                                inhoud = "*";
                            }
                            result += "(" + inhoud + ")";
                        } 
                        else
                        {
                            throw new Exception("Ik ken dit veld niet.");
                        }

                        for (var k = j + 1; k < width; k++)
                        {
                            if (_points[i, k] == null)
                            {
                                result += " ";
                                break;
                            }
                            if (_points[i, j].Buren.Contains(_points[i, k]))
                            {
//                                result += "-";
                                for (var l = -1; l < (k - j - 1) * 3; l++)
                                {
                                    result += "-";
                                }
                            }
                            break;
                        }
                    }
                }
                result += "\r\n";

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
                            if(!add)
                                add = _points[i, j].Buren.Contains(_points[k, j]);
                        }
                        result += add ? "|" : " ";
                        result += "  ";
                    }
                }

                result += "\r\n";
            }



            return result + uitzondering;

        }
	}
}

