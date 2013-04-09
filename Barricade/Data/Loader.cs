using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Barricade.Logic;
using Barricade.Logic.Velden;

namespace Barricade.Data
{
    public class Loader
    {
        #region Constructor
        public Loader(String[] lines)
        {
            Parse(lines);
        }

        public Loader(TextReader file)
        {
            var list = new List<string>();
            while (file.Peek() >= 0)
            {
                list.Add(file.ReadLine());
            }
            Parse(list.ToArray());
        }

        public Loader(string file)
        {
            Parse(file.Split('\n'));
        }

        #endregion

        #region Properties
        private Dictionary<char, Speler> Spelers { get; set; }
        public IVeld[,] Kaart { get; private set; }
        public List<Connection> Connecties { get; private set; }
        public Spel Spel { get; set; }
        #endregion

        #region Methodes
        private static void CalculateSize(IList<string> lines, out int firstX, out int firstY, out int lastX,
                                  out int lastY,
                                  out int isXeven, out int isYeven)
        {
            // Min, max values, voor uitrekenen van breedte en lengte
            firstX = int.MaxValue;
            firstY = int.MaxValue;
            lastX = int.MinValue;
            lastY = int.MinValue;

            // Snelle controle naar spelgrootte en hoe de vakjes staan
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    if (!new[] { '<', '(', '[', '{' }.Contains(lines[i][j])) continue;

                    firstX = Math.Min(firstX, j);
                    lastX = Math.Max(lastX, j + 3);

                    firstY = Math.Min(firstY, i);
                    lastY = Math.Max(lastY, i);
                }
            }
            // Trucje om te kijken waar het middenpunt van een vakje zit
            isXeven = (lastX - firstX - 1) % 2;
            isYeven = firstY % 2;
        }
        
        private void Parse(string[] lines)
        {
            // Alle informatie over bijzondere vakjes ophalen
            var uitzonderingen = ParseUitzonderingen(lines);

            int firstX, firstY;
            int lastX, lastY;
            int isXeven, isYeven;
            CalculateSize(lines, out firstX, out firstY, out lastX, out lastY, out isXeven, out isYeven);

            var height = (int) Math.Ceiling(((decimal) (lastY - firstY + 1)/2));
            var width = (lastX - firstX)/4 + 1;

            Spelers = new Dictionary<char, Speler>();
            Connecties = new List<Connection>();
            Kaart = new IVeld[height,width];

            var getX = new Func<int, int>(x => (x - firstX)/4);
            var getY = new Func<int, int>(y => (int) Math.Ceiling(((decimal) (y - firstY + 1)/2)) - 1);

            var seeds = lines.Where(line => line.StartsWith("*SEED:")).ToList();
            var seed = new Random().Next();
            var tries = 0;
            if (seeds.Any())
            {
                var first = seeds.First().Split(':')[1];
                var splitsel = first.Split(',');
                seed = int.Parse(splitsel[0]);
                tries = int.Parse(splitsel[1]);
            }
            Spel = new Spel(seed, tries);

            foreach (var start in uitzonderingen.Where(line => line.Value.StartsWith("START")))
            {
                char playerName = start.Value.Split(',')[1][0];
                CreatePlayer(playerName, Spelers);

            }

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length < 2) continue;

                var line = lines[i];

                var isDorp = line.Substring(0,2).Contains("D");
                var isBeschermd = line.Substring(0, 2).Contains("-");

                for (var j = firstX; j < line.Length; j++)
                {
                    var letter = line[j];
                    if (new[] {'<', '(', '[', '{'}.Contains(letter))
                    {
                        var letters = line.Substring(j, 3);

                        Kaart[getY(i), getX(j + 1)] = ParseBlock(letters, isBeschermd, isDorp, uitzonderingen);

                        j += 2;
                    }
                    else
                        switch (letter)
                        {
                            case '-':
                                {
                                    var pos1 = new Position(getX(j - 2), getY(i));
                                    var pos2 = new Position(getX(j + 2), getY(i));
                                    Connecties.Add(new Connection(pos1, pos2));
                                }
                                break;
                            case '|':
                                if (i%2 != isYeven)
                                {
                                    var pos1 = new Position(getX(j), getY(i - 1));
                                    var pos2 = new Position(getX(j), getY(i + 1));

                                    Connecties.Add(new Connection(pos1, pos2));
                                }
                                break;
                        }
                }
            }

            /**
             * Hier alle nodes koppelen
             */
            foreach (var connectie in Connecties)
            {
                var first = Kaart[connectie.Item1.Y, connectie.Item1.X];
                var second = Kaart[connectie.Item2.Y, connectie.Item2.X];

                if (first != null && second == null)
                {
                    // Kijk of de code verticaal of horizontaal moet
                    if (connectie.Item1.X == connectie.Item2.X)
                    {
                        for (var i = connectie.Item2.Y; i < Kaart.GetLength(0); i++)
                        {
                            second = Kaart[i, connectie.Item2.X];
                            if (second != null) break;
                        }
                    }
                    else if (connectie.Item1.Y == connectie.Item2.Y)
                    {
                        for (var i = connectie.Item2.X; i < Kaart.GetLength(1); i++)
                        {
                            second = Kaart[connectie.Item2.Y, i];
                            if (second != null) break;
                        }
                    }
                }
                if (first == null || second == null) continue;

                first.Buren.Add(second);
                second.Buren.Add(first);
            }

            Spel.BerekenScores();

        }

        private static Dictionary<char, string> ParseUitzonderingen(IEnumerable<string> lines)
        {
            var lastList = from line in lines
                           where line.StartsWith("*") && line.Contains(":") && !line.EndsWith(":")
                           select line;

            var uitzonderingen = lastList.ToDictionary(
                line => line.Trim('*').Split(':')[0][0],
                line => line.Split(':')[1]);
            return uitzonderingen;
        }



        private IVeld ParseBlock(string letters, bool isBeschermd, bool isDorp,
                                 IReadOnlyDictionary<char, string> uitzonderingen)
        {
            IVeld veld = null;
            if (letters[0] == '<' && letters[2] == '>')
            {
                if (letters[1] == ' ')
                {
                    veld = new Finishveld();
                    Spel.Finishvelden.Add(veld as Finishveld);
                }
                else
                {
                    if (uitzonderingen.ContainsKey(letters[1]))
                    {
                        string uitzondering = uitzonderingen[letters[1]];
                        if (uitzondering.StartsWith("BOS"))
                        {
                            veld = new Bos();
                            Spel.Bos = (Bos) veld;

                            string players = uitzondering.Split(',')[1];
                            foreach (char player in players)
                            {
                                var speler = CreatePlayer(player, Spelers);
                                NieuwPion(speler, veld);
                            }
                        }
                        else if (uitzondering.StartsWith("START"))
                        {
                            if (!uitzondering.Contains(",") || uitzondering.EndsWith(","))
                            {
                                throw new ParserException("Uitzondering '" + letters[1] + "' (START), heeft geen speler");
                            }
                            veld = new Startveld();
                            char playerName = uitzondering.Split(',')[1][0];
                            int amount = int.Parse(uitzondering.Split(',')[1].Substring(1));

                            var speler = CreatePlayer(playerName, Spelers);
                            for (int i = 0; i < amount; i++)
                            {
                                NieuwPion(speler, veld);
                            }
                            // bijhouden welk veld van welke speler is
                            speler.Startveld = veld as Startveld;
                            (veld as Startveld).Speler = speler;
                        }
                        else
                        {
                            throw new ParserException("Uitzondering '" + letters[1] + "' snap ik niet.");
                        }
                    }
                }

                return veld;
            }

            if (letters[0] == '(' && letters[2] == ')')
            {
                veld = new Veld();
            }
            else if (letters[0] == '[' && letters[2] == ']')
            {
                veld = new Veld() { StandaardBarricade = true };
            }
            else if (letters[0] == '{' && letters[2] == '}')
            {
                veld = new Rustveld();
            }
            else
            {
                throw new ParserException("Dit veld ken ik niet");
            }
            veld.IsDorp = isDorp;
            /**
             * Kijken of er een barricade op mag komen.
             */
            if (veld is Veld)
            {
                var barricadeVeld = veld as Veld;
                barricadeVeld.IsBeschermd = isBeschermd;
                if (!isBeschermd && letters[1] == '*')
                {
                    var barricade = new Logic.Barricade();
                    barricadeVeld.Barricade = barricade;
                    barricade.Veld = barricadeVeld;
                }
            }

            /**
             * Kijken of er een speler op staat
             */
            if (letters[1] != '*' && letters[1] != ' ')
            {
                var speler = CreatePlayer(letters[1], Spelers);
                NieuwPion(speler, veld);
            }

            return veld;
        }

        private static void NieuwPion(Speler speler, IVeld veld)
        {
            var pion = new Pion(speler) {IVeld = veld};
            if (!veld.Pionnen.Contains(pion))
                veld.Pionnen.Add(pion);
            speler.Pionnen.Add(pion);
        }

        private Speler CreatePlayer(char letter, Dictionary<char, Speler> spelers)
        {
            if (!spelers.ContainsKey(letter))
            {
                spelers[letter] = new Speler(letter) {Spel = Spel};
                Spel.Spelers.Add(spelers[letter]);
            }
            return spelers[letter];
        }

        public IVeld[,] ToArray()
        {
            return Kaart;
        }
        #endregion

        public class Connection : Tuple<Position, Position>
        {
            public Connection(Position item1, Position item2) : base(item1, item2)
            {
            }
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

        public class Position : Tuple<int, int>
        {
            public Position(int item1, int item2) : base(item1, item2)
            {
            }

            public int X
            {
                get { return Item1; }
            }

            public int Y
            {
                get { return Item2; }
            }
        }
    }

    public class ParserException : Exception
    {
        public ParserException(string s)
            : base(s)
        {
        }

        public ParserException(int linenr, int letternr, string message)
            : base("[regel " + linenr + ", karakter " + letternr + "] " + message)
        {
        }
    }
}