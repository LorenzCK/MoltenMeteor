using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.Test {

    static class Common {

        public static string AssemblyDir {
            get => Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).AbsolutePath));
        }

        public static Stream OpenLocalFile(string filename) {
            return new FileStream(Path.Combine(AssemblyDir, filename), FileMode.Open);
        }

        public static string ReadToString(this Stream s) {
            using (var sr = new StreamReader(s)) {
                return sr.ReadToEnd();
            }
        }

    }

}
