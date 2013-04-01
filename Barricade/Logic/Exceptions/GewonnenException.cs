using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Barricade.Logic.Exceptions
{
    class GewonnenException : Exception
    {

        public Speler Speler { get; set; }

        public GewonnenException(Speler speler)
        {
            Speler = speler;
        }
    }
}