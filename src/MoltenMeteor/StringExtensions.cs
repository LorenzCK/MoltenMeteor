using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    internal static class StringExtensions {

        /// <summary>
        /// Counts the number of overlapping characters between strings.
        /// </summary>
        public static int CountOverlapping(this string a, string b) {
            int count = 0;
            while (count < a.Length && count < b.Length && a[count] == b[count]) {
                ++count;
            }
            return count;
        }

    }

}
