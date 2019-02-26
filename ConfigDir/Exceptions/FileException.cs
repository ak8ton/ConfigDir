using System;


namespace ConfigDir.Exceptions
{
    /// <summary>
    /// Any file IO errors
    /// </summary>
    public class FileException : ConfigException
    {
        internal FileException(string message, Exception inner) : base(message, inner) { }
    }
}
