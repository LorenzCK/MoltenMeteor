using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This class is NOT thread-safe. Access to the input stream is not synchronized.
    /// </remarks>
    public class Reader : IDisposable {

        private readonly BinaryReader _reader;

        internal byte Version { get; private set; }
        internal Guid Identifier { get; private set; }

        public Reader(Stream inputStream) {
            if (inputStream == null || !inputStream.CanRead)
                throw new ArgumentException("Invalid stream, cannot be read", nameof(inputStream));

            _reader = new BinaryReader(inputStream);
            _reader.BaseStream.Position = 0;

            var header = _reader.ReadBytes(3);
            if(!header.SequenceEqual(Constants.MagicHeader)) {
                throw new ArgumentException("Stream does not contain valid header", nameof(inputStream));
            }

            Version = _reader.ReadByte();
            if(Version != Constants.LastVersion) {
                throw new ArgumentException($"Stream encoded using unsupported Molten Meteor v.{Version}");
            }

            Identifier = new Guid(_reader.ReadBytes(16));
        }

        public void Dispose() {
            _reader.Dispose();
        }

        /// <summary>
        /// Reads a data field from an offset as a byte array.
        /// </summary>
        /// <remarks>
        /// Data is copied from the original stream.
        /// </remarks>
        public (int id, byte[] data) ReadAsArray(long offset) {
            _reader.BaseStream.Position = offset;

            var id = _reader.ReadInt32();
            var length = _reader.ReadUInt32();
            if (length > int.MaxValue)
                throw new ArgumentException("Blob field too large");
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

            var id = _reader.ReadInt32();
            var length = _reader.ReadUInt32();
            if (length > int.MaxValue)
                throw new ArgumentException("Blob field too large");

            return (id, new SubReadOnlyStream(_reader.BaseStream, _reader.BaseStream.Position, length));
        }

    }

}
