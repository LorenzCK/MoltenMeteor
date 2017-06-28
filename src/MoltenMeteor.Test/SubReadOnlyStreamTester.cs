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
            var s = GenerateAlphabet();
            var reader = new SubReadOnlyStream(s, 0, 4);

            Assert.AreEqual(reader.Length, 4);

            var bytes = new byte[reader.Length];
            reader.Read(bytes, 0, (int)reader.Length);
            var reconstruct = Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(Alphabet.Substring(0, 4), reconstruct);
        }

        [Test]
        public void SliceReadMiddle() {
            var s = GenerateAlphabet();
            var reader = new SubReadOnlyStream(s, 4, 6);

            Assert.AreEqual(reader.Length, 6);

            var bytes = new byte[reader.Length];
            reader.Read(bytes, 0, (int)reader.Length);
            var reconstruct = Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(Alphabet.Substring(4, 6), reconstruct);
        }

        [Test]
        public void SliceReadEnd() {
            var s = GenerateAlphabet();
            var reader = new SubReadOnlyStream(s, 25, 1);

            Assert.AreEqual(reader.Length, 1);

            char ending = (char)reader.ReadByte();
            Assert.AreEqual(ending, 'z');
        }

        [Test]
        public void SliceReadOver() {
            var s = GenerateAlphabet();
            var reader = new SubReadOnlyStream(s, 25, 1);

            Assert.AreEqual(reader.Length, 1);

            var bytes = new byte[2];
            int readBytes = reader.Read(bytes, 0, 2);
            Assert.AreEqual(readBytes, 1);
            Assert.AreEqual((char)bytes[0], 'z');
            Assert.AreEqual(bytes[1], 0);
        }

        [Test]
        public void SliceReadLimited() {
            var s = GenerateAlphabet();
            var reader = new SubReadOnlyStream(s, 2, 1);

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

    }

}
