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
            var veldenCount = 0;

            // Snelle controle naar spelgrootte en hoe de vakjes staan
            for (var i = 0; i < lines.Length; i++)
            {
                var lineVeldenCount = 0;
                for (var j = 0; j < lines[i].Length; j++)
                {
                    if (!new[] {'<', '(', '[', '{'}.Contains(lines[i][j])) continue;

                    lineVeldenCount++;
                    firstX = Math.Min(firstX, j + 1);
                    firstY = Math.Min(firstY, i);
                    lastX = Math.Max(lastX, j + 1);
                    lastY = Math.Max(lastY, i);
                }
                veldenCount = Math.Max(lineVeldenCount, veldenCount);
            }
            // Trucje om te kijken waar het middenpunt van een vakje zit
            var isEven = firstX % 2;

            var barricades = new List<Logic.Barricade>();
            var spelers = new Dictionary<char, Speler>();
            var connecties = new List<Tuple<Position, Position>>();

            var kaart = new IVeld[(lastX-firstX+1) / 4, lastY - firstY];

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
                                    new Position(letternr - firstX - 2, linenr), 
                                    new Position(letternr - firstX + 2, linenr)
                                    ));
                                break;
                            case '|':
                                connecties.Add(new Tuple<Position, Position>(
                                    new Position(letternr - firstX, linenr - 1),
                                    new Position(letternr - firstX, linenr + 1)
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
                            var posx = letternr - 1 - firstX;
                            var posy = linenr;
                            if (posx % 2 != isEven)
                                throw new ParserException(linenr, letternr, "Dit symbool staat hier verkeerd, hij staat verkeerd tegenover de rest.");

                            kaart[posx, posy] = next;
                        }
                        //letternr-1, positie;
                        next = null;
                        expected = '\0';
                    }
                    previous = letter;
                }
            }
        }

        public Loader(TextReader file)
        {
            throw new NotImplementedException();
        }


        public List<Point> ToArray()
        {
            throw new NotImplementedException();
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

