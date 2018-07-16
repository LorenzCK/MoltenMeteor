using MoltenMeteor.IdentifierIndex;
using NUnit.Framework;

namespace MoltenMeteor.Test {

    public class IdentifierIndexTester {

        private Meteor<int> CreateMeteor() {
            return new Meteor<int>(
                () => Common.OpenLocalFile("blob-123.dat"),
                (s) => (int)s.ReadBinaryByte()
            );
        }

        [Test]
        public void GenerateAndTestInMemory() {
            var m = CreateMeteor();
            var index = m.GenerateIdentifierIndex();

            // Test index length
            Assert.AreEqual(3, index.Count);

            // Test index offsets
            Assert.AreEqual(0x14, index.FindById(1));
            Assert.AreEqual(0x1d, index.FindById(2));
            Assert.AreEqual(0x26, index.FindById(3));

            // Test loaded entries
            Assert.AreEqual((byte)'1', m.Get(1));
            Assert.AreEqual((byte)'2', m.Get(2));
            Assert.AreEqual((byte)'3', m.Get(3));
        }

        [Test]
        public void TestFromStream() {
            var m = CreateMeteor();
            var index = m.LoadIdentifierIndex(
                () => Common.OpenLocalFile("blob-123-identifiers.dat")
            );

            // Test index length
            Assert.AreEqual(3, index.Count);

            // Test index offsets
            Assert.AreEqual(0x14, index.FindById(1));
            Assert.AreEqual(0x1d, index.FindById(2));
            Assert.AreEqual(0x26, index.FindById(3));

            // Test loaded entries
            Assert.AreEqual((byte)'1', m.Get(1));
            Assert.AreEqual((byte)'2', m.Get(2));
            Assert.AreEqual((byte)'3', m.Get(3));
        }

    }

}
