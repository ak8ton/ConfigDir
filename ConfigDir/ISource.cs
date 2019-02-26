using System.Collections.Generic;

namespace ConfigDir
{
    /// <summary>
    /// Configuration source base interface
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Get all values by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<object> GetAllValues(string key);

        /// <summary>
        /// String representation
        /// </summary>
        string ToString();
    }
}
