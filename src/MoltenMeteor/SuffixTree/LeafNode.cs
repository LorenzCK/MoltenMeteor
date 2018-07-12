using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.SuffixTree {

    internal class LeafNode : Node {

        private readonly List<int> _ids;

        public LeafNode(int id) {
            _ids = new List<int> { id };
        }

        public LeafNode(IEnumerable<int> ids) {
            _ids = new List<int>(ids);
        }

        public int[] IDs {
            get {
                return _ids.ToArray();
            }
        }

        public override void AddLink(string suffix, LeafNode link) {
            if (!string.IsNullOrWhiteSpace(suffix))
                throw new InvalidOperationException(string.Format("Cannot push new leaf node ({0}) onto leaf node ({1}) with a non-null suffix ({2}).", link, this, suffix));

            _ids.AddRange(link.IDs);
        }

        public override string ToString() {
            return "[" + string.Join(",", _ids) + "]";
        }

    }

}
