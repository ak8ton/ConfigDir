using ConfigDir.Exceptions;

namespace ConfigDir.Data
{
    /// <summary>
    /// Error details
    /// </summary>
    public class ConfigErrorEventArgs
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
