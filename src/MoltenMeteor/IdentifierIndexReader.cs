using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    public class IdentifierIndexReader : IIdentifierIndex, IDisposable {

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

        private readonly BinaryReader _reader;

        public IdentifierIndexReader(Meteor owner, Stream input) {
            if (input == null)
                throw new ArgumentNullException();
            if (!input.CanRead || !input.CanSeek)
                throw new ArgumentException("Cannot read or seek stream");
            if (input.Length % BlockSize != 0)
                throw new ArgumentException("Malformed stream (wrong length)");

            Count = (int)(input.Length / BlockSize);

            (var reader, var version, var identifier) = input.ReadBinaryHeader();
            _reader = reader;

            // Safety check
            if(identifier != owner.BlobIdentifier) {
                throw new ArgumentException("Identifier index does not match owning blob file");
            }
        }

        /// <summary>
        /// Get number of elements in the index.
        /// </summary>
        public int Count { get; private set; }

        public long FindById(int id) {
            int a = 0;
            int b = Count - 1;

            while (b >= a) {
                if ((b - a) <= LinearSearchThreshold) {
                    return LinearSearch(id, a, b);
                }

                int middle = (b + a) / 2;

                _reader.BaseStream.Position = IndexToOffset(middle);
                int middleId = _reader.ReadInt32();

                if (middleId == id) {
                    return _reader.ReadInt32();
                }
                else if (id > middleId) {
                    a = middle + 1;
                }
                else {
                    b = middle - 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sequentially reads all entries in the ID index.
        /// </summary>
        public IEnumerable<(int id, long offset)> ReadAll() {
            _reader.BaseStream.MoveToData();

            while(_reader.BaseStream.Position < _reader.BaseStream.Length) {
                var id = _reader.ReadInt32();
                var offset = _reader.ReadUInt32();

                yield return (id, offset);
            }

            yield break;
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

        private long LinearSearch(int id, int startElement, int endElement) {
            _reader.BaseStream.MoveToData(startElement * BlockSize);
            long endPosition = Constants.CommonHeaderLength + (endElement * BlockSize);

            while (_reader.BaseStream.Position <= endPosition) {
                var foundId = _reader.ReadInt32();
                if (foundId == id) {
                    return _reader.ReadUInt32();
                }

                _reader.BaseStream.Position += 4; //skip offset value
            }

            return -1;
        }

        public void Dispose() {
            _reader?.Dispose();
        }

        /// <summary>
        /// Convert the reader to an in-memory map.
        /// </summary>
        public IdentifierIndex ToInMemory() {
            return new IdentifierIndex(ReadAll());
        }

    }

}
