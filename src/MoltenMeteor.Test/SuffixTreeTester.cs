using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoltenMeteor.SuffixTree;

namespace MoltenMeteor.Test {

    public class SuffixTreeTester {

        [Test]
        public void GenerateMemorySuffixTree() {
            var m = new Meteor<string>(
                () => Common.OpenLocalFile("blob-strings.dat"),
                s => s.ReadToString()
            );

            Assert.AreEqual(m.Get(1), "Ciao");

            var tree = m.GenerateSuffixIndex(
                s => s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                s => s.ToLowerInvariant()
            );

            Assert.AreEqual("{'cia'=>{'o'=>{'$'=>[1,2],'ne$'=>[3]},'ffo$'=>[4]}}", tree.ToString());
        }

    }

}
