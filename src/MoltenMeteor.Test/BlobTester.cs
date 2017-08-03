using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.Test {

    public class BlobTester {

        [Test]
        public void ReadAll123() {
            using (var fs = Common.OpenLocalFile("blob-123.dat")) {
                using (var reader = new BlobReader(fs)) {
                    var data = reader.ReadAll().ToArray();

                    Assert.AreEqual(3, data.Length);

                    Assert.AreEqual(1, data[0].id);
                    Assert.AreEqual(2, data[1].id);
                    Assert.AreEqual(3, data[2].id);

                    Assert.AreEqual("1", data[0].data.ReadToString());
                    Assert.AreEqual("2", data[1].data.ReadToString());
                    Assert.AreEqual("3", data[2].data.ReadToString());
                }
            }
        }

    }

}
