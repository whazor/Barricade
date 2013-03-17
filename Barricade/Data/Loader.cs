using System.Diagnostics;
using System.IO;
using Logic;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Barricade.Data
{
    public class Loader
    {
        List<Point> nodes;

        public Loader(String[] lines)
        {
            // Alle informatie over bijzondere vakjes ophalen
            var lastList = from line in lines
                           where line.StartsWith("*") && line.Contains(":") && !line.EndsWith(":")
                           select line;

            var uitzonderingen = lastList.ToDictionary(
                line => line.Trim('*').Split(':')[0][0],
                line => line.Split(':')[1]);


            // Min, max values, voor uitrekenen van breedte en lengte
            var firstX = int.MaxValue;
            var firstY = int.MaxValue;
            var lastX = int.MinValue;
            var lastY = int.MinValue;

            // Snelle controle naar spelgrootte en hoe de vakjes staan
            for (var i = 0; i < lines.Length; i++)
            {
                for (var j = 0; j < lines[i].Length; j++)
                {
                    if (!new[] {'<', '(', '[', '{'}.Contains(lines[i][j])) continue;

                    firstX = Math.Min(firstX, j);
                    lastX = Math.Max(lastX, j + 3);

                    firstY = Math.Min(firstY, i);
                    lastY = Math.Max(lastY, i);
                }
            }
            // Trucje om te kijken waar het middenpunt van een vakje zit
            var isXeven = (lastX - firstX - 1) % 2;
            var isYeven = firstY % 2;

            var barricades = new List<Logic.Barricade>();
            var spelers = new Dictionary<char, Speler>();
            var connecties = new List<Tuple<Position, Position>>();
            
            nodes = new List<Point>();

            var kaart = new IVeld[(int)Math.Ceiling(((decimal)(lastY - firstY + 1) / 2)), (lastX - firstX) / 4 + 1];

            
            var getX = new Func<int, int>(x => (x - firstX) / 4);
            var getY = new Func<int, int>(y => (int)Math.Ceiling(((decimal)(y - firstY + 1) / 2)) - 1);

            var linenr = 0;
            foreach (var line in lines)
            {
                linenr++;
                if (line.Length == 0)
                    continue;

                // Begin van de regel kan D of - staan, dit betekent iets voor de vakjes.
                var isDorp = (line[0] + line[1] + "").Contains("D");
                var isBarricadeVrij = (line[0] + line[1] + "").Contains("-");

                var expected = '\0';
                var previous = '\0';

                var letternr = 0;
                IVeld next = null;

                foreach (var letter in line)
                {
                    letternr++;
                    if (expected == '\0')
                        switch (letter)
                        {
                            case '<':
                                expected = '>';
                                break;
                            case '(':
                                expected = ')';
                                next = new Veld();
                                break;
                            case '[':
                                expected = ']';
                                next = new Veld();
                                //TODO: veld rood maken omdat er een barricade opstaat
                                break;
                            case '{':
                                expected = '}';
                                next = new Rustveld();
                                break;
                            case '-':
                                connecties.Add(new Tuple<Position, Position>(
                                    new Position(getX(letternr - 2), getY(linenr)), 
                                    new Position(getX(letternr + 2), getY(linenr))
                                    ));
                                break;
                            case '|':
                                connecties.Add(new Tuple<Position, Position>(
                                    new Position(getX(letternr), getY(linenr - 1)),
                                    new Position(getX(letternr), getY(linenr + 1))
                                    ));
                                break;
                        }
                    else if (expected == letter)
                    {
                        // uitzondering
                        if (expected == '>')
                        {
                            if (previous == ' ')
                            {
                                next = new Finishveld();
                            }
                            else if (uitzonderingen.ContainsKey(previous))
                            {
                                var uitzondering = uitzonderingen[previous];
                                if (uitzondering.StartsWith("BOS"))
                                {
                                    next = new Bos();
                                    //TODO: spelers uitzoeken
                                }
                                else if (uitzondering.StartsWith("START"))
                                {
                                    if (!uitzondering.Contains(",") || uitzondering.EndsWith(","))
                                    {
                                        throw new ParserException("Uitzondering '" + previous + "' (START), heeft geen speler");
                                    }
                                    next = new Startveld();

                                    var playerName = uitzondering.Split(',')[1][0];
                                    if (!spelers.ContainsKey(previous))
                                    {
                                        spelers[playerName] = new Speler();
                                    }
                                    spelers[playerName].Startveld.Add(next as Startveld);
                                    for (var i = 0; i < int.Parse(uitzondering.Split(',')[1][1]+""); i++)
                                    {
                                        var pion = new Pion { IVeld = next };
                                        if (!next.Pionen.Contains(pion))
                                            next.Pionen.Add(pion);
                                        spelers[playerName].Pionen.Add(pion);   
                                    }
                                }
                                else
                                {
                                    throw new ParserException("Uitzondering '" + previous + "' snap ik niet.");
                                }
                            }
                        }
                        else
                        {
                            Debug.Assert(next != null, "Deze situatie mag niet voorkomen.");
                            
                            next.IsDorp = isDorp;
                            if (next is Veld)
                            {
                                (next as Veld).MagBarricade = isBarricadeVrij;
                                if (isBarricadeVrij && previous == '*')
                                {
                                    var barricade = new Logic.Barricade();
                                    (next as Veld).Barricade = barricade;
                                    barricades.Add(barricade);
                                }
                            }
                            if (previous != '*')
                            {
                                if (!spelers.ContainsKey(previous))
                                {
                                    spelers[previous] = new Speler();
                                }
                                var pion = new Pion {IVeld = next};
                                if(!next.Pionen.Contains(pion)) 
                                    next.Pionen.Add(pion);
                                spelers[previous].Pionen.Add(pion);
                            }
                        }

                        
                        if (next != null)
                        {
                            var posx = getX(letternr);
                            var posy = getY(linenr);
                            if ((letternr-firstX-1) % 2 != isXeven)
                                throw new ParserException(linenr, letternr, "Dit symbool staat hier verkeerd, hij staat verkeerd tegenover de rest.");

                            kaart[posy, posx] = next;
                            nodes.Add(new Point(new Position(posx, posy), next));
                        }
                        //letternr-1, positie;
                        next = null;
                        expected = '\0';
                    }
                    previous = letter;
                }
            }

            /**
             * Hier alle nodes koppelen
             */
        }

        public Loader(TextReader file)
        {
            throw new NotImplementedException();
        }


        public List<Point> ToArray()
        {
            return nodes;
        }

        public class Position : Tuple<int, int>
        {
            public Position(int item1, int item2) : base(item1, item2)
            {
            }

            public int X { get { return Item1; } }
            public int Y { get { return Item2; } }
        }

        public class Point
        {
            public Point(Position a1, IVeld a2)
            {
                Locatie = a1;
                Veld = a2;
            }
            public IVeld Veld { get; set; }
            public Position Locatie { get; set; }
        }
    }

    public class ParserException : Exception
    {
        public ParserException(string s)
            : base(s)
        {

        }

        public ParserException(int linenr, int letternr, string message) 
            : base ("[regel "+linenr+", karakter "+letternr+"] "+message)
        {
        }
    }
}

