using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.Test {

    public class MeteorTester {

        [Test]
        public void ReadMeteorOfT() {
            var m = new Meteor<int>(
                () => Common.OpenLocalFile("blob-123.dat"),
                (s) => {
                    return Convert.ToInt32(s.ReadToString());
                }
            );

            Assert.AreEqual(1, m.Get(1));
            Assert.AreEqual(2, m.Get(2));
            Assert.AreEqual(3, m.Get(3));
            Assert.AreNotEqual(2, m.Get(1));
        }

    }

}
