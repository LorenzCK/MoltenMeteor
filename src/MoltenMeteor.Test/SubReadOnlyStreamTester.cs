using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.Test {

    class SubReadOnlyStreamTester {

        const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        private Stream GenerateAlphabet() {
            var bytes = Encoding.UTF8.GetBytes(Alphabet);
            return new MemoryStream(bytes);
        }

        private SubReadOnlyStream GenerateReader(int offset, int length) {
            var s = GenerateAlphabet();
            return new SubReadOnlyStream(s, offset, length);
        }

        [Test]
        public void SimpleRead() {
            var reader = GenerateReader(0, Alphabet.Length);

            Assert.AreEqual(Alphabet.Length, reader.Length);

            var bytes = new byte[reader.Length];
            reader.Read(bytes, 0, (int)reader.Length);
            var reconstruct = Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(Alphabet, reconstruct);
        }

        [Test]
        public void SliceReadBeginning() {
            var reader = GenerateReader(0, 4);

            Assert.AreEqual(reader.Length, 4);

            var bytes = new byte[reader.Length];
            reader.Read(bytes, 0, (int)reader.Length);
            var reconstruct = Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(Alphabet.Substring(0, 4), reconstruct);
        }

        [Test]
        public void SliceReadMiddle() {
            var reader = GenerateReader(4, 6);

            Assert.AreEqual(reader.Length, 6);

            var bytes = new byte[reader.Length];
            reader.Read(bytes, 0, (int)reader.Length);
            var reconstruct = Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(Alphabet.Substring(4, 6), reconstruct);
        }

        [Test]
        public void SliceReadEnd() {
            var reader = GenerateReader(25, 1);

            Assert.AreEqual(reader.Length, 1);

            char ending = (char)reader.ReadByte();
            Assert.AreEqual(ending, 'z');
        }

        [Test]
        public void SliceReadOver() {
            var reader = GenerateReader(25, 1);

            Assert.AreEqual(reader.Length, 1);

            var bytes = new byte[2];
            int readBytes = reader.Read(bytes, 0, 2);
            Assert.AreEqual(readBytes, 1);
            Assert.AreEqual((char)bytes[0], 'z');
            Assert.AreEqual(bytes[1], 0);
        }

        [Test]
        public void SliceReadLimited() {
            var reader = GenerateReader(2, 1);

            Assert.AreEqual(reader.Length, 1);

            var bytes = new byte[2];
            int readBytes = reader.Read(bytes, 0, 2);
            Assert.AreEqual(readBytes, 1);
            Assert.AreEqual((char)bytes[0], 'c');
            Assert.AreEqual(bytes[1], 0);
        }

        [Test]
        public void SliceTooLong() {
            var s = GenerateAlphabet();
            Assert.Catch<ArgumentOutOfRangeException>(() => {
                var reader = new SubReadOnlyStream(s, 25, 10);
            });
        }

        [Test]
        public void Seeking() {
            var reader = GenerateReader(1, 10);

            // From current
            Assert.AreEqual('b', (char)reader.ReadByte());
            reader.Seek(2, SeekOrigin.Current);
            Assert.AreEqual('e', (char)reader.ReadByte());
            reader.Seek(-1, SeekOrigin.Current);
            Assert.AreEqual('e', (char)reader.ReadByte());

            // From end
            reader.Seek(-1, SeekOrigin.End);
            Assert.AreEqual('k', (char)reader.ReadByte());
            reader.Seek(-reader.Length, SeekOrigin.End);
            Assert.AreEqual('b', (char)reader.ReadByte());

            // From begin
            reader.Seek(3, SeekOrigin.Begin);
            Assert.AreEqual('e', (char)reader.ReadByte());
            reader.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual('b', (char)reader.ReadByte());

            Assert.Catch<IOException>(() => {
                reader.Seek(-10, SeekOrigin.Current);
            });
            Assert.Catch<IOException>(() => {
                reader.Seek(-1, SeekOrigin.Begin);
            });
            Assert.Catch<IOException>(() => {
                reader.Seek(-11, SeekOrigin.End);
            });
        }

    }

}
