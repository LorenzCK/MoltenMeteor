using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.SuffixTree {

    internal class InternalNode : Node {

        private readonly List<(string, Node)> _links;

        public InternalNode() {
            _links = new List<(string, Node)>();
        }

        public InternalNode(
            string leftSuffix, Node leftNode,
            string rightSuffix, Node rightNode) {

            _links = new List<(string, Node)> {
                (leftSuffix, leftNode),
                (rightSuffix, rightNode)
            };
        }

        public override void AddLink(string suffix, LeafNode link) {
            var pickIndex = SeekPointer(suffix);

            if(pickIndex >= 0) {
                (var label, var target) = _links[pickIndex];

                var overlapping = suffix.CountOverlapping(label);
                if(overlapping == label.Length) {
                    // Picked pointer fully matches suffix to insert
                    // Ex.: "example$" picks arch with "exa" label
                    target.AddLink(suffix.Substring(label.Length), link);
                }
                else {
                    // Picked pointer matches partially (must be split)
                    // Ex.: "example$" picks arch with "exho" label
                    _links[pickIndex] = (label.Substring(0, overlapping), new InternalNode(
                        label.Substring(overlapping), target,
                        suffix.Substring(overlapping), link
                     ));
                }
            }
            else {
                // No pointer found, add a new one
                _links.Add((suffix, link));
            }
        }

        /// <summary>
        /// Seeks the best matching link in this node's array.
        /// </summary>
        private int SeekPointer(string suffix) {
            Debug.Assert(suffix.Length >= 1);

            for(int i = 0; i < _links.Count; ++i) {
                if(_links[i].Item1[0] == suffix[0]) {
                    return i;
                }
            }

            return -1;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach(var link in _links) {
                if (sb.Length > 1)
                    sb.Append(",");
                sb.AppendFormat("'{0}'=>{1}", link.Item1, link.Item2.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }

    }

}
