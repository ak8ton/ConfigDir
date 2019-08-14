using System.Collections.Generic;

namespace ConfigDir
{
    /// <summary>
    /// Configuration source base interface
    /// </summary>
    public interface IConfigSource : ISource
    {
        /// <summary>
        /// Get all values by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<object> GetAllValues(string key);

        /// <summary>
        /// All keys
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetKeys();
    }
}
