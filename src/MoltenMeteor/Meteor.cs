using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    /// <summary>
    /// Represents a single database unit, composed of one binary blob and any
    /// number of indexes on it, that can be read.
    /// </summary>
    public class Meteor {

        public delegate Stream StreamOpener();

        private readonly StreamOpener _opener;

        public Meteor(StreamOpener opener) {
            _opener = opener ?? throw new ArgumentNullException(nameof(opener));

            using(var r = new Reader(BlobStream)) {
                BlobIdentifier = r.Identifier;
                BlobVersion = r.Version;
            }
        }

        protected Stream BlobStream {
            get {
                return _opener();
            }
        }

        /// <summary>
        /// Gets the binary blob's unique identifier.
        /// </summary>
        public Guid BlobIdentifier { get; private set; }

        /// <summary>
        /// Gets the binary blob's data format version.
        /// </summary>
        public int BlobVersion { get; private set; }

    }

}
