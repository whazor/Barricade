using System.Collections.Generic;

namespace Barricade.Logic.Velden
{
    public class Bos : IVeld
    {
        #region Properties & variablen
        public bool IsDorp { get; set; }
	    public bool IsPlaatsbaar { get; set; }
	    public List<IVeld> Buren { get; private set; }
	    public List<Pion> Pionnen { get; set; }
        public int Score { get; set; }
	    public bool MagBarricade { get { return false; } }
        public bool MagPionErlangs { get { return true; } }
	    public bool MagPion(Pion pion)
        {
	        return true;
	    }
        #endregion

        #region constructor
        public Bos()
        {
            Buren = new List<IVeld>();
            Pionnen = new List<Pion>();
        }
        #endregion

        #region logica methodes
        /// <summary>
        /// Plaats een pion in het bos
        /// </summary>
        /// <param name="pion">de pion</param>
        /// <returns>altijd true</returns>
        public bool Plaats(Pion pion)
        {
            //meerdere pionnen op dit veld toegestaan
            Pionnen.Add(pion);
            return true;
        }
        #endregion

    }


}

