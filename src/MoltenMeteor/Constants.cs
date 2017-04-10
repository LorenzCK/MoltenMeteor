using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    internal static class Constants {

        public static byte[] MagicHeader = {
            (byte)'L',
            (byte)'C',
            (byte)'K',
        };

        public static byte LastVersion = (byte)1;

    }

}
