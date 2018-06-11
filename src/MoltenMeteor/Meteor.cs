using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    /// <summary>
    /// Represents a single database unit, composed of one binary blob and any
    /// number of indices on it, that can be read.
    /// </summary>
    public class Meteor {

        public delegate Stream StreamOpener();

        private readonly StreamOpener _opener;

        public Meteor(StreamOpener opener) {
            _opener = opener ?? throw new ArgumentNullException(nameof(opener));

            using(var r = new BlobReader(OpenBlobStream())) {
                BlobIdentifier = r.Identifier;
                BlobVersion = r.Version;
            }
        }

        protected Stream OpenBlobStream() {
            return _opener();
        }

        /// <summary>
        /// Gets the binary blob's unique identifier.
        /// </summary>
        public Guid BlobIdentifier { get; private set; }

        /// <summary>
        /// Gets the binary blob's data format version.
        /// </summary>
        public int BlobVersion { get; private set; }

        /// <summary>
        /// Gets the blob's identifier index, if any.
        /// </summary>
        public IIdentifierIndex IdentifierIndex { get; private set; }

        /// <summary>
        /// Reads an identifier index from an input stream and loads
        /// an in-memory representation as the blob's current identifier index.
        /// </summary>
        public StreamIdentifierIndex LoadIdentifierIndex(StreamOpener indexOpener) {
            var newIndex = new StreamIdentifierIndex(this, indexOpener);
            IdentifierIndex = newIndex.ToInMemory();
            return newIndex;
        }

        /// <summary>
        /// Generates a new in-memory identifier index ands loads it as
        /// the blob's current identifier index.
        /// </summary>
        public MemoryIdentifierIndex GenerateIdentifierIndex() {
            using(var reader = new BlobReader(OpenBlobStream())) {
                var newIndex = new MemoryIdentifierIndex(reader.ReadAllOffsets());
                IdentifierIndex = newIndex;
                return newIndex;
            }
        }

        /// <summary>
        /// Gets an element's binary information as a raw byte array.
        /// </summary>
        public byte[] Get(int id) {
            using (var reader = new BlobReader(OpenBlobStream())) {
                if(IdentifierIndex != null) {
                    long offset = IdentifierIndex.FindById(id);
                    if (offset < 0)
                        throw new ArgumentException("Element not found", nameof(id));

                    (var outId, var outData) = reader.ReadAsArray(offset);
#if DEBUG
                    if (outId != id)
                        throw new InvalidOperationException($"Loading data for element #{id} returned data for #{outId}");
#endif

                    return outData;
                }
                else {
                    // Fallback to linear search on blob data
                    foreach(var element in reader.ReadAll()) {
                        if(element.id == id) {
                            byte[] outData = new byte[element.data.Length];
                            element.data.Read(outData, 0, (int)element.data.Length);
                            return outData;
                        }
                    }

                    throw new ArgumentException("Element not found", nameof(id));
                }
            }
        }

        

    }

}
