using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Barricade.Data
{
    class Levels2
    {
        readonly ResourceSet resourceSet = Levels.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        public List<string> Lijst()
        {
            return (from DictionaryEntry entry in resourceSet select entry.Key as string).ToList();
        }
    }
}
