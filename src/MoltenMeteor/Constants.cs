using System;

namespace MoltenMeteor {

    internal static class Constants {

        // Can it get more self-referential than this?
        public static byte[] MagicHeader = {
            (byte)'L',
            (byte)'C',
            (byte)'K',
        };

        public const byte LastVersion = (byte)1;

        public const long CommonHeaderLength = 20; // 20-byte header (3 magic, 1 version, 16 GUID)

    }

}
