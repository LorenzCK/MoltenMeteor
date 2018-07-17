using MoltenMeteor.IdentifierIndex;
using System;
using System.Collections.Generic;
using System.IO;

namespace MoltenMeteor {

    /// <summary>
    /// Represents a single database unit, based on one binary blob that can be
    /// read and contains entries that can be read as instances of T.
    /// </summary>
    public class Meteor<T> {

        public delegate T EntryReader(Stream input);

        private readonly StreamOpener _opener;
        private readonly EntryReader _reader;

        public Meteor(StreamOpener opener, EntryReader reader) {
            _opener = opener ?? throw new ArgumentNullException(nameof(opener));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));

            using(var r = new BlobReader(OpenBlobStream())) {
                BlobIdentifier = r.Identifier;
                BlobVersion = r.Version;
            }
        }

        protected internal Stream OpenBlobStream() {
            return _opener();
        }

        protected internal T ReadEntry(Stream input) {
            return _reader(input);
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
        public IIdentifierIndex IdentifierIndex { get; protected internal set; }

        protected (long offset, BlobReader reader) Find(int id) {
            var reader = new BlobReader(OpenBlobStream());

            // Generate new in-memory identifier index if none available
            if (IdentifierIndex == null) {
                var newIndex = new MemoryIdentifierIndex(reader.ReadAllOffsets());
                IdentifierIndex = newIndex;
            }

            var offset = IdentifierIndex.FindById(id);
            if(offset < 0) {
                reader.Dispose();
                throw new ArgumentException("Element not found", nameof(id));
            }

            var foundId = reader.ReadId(offset);
            if (foundId != id) {
                reader.Dispose();
                throw new InvalidOperationException($"Loading data for element #{id} returned data for #{foundId}");
            }

            return (offset, reader);
        }

        /// <summary>
        /// Gets an element's binary information as a raw byte array.
        /// </summary>
        public byte[] GetRaw(int id) {
            (long offset, var reader) = Find(id);
            using (reader) {
                (_, var data) = reader.ReadAsArray(offset);
                return data;
            }
        }

        /// <summary>
        /// Gets an element by ID and converts it into an instances of T.
        /// </summary>
        public T Get(int id) {
            (long offset, var reader) = Find(id);
            using (reader) {
                (_, var stream) = reader.ReadAsStream(offset);
                return ReadEntry(stream);
            }
        }

    }

}
