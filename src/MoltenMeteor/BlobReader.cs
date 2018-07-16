using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    /// <summary>
    /// Provides read access to data elements in the MoltenMeter blob.
    /// </summary>
    /// <remarks>
    /// This class is NOT thread-safe. Access to the input stream is not synchronized.
    /// </remarks>
    public class BlobReader : IDisposable {

        private readonly BinaryReader _reader;

        internal byte Version { get; private set; }
        internal Guid Identifier { get; private set; }

        public BlobReader(Stream input) {
            if (input == null || !input.CanRead)
                throw new ArgumentException("Invalid stream, cannot be read", nameof(input));

            (_reader, Version, Identifier) = input.ReadBinaryHeader();
        }

        public void Dispose() {
            _reader.Dispose();
        }

        private (int id, uint length) ReadAtCurrent() {
            var id = _reader.ReadInt32();
            var length = _reader.ReadUInt32();
            if (length > int.MaxValue)
                throw new ArgumentException("Blob field too large");

            return (id, length);
        }

        /// <summary>
        /// Reads ID from an offset.
        /// </summary>
        public int ReadId(long offset) {
            _reader.BaseStream.Position = offset;

            return _reader.ReadInt32();
        }

        /// <summary>
        /// Reads a data field from an offset as a byte array.
        /// </summary>
        /// <remarks>
        /// Data is copied from the original stream.
        /// </remarks>
        public (int id, byte[] data) ReadAsArray(long offset) {
            _reader.BaseStream.Position = offset;

            (var id, var length) = ReadAtCurrent();
            var data = _reader.ReadBytes((int)length);

            return (id, data);
        }

        /// <summary>
        /// Reads a data field from an offset as an in-memory stream.
        /// </summary>
        /// <remarks>
        /// Data is not copied from the original stream. The returned data
        /// stream is a view on the original stream and access is not synchronized.
        /// </remarks>
        public (int id, Stream data) ReadAsStream(long offset) {
            _reader.BaseStream.Position = offset;

            (var id, var length) = ReadAtCurrent();

            return (id, new SubReadOnlyStream(_reader.BaseStream, _reader.BaseStream.Position, length));
        }

        /// <summary>
        /// Reads a data field from an offset and returns an unsafe stream.
        /// </summary>
        /// <remarks>
        /// Data is not copied from the original stream. The original stream (moved to
        /// the data field start position) is returned.
        /// </remarks>
        internal (int id, uint length, Stream stream) ReadAsStreamUnsafe(long offset) {
            _reader.BaseStream.Position = offset;

            (var id, var length) = ReadAtCurrent();

            return (id, length, _reader.BaseStream);
        }

        /// <summary>
        /// Reads all available data blocks in the blob.
        /// </summary>
        /// <remarks>
        /// The underlying stream's position must not be moved during enumeration.
        /// </remarks>
        public IEnumerable<(int id, Stream data)> ReadAll() {
            _reader.BaseStream.MoveToData();

            while (_reader.BaseStream.Position < _reader.BaseStream.Length) {
                (var id, var length) = ReadAtCurrent();
                long nextOffset = _reader.BaseStream.Position + length;
                yield return (id, new SubReadOnlyStream(_reader.BaseStream, _reader.BaseStream.Position, length));

                _reader.BaseStream.Position = nextOffset;
            }

            yield break;
        }

        /// <summary>
        /// Reads all data blocks in the blob and returns their offsets.
        /// </summary>
        /// <remarks>
        /// The underlying stream's position must not be moved during enumeration.
        /// </remarks>
        internal IEnumerable<(int id, long offset)> ReadAllOffsets() {
            _reader.BaseStream.MoveToData();

            while (_reader.BaseStream.Position < _reader.BaseStream.Length) {
                (var id, var length) = ReadAtCurrent();
                yield return (id, _reader.BaseStream.Position - 8);

                _reader.BaseStream.Position += length;
            }

            yield break;
        }

    }

}
