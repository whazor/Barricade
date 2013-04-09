namespace Barricade.Logic.Velden
{
    public class Startveld : VeldBase
    {
        #region properties
        public Speler Speler { get; set; }
        public override bool MagBarricade { get { return false; } }
        public override bool MagPionErlangs { get { return true; } }
        public override bool MagPion(Pion pion)
        {
            return false;
        }
        #endregion

        #region methods
        /// <summary>
        /// Plaats pion op veld
        /// </summary>
        /// <param name="pion">Pion om er op te zetten</param>
        /// <returns>ja of nee</returns>
        public override bool Plaats(Pion pion)
        {
            Pionnen.Add(pion);
            return true;
        }
        #endregion

    }
}

