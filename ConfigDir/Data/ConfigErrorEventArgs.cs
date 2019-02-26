using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigDir.Exceptions;

namespace ConfigDir.Data
{
    public class ConfigErrorEventArgs : ConfigEventArgs
    {
        /// <summary>
        /// Exception
        /// </summary>
        public ConfigException Exception { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Exception?.ToString();
        }
    }
}
