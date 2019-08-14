using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigDir.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class KeyOrIndex
    {
        internal KeyOrIndex(string key)
        {
            Key = key;
        }

        internal KeyOrIndex(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Key string or null
        /// </summary>
        public string Key { get; } = null;

        /// <summary>
        /// Index od null
        /// </summary>
        public int? Index { get; } = null;

        /// <summary>
        /// String representation of key or index
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Key ?? Index?.ToString();
        }
    }
}
