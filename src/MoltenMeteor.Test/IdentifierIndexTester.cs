using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.Test {

    public class IdentifierIndexTester {

        [Test]
        public void GenerateAndTest() {
            var m = new Meteor(() => {
                return Common.OpenLocalFile("blob-123.dat");
            });

            var index = m.GenerateIdentifierIndex();

            Assert.AreEqual(3, index.Count);

            Assert.AreEqual(0x1c, index.FindById(1));
            Assert.AreEqual(0x25, index.FindById(2));
            Assert.AreEqual(0x2e, index.FindById(3));
        }

    }

}
