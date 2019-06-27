using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDir
{
    /// <summary>
    /// Configuration source base interface
    /// </summary>
    public interface IArraySource
    {
        /// <summary>
        /// Get all values by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEnumerable<object> GetAllValues(int index);

        /// <summary>
        /// Count
        /// </summary>
        /// <returns></returns>
        int GetCount();

        /// <summary>
        /// String representation
        /// </summary>
        string ToString();
    }
}
