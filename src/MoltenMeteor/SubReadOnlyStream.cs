using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    /// <summary>
    /// Read-only view on a stream's subsection.
    /// </summary>
    public class SubReadOnlyStream : Stream {

        private readonly Stream _parent;
        private readonly long _parentStart;
        private readonly long _length;
        private long _parentPosition;

        public SubReadOnlyStream(Stream parent, long startOffset, long length) {
            if (startOffset + length > parent.Length)
                throw new ArgumentOutOfRangeException("Parent stream is not long enough");

            _parent = parent;
            _parentStart = startOffset;
            _length = length;
            _parentPosition = startOffset;
        }

        public override bool CanRead => _parent.CanRead;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _length;

        public override long Position {
            get => _parentPosition - _parentStart;
            set {
                if(value < 0 || value > (_length)) {
                    throw new ArgumentOutOfRangeException();
                }

                _parentPosition = (_parentStart + value);
            }
        }

        public override void Flush() {
            // Nop
        }

        public override int Read(byte[] buffer, int offset, int count) {
            int maxCount = (int)(_length - (_parentPosition - _parentStart));
            if (count > maxCount)
                count = maxCount;

            _parent.Position = _parentPosition;
            int effectiveRead = _parent.Read(buffer, offset, count);
            _parentPosition = _parent.Position;

            return effectiveRead;
        }

        private long GetParentPosition(SeekOrigin origin) {
            switch(origin) {
                default:
                    throw new ArgumentException("Unknown seek origin", nameof(origin));
                case SeekOrigin.Begin:
                    return _parentStart;
                case SeekOrigin.Current:
                    return _parentPosition;
                case SeekOrigin.End:
                    return _parentStart + _length;
            }
        }

        public override long Seek(long offset, SeekOrigin origin) {
            var destination = GetParentPosition(origin) + offset;
            if (destination < _parentStart)
                throw new IOException("Cannot seek to negative position");

            _parentPosition = destination;
            return _parentPosition;
        }

        public override void SetLength(long value) {
            throw new InvalidOperationException("Cannot change length of read-only stream");
        }

        public override void Write(byte[] buffer, int offset, int count) {
            throw new InvalidOperationException("Cannot write to read-only stream");
        }
    }

}
