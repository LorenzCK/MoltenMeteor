using System;

namespace MoltenMeteor {

    internal static class Constants {

        // Can it get more self-referential than this?
        public static byte[] MagicHeader = {
            (byte)'L',
            (byte)'C',
            (byte)'K',
        };

        public static byte LastVersion = (byte)1;

    }

}
