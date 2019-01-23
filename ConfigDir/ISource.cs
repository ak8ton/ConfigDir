using System.Collections.Generic;

namespace ConfigDir
{
    /// <summary>
    /// Configuration source base interface
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Source description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Get all values by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<object> GetAllValues(string key);
    }
}
