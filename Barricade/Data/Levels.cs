using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Barricade.Data
{
    public class Levels
    {
        static public List<string> Lijst()
        {
            var resourceManager = Items.ResourceManager;
            var levels = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            return (from DictionaryEntry level in levels select (string)level.Key).ToList();
        }

        public static string[] Open(string s)
        {
            var bytes = Items.ResourceManager.GetObject(s) as Byte[];
            Debug.Assert(bytes != null, "bytes != null");
            return Encoding.UTF8.GetString(bytes).Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        }
    }
}
