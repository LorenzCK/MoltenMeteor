using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoltenMeteor.IdentifierIndex {

    /// <summary>
    /// Index on the blob that can find elements by index.
    /// </summary>
    public interface IIdentifierIndex {

        /// <summary>
        /// Get offset of element with given ID.
        /// </summary>
        /// <param name="id">ID of the element to find.</param>
        /// <returns>
        /// Byte offset of the found element.
        /// -1 if the element was not found.
        /// </returns>
        long FindById(int id);

        /// <summary>
        /// Get number of elements in the index.
        /// </summary>
        int Count { get; }

    }

}
