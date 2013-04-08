using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barricade.Logic.Velden;

namespace Barricade.Logic.Exceptions
{
    class BarricadeVerplaatsException : Exception
    {
        public Veld Veld { get; set; }
        public Barricade Barricade { get; set; }

        public BarricadeVerplaatsException(Veld veld, Barricade barricade)
        {
            Veld = veld;
            Barricade = barricade;
        }
    }
}
