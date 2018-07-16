using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.IdentifierIndex {

    public static class IdentifierIndexMeteorExtensions {

        /// <summary>
        /// Reads an identifier index from an input stream and loads
        /// an in-memory representation as the blob's current identifier index.
        /// </summary>
        public static StreamIdentifierIndex<T> LoadIdentifierIndex<T>(this Meteor<T> m, StreamOpener indexOpener) {
            var newIndex = new StreamIdentifierIndex<T>(m, indexOpener);
            m.IdentifierIndex = newIndex.ToInMemory();
            return newIndex;
        }

        /// <summary>
        /// Generates a new in-memory identifier index ands loads it as
        /// the blob's current identifier index.
        /// </summary>
        public static MemoryIdentifierIndex GenerateIdentifierIndex<T>(this Meteor<T> m) {
            using (var reader = new BlobReader(m.OpenBlobStream())) {
                var newIndex = new MemoryIdentifierIndex(reader.ReadAllOffsets());
                m.IdentifierIndex = newIndex;
                return newIndex;
            }
        }

    }

}
