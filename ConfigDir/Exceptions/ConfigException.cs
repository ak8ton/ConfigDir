using System;
using System.Text;
using ConfigDir.Data;


namespace ConfigDir.Exceptions
{
    /// <summary>
    /// Base Config error exception class
    /// </summary>
    public class ConfigException : Exception
    {
        /// <summary>
        /// Requested path
        /// </summary>
        public string RequestedPath => rp ?? (rp = RequestedFinder.GetPath(RequestedKey));
        string rp = null;

        internal Finder RequestedFinder { get; set; }
        internal string RequestedKey { get; set; }

        internal ConfigException(string message) : this(message, null) { }
        internal ConfigException(string message, Exception inner) : base(message, inner) { }

        internal virtual string GetDetails()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(RequestedPath))
            {
                sb.Append("RequestedPath: ");
                sb.AppendLine(RequestedPath);
            }

            return sb.ToString();
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("ErrorType: ConfigException");

            if (!string.IsNullOrWhiteSpace(Message))
            {
                sb.Append("Message: ");
                sb.AppendLine(Message);
            }

            sb.Append(GetDetails());
            return sb.ToString();
        }
    }
}
