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
	    public Logic.Speler AanDeBeurt { get; private set; }

        public delegate void BeurtWijzigingEvent(Logic.Speler speler);
        public event BeurtWijzigingEvent BeurtWijziging;

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

            }
            catch (Logic.Exceptions.BarricadeVerplaatsException e)
            {
                
            }
		}

		public virtual void Verplaats(Logic.Barricade barricade, IVeld bestemming)
		{
			throw new System.NotImplementedException();
		}


        private void VolgendeBeurt()
        {
            AanDeBeurt = _logicSpel.Spelers.First();
            if(BeurtWijziging != null) 
                BeurtWijziging(AanDeBeurt);
        }
	}
}

