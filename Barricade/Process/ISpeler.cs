using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barricade.Logic;

namespace Barricade.Process
{
    public interface ISpeler
    {
        /// <summary>
        /// Uit deze methode komt de bestemming van een barricade, dit mag lang duren.
        /// </summary>
        /// <param name="magBarricade">methode die bepaald of een veld een barricade is</param>
        /// <returns>veld</returns>
        Task<IVeld> VerplaatsBarricade(Func<IVeld, bool> magBarricade);

        /// <summary>
        /// Uit deze methode komt een pion, dit mag lang duren.
        /// </summary>
        /// <param name="pionnen"></param>
        /// <returns></returns>
        Task<Pion> KiesPion(ICollection<Pion> pionnen);

        /// <summary>
        /// Uit deze methode komt de bestemming van een pion, dit mag lang duren.
        /// </summary>
        /// <param name="gekozen">eerder gekozen pion</param>
        /// <param name="mogelijk">alle velden waar deze pion op mag staan</param>
        /// <returns></returns>
        Task<IVeld> VerplaatsPion(Pion gekozen, ICollection<IVeld> mogelijk);

        // Huidige dobbel aantal
        int Gedobbeld { get; set; }

        // Wie er aan de beurt is
        Speler AanDeBeurt { get; set; }

        Task DobbelTask();
    }
}
