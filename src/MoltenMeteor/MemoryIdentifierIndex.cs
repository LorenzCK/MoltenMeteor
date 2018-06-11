using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    /// <summary>
    /// In-memory representation of an identifier index.
    /// </summary>
    public class MemoryIdentifierIndex : IIdentifierIndex {

        private readonly Dictionary<int, long> _map;

        internal MemoryIdentifierIndex(IEnumerable<(int, long)> map) {
            _map = new Dictionary<int, long>();
            foreach(var element in map) {
                _map.Add(element.Item1, element.Item2);
            }
        }

        public long FindById(int id) {
            if (!_map.ContainsKey(id))
                return -1;

            return _map[id];
        }

        /// <summary>
        /// Get number of elements in the index.
        /// </summary>
        public int Count {
            get => _map.Count;
        }

    }

}
