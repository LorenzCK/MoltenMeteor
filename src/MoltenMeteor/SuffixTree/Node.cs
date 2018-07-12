using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.SuffixTree {

    internal abstract class Node {

        public abstract void AddLink(string suffix, LeafNode link);

    }

}
