using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.Test {

    public class IdentifierIndexTester {

        [Test]
        public void GenerateAndTestInMemory() {
            var m = new Meteor(() => {
                return Common.OpenLocalFile("blob-123.dat");
            });

            var index = m.GenerateIdentifierIndex();

            Assert.AreEqual(3, index.Count);

            Assert.AreEqual(0x14, index.FindById(1));
            Assert.AreEqual(0x1d, index.FindById(2));
            Assert.AreEqual(0x26, index.FindById(3));

            Assert.AreEqual((byte)'1', m.GetRaw(1)[0]);
            Assert.AreEqual((byte)'2', m.GetRaw(2)[0]);
            Assert.AreEqual((byte)'3', m.GetRaw(3)[0]);
        }

        [Test]
        public void TestFromStream() {
            var m = new Meteor(() => {
                return Common.OpenLocalFile("blob-123.dat");
            });

            var index = m.LoadIdentifierIndex(() => {
                return Common.OpenLocalFile("blob-123-identifiers.dat");
            });

            Assert.AreEqual(3, index.Count);

            Assert.AreEqual(0x14, index.FindById(1));
            Assert.AreEqual(0x1d, index.FindById(2));
            Assert.AreEqual(0x26, index.FindById(3));

            Assert.AreEqual((byte)'1', m.GetRaw(1)[0]);
            Assert.AreEqual((byte)'2', m.GetRaw(2)[0]);
            Assert.AreEqual((byte)'3', m.GetRaw(3)[0]);
        }

    }

}
