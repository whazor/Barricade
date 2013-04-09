using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barricade.Utilities
{
    static class StringExtensions
    {
        public static string Replace(this string target, int i, string value)
        {
            return target.Insert(i, value).Remove(i + value.Length, value.Length);
        }
    }
}
