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

        private string AssemblyDir {
            get => Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).AbsolutePath));
        }

        private string ReadStream(Stream s) {
            using(var sr = new StreamReader(s)) {
                return sr.ReadToEnd();
            }
        }

        [Test]
        public void ReadAll123() {
            using(var fs = new FileStream(Path.Combine(AssemblyDir, "blob-123.dat"), FileMode.Open)) {
                using(var reader = new BlobReader(fs)) {
                    var data = reader.ReadAll().ToArray();

                    Assert.AreEqual(3, data.Length);

                    Assert.AreEqual(1, data[0].id);
                    Assert.AreEqual(2, data[1].id);
                    Assert.AreEqual(3, data[2].id);

                    Assert.AreEqual("1", ReadStream(data[0].data));
                    Assert.AreEqual("2", ReadStream(data[1].data));
                    Assert.AreEqual("3", ReadStream(data[2].data));
                }
            }
        }

    }

}
