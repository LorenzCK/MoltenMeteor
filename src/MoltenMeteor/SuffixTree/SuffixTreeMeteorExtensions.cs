using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.SuffixTree {

    public static class SuffixTreeMeteorExtensions {

        public static MemorySuffixTreeIndex GenerateSuffixIndex(this Meteor meteor,
            Func<Stream, string[]> keyToSuffixes = null,
            Func<string, string> suffixPreprocessor = null) {

            var tree = new MemorySuffixTreeIndex();

            using (var reader = new BlobReader(meteor.OpenBlobStream())) {
                foreach (var (id, data) in reader.ReadAll()) {
                    var suffixes = keyToSuffixes(data);
                    foreach(var suffix in suffixes) {
                        tree.AddSuffix(suffixPreprocessor(suffix), id);
                    }
                }
            }

            return tree;
        }

    }

}
