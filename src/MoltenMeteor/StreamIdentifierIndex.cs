using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    /// <summary>
    /// Identifier index that accesses a readable stream.
    /// </summary>
    public class StreamIdentifierIndex : IIdentifierIndex {

        private readonly Meteor.StreamOpener _opener;

        /// <summary>
        /// Byte size of single index blocks (int32 ID, uint32 offset).
        /// </summary>
        const int BlockSize = 8;

        /// <summary>
        /// Threshold of elements to search under which linear search
        /// is used instead of binary.
        /// </summary>
        /// <remarks>
        /// This has been sperimentally detected on a PC.
        /// Probably works on mobile in a similar way.
        /// </remarks>
        const int LinearSearchThreshold = 4;

        public StreamIdentifierIndex(Meteor owner, Meteor.StreamOpener opener) {
            if(owner == null)
                throw new ArgumentNullException(nameof(owner));
            if (opener == null)
                throw new ArgumentNullException(nameof(opener));

            _opener = opener;

            // Test input stream
            using (var input = _opener()) {
                if (input == null)
                    throw new ArgumentNullException();
                if (!input.CanRead || !input.CanSeek)
                    throw new ArgumentException("Cannot read or seek stream");
                if ((input.Length - Constants.CommonHeaderLength) % BlockSize != 0)
                    throw new ArgumentException("Malformed stream (wrong length)");

                Count = (int)((input.Length - Constants.CommonHeaderLength) / BlockSize);

                // Header check
                (var reader, var version, var identifier) = input.ReadBinaryHeader();
                if (identifier != owner.BlobIdentifier) {
                    throw new ArgumentException("Identifier index does not match owning blob file");
                }
            }
        }

        /// <summary>
        /// Get number of elements in the index.
        /// </summary>
        public int Count { get; private set; }

        public long FindById(int id) {
            int a = 0;
            int b = Count - 1;

            using (var reader = new BinaryReader(_opener())) {
                while (b >= a) {
                    if ((b - a) <= LinearSearchThreshold) {
                        return LinearSearch(reader, id, a, b);
                    }

                    int middle = (b + a) / 2;

                    reader.BaseStream.Position = IndexToOffset(middle);
                    int middleId = reader.ReadInt32();

                    if (middleId == id) {
                        return reader.ReadInt32();
                    }
                    else if (id > middleId) {
                        a = middle + 1;
                    }
                    else {
                        b = middle - 1;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Sequentially reads all entries in the ID index.
        /// </summary>
        public IEnumerable<(int id, long offset)> ReadAll() {
            using (var reader = new BinaryReader(_opener())) {
                reader.BaseStream.MoveToData();

                while (reader.BaseStream.Position < reader.BaseStream.Length) {
                    var id = reader.ReadInt32();
                    var offset = reader.ReadUInt32();

                    yield return (id, offset);
                }

                yield break;
            }
        }

        /// <summary>
        /// Converts word index to offset inside the ID map file.
        /// </summary>
        private long IndexToOffset(int i) {
#if DEBUG
            if (i >= Count)
                throw new ArgumentOutOfRangeException();
#endif
            return Constants.CommonHeaderLength + (i * BlockSize);
        }

        private long LinearSearch(BinaryReader reader, int id, int startElement, int endElement) {
            reader.BaseStream.MoveToData(startElement * BlockSize);
            long endPosition = Constants.CommonHeaderLength + (endElement * BlockSize);

            while (reader.BaseStream.Position <= endPosition) {
                var foundId = reader.ReadInt32();
                if (foundId == id) {
                    return reader.ReadUInt32();
                }

                reader.BaseStream.Position += 4; //skip offset value
            }

            return -1;
        }

        /// <summary>
        /// Convert the reader to an in-memory map.
        /// </summary>
        public MemoryIdentifierIndex ToInMemory() {
            return new MemoryIdentifierIndex(ReadAll());
        }

    }

}
