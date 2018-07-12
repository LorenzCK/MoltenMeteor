using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor {

    public class Meteor<T> : Meteor {

        private readonly Func<Stream, T> _converter;

        public Meteor(StreamOpener opener, Func<Stream, T> converter)
            : base(opener) {

            _converter = converter;
        }

        public T Get(int id) {
            if(IdentifierIndex == null) {
                GenerateIdentifierIndex();
            }

            using (var reader = new BlobReader(OpenBlobStream())) {
                long offset = IdentifierIndex.FindById(id);
                if (offset < 0)
                    throw new ArgumentException("Element not found", nameof(id));

                (var outId, var data) = reader.ReadAsStream(offset);

#if DEBUG
                if (outId != id)
                    throw new InvalidOperationException($"Loading data for element #{id} returned data for #{outId}");
#endif

                return _converter(data);
            }
        }

    }

}
