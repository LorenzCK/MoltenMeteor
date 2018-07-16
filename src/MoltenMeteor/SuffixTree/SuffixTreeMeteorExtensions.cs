using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.SuffixTree {

    public static class SuffixTreeMeteorExtensions {

        public static MemorySuffixTreeIndex GenerateSuffixIndex<T>(this Meteor<T> meteor,
            Func<T, string[]> keyToSuffixes = null,
            Func<string, string> suffixPreprocessor = null) {

            var tree = new MemorySuffixTreeIndex();
            using(var reader = new BlobReader(meteor.OpenBlobStream())) {
                foreach(var rawElement in reader.ReadAll()) {
                    var element = meteor.ReadEntry(rawElement.data);

                    var suffixes = keyToSuffixes(element);
                    foreach (var suffix in suffixes) {
                        tree.AddSuffix(suffixPreprocessor(suffix), rawElement.id);
                    }
                }
            }

            return tree;
        }

    }

}
