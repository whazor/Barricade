
using System.Windows.Media;
using Barricade.Logic.Velden;

namespace Barricade.Logic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class Speler
    {
        #region properties en variablen
        public char Name { get; private set; }
        private static Dictionary<char, Color> kleuren = new Dictionary<char, Color>();

        public Color Kleur
        {
            get { return kleur((Name)); }
        }


        public virtual List<Pion> Pionnen
        {
            get;
            private set;
        }

        public virtual Spel Spel
        {
            get;
            set;
        }

        public virtual Startveld Startveld
        {
            get;
            set;
        }
        #endregion

        #region constructor
        public Speler(char name)
        {
            Name = name;
            Pionnen = new List<Pion>();
        }
        #endregion

        #region methods
        /// <summary>
        /// Geeft kleur
        /// </summary>
        /// <param name="speler">speler</param>
        /// <returns>kleur</returns>
        private static Color kleur(char speler)
	    {
            if(kleuren.Count == 0) {
                kleuren['A'] = Color.FromRgb(255, 191, 0);
                kleuren['B'] = Color.FromRgb(0, 0, 255);
                kleuren['C'] = Color.FromRgb(0, 255, 255);
                kleuren['D'] = Color.FromRgb(21, 96, 189);
                kleuren['E'] = Color.FromRgb(97, 64, 81);
                kleuren['F'] = Color.FromRgb(255, 0, 255);
                kleuren['G'] = Color.FromRgb(0, 255, 0);
                kleuren['H'] = Color.FromRgb(223, 155, 255);
                kleuren['I'] = Color.FromRgb(0, 65, 106);
                kleuren['J'] = Color.FromRgb(0, 168, 107);
                kleuren['K'] = Color.FromRgb(195, 176, 145);
                kleuren['L'] = Color.FromRgb(255, 250, 205);
                kleuren['M'] = Color.FromRgb(128, 0, 0);
                kleuren['N'] = Color.FromRgb(0, 0, 128);
                kleuren['O'] = Color.FromRgb(128, 128, 0);
                kleuren['P'] = Color.FromRgb(128, 0, 128);
                kleuren['Q'] = Color.FromRgb(108, 105, 97);
                kleuren['R'] = Color.FromRgb(255, 0, 0);
                kleuren['S'] = Color.FromRgb(112, 66, 20);
                kleuren['T'] = Color.FromRgb(255, 99, 71);
                kleuren['U'] = Color.FromRgb(18, 10, 143);
                kleuren['V'] = Color.FromRgb(139, 0, 255);
                kleuren['W'] = Color.FromRgb(255, 255, 255);
                kleuren['X'] = Color.FromRgb(238, 237, 9);
                kleuren['Y'] = Color.FromRgb(255, 255, 0);
                kleuren['Z'] = Color.FromRgb(80, 96, 34);
            }
            if(kleuren.ContainsKey(speler))
	            return kleuren[speler];
            return Color.FromRgb(0,0,0);
	    }
        #endregion

	}
}

