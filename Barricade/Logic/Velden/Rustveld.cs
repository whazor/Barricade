
using System.Linq;

namespace Barricade.Logic.Velden
{
    public class Rustveld : VeldBase
    {
        #region properties
        public override bool MagBarricade { get { return false; } }
        public override bool MagPionErlangs { get { return true; } }
        #endregion

        #region methods

        /// <summary>
        /// Geeft aan of die leeg is of niet.
        /// </summary>
        /// <param name="pion">Pion</param>
        /// <returns>ja of nee</returns>
        public override bool MagPion(Pion pion)
        {
            return Pionnen.Count == 0;
        }

        /// <summary>
        /// Plaatst een pion op het veld
        /// </summary>
        /// <param name="pion">Pion</param>
        /// <returns>boolean als het gelukt is of niet</returns>
        public override bool Plaats(Pion pion)
        {
            //staat al iets op
            if (Pionnen.Any()) return false;

            Pionnen.Add(pion);
            return true;
        }
        #endregion

    }
}

