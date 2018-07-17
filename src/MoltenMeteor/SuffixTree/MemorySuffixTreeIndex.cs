using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.SuffixTree {

    /// <summary>
    /// In-memory representation of a suffix tree.
    /// </summary>
    public class MemorySuffixTreeIndex : ISuffixTreeIndex {

        private readonly InternalNode _root;
        private const string Terminator = "$";

        public MemorySuffixTreeIndex() {
            _root = new InternalNode();
        }

        public void AddSuffix(string suffix, int id) {
            _root.AddLink(suffix + Terminator, new LeafNode(id));
        }

        public IEnumerable<int> FindBySuffix(string suffix) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return _root.ToString();
        }

    }

}
