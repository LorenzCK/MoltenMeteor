using MoltenMeteor.SuffixTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    public static class MeteorExtensions {

        /// <summary>
        /// Find matching elements in a <see cref="Meteor{T}"/> using a suffix tree index.
        /// </summary>
        public static IEnumerable<T> FindBySuffix<T>(this Meteor<T> m, ISuffixTreeIndex index, string suffix) {
            return from e in index.FindBySuffix(suffix)
                   select m.Get(e);
        }

    }

}
