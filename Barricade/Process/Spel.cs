using System;
using Barricade.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Barricade.Process
{
	public class Spel
	{
	    private readonly Logic.Spel _logicSpel;
	    private int _beurt;
	    public Logic.Speler AanDeBeurt { get; private set; }

        public int Plaatsen { get; set; }

	    public delegate void BeurtWijzigingEvent(Logic.Speler speler, int dobbel);
	    public delegate void Neerzetten(Logic.Barricade barricade, IVeld bestemming);
        public delegate void BarricadeVerplaatsEvent(Logic.Barricade barricade, Neerzetten zetOp);

        public event BeurtWijzigingEvent BeurtWijziging;
        public event BarricadeVerplaatsEvent BarricadeVerplaatsing;

	    public Spel(Logic.Spel logicSpel)
	    {
	        _logicSpel = logicSpel;
	    }

        public virtual void Start()
        {
            VolgendeBeurt();
        }

	    public virtual void Verplaats(Pion pion, IVeld bestemming)
	    {
            try
            {
                pion.Verplaats(bestemming);
                VolgendeBeurt();
            }
            catch (Logic.Exceptions.GewonnenException e)
            {
                //TODO: iemand heeft gewonnen
            }
            catch (Logic.Exceptions.BarricadeVerplaatsException e)
            {
                BarricadeVerplaatsing(e.Barricade, (a, b) =>
                    {
                        // Plaats barricade
                        Verplaats(a, b);
                        // Opnieuw pion plaatsen
                        Verplaats(pion, bestemming);
                    });
            }
		}

		public virtual void Verplaats(Logic.Barricade barricade, IVeld veld)
		{
            barricade.Verplaats(veld as Veld);
		}

	    readonly Random random = new Random();
        private void VolgendeBeurt()
        {
            AanDeBeurt = _logicSpel.Spelers[_beurt++ % _logicSpel.Spelers.Count];
            //TODO: check naar mogelijkheden
            Plaatsen = random.Next(1, 7);
            if(BeurtWijziging != null) 
                BeurtWijziging(AanDeBeurt, Plaatsen);
            //TODO check of het klopt
        }
	}
}

