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
        Task<IVeld> KiesVeld(Func<IVeld, bool> mogelijk);

        Task<Pion> KiesPion(ICollection<Pion> pionnen);
        Task<IVeld> KiesVeld(ICollection<IVeld> velden);

        int Gedobbeld { get; set; }
        Speler AanDeBeurt { get; set; }
    }
}
