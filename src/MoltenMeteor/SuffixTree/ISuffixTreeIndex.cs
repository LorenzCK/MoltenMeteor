using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.SuffixTree {

    /// <summary>
    /// Index on the blob that can find elements by text suffixes.
    /// </summary>
    public interface ISuffixTreeIndex {

        /// <summary>
        /// Get offsets of elements matching a given suffix.
        /// </summary>
        /// <param name="suffix">String suffix to look for.</param>
        /// <returns>Enumeration of byte offsets of matching elements.</returns>
        IEnumerable<long> FindBySuffix(string suffix);

    }

}
