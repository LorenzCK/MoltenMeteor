using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    internal static class HeaderExtensions {

        /// <summary>
        /// Creates a <see cref="BinaryReader"/> and reads the common header
        /// of MoltenMeteor files.
        /// </summary>
        /// <returns>
        /// Tuple with binary reader (moved to position after the common header),
        /// integer version, and unique identifier of the database.
        /// </returns>
        public static (BinaryReader reader, byte version, Guid id) ReadBinaryHeader(this Stream input) {
            var reader = new BinaryReader(input);
            reader.BaseStream.Position = 0;

            var header = reader.ReadBytes(3);
            if (!header.SequenceEqual(Constants.MagicHeader)) {
                throw new ArgumentException("Stream does not contain valid header", nameof(input));
            }

            var version = reader.ReadByte();
            if (version != Constants.LastVersion) {
                throw new ArgumentException($"Stream encoded using unsupported Molten Meteor v.{version}");
            }

            var identifier = new Guid(reader.ReadBytes(16));

            return (reader, version, identifier);
        }

        /// <summary>
        /// Moves the stream to the position just after the common header
        /// (i.e., where the data portion starts).
        /// </summary>
        public static void MoveToData(this Stream s) {
            s.Position = Constants.CommonHeaderLength;
        }

        /// <summary>
        /// Moves the stream to a position after the common header
        /// (i.e., where the data portion starts).
        /// </summary>
        /// <param name="offset">Offset of the data portion where to move.</param>
        public static void MoveToData(this Stream s, long offset) {
            s.Position = Constants.CommonHeaderLength + offset;
        }

    }

}
