﻿using System.Collections.Generic;

namespace ConfigDir
{
    /// <summary>
    /// Configuration source base interface
    /// </summary>
    public interface IArraySource : ISource
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
    }
}
