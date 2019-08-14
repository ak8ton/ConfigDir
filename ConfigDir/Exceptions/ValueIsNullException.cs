using ConfigDir.Data;
using ConfigDir.Internal;
using System;
using System.Text;


namespace ConfigDir.Exceptions
{
    /// <summary>
    /// Config value not found
    /// </summary>
    public class ValueIsNullException : ConfigException
    {
        /// <summary>
        /// Path where error occurred
        /// </summary>
        public string ErrorPath => ep ?? (ep = ErrorFinder.GetPath(ErrorKey.ToString()));
        string ep = null;

        /// <summary>
        /// Source where error occurred
        /// </summary>
        public ISource ErrorSource { get; internal set; }

        internal Finder ErrorFinder { get; set; }
        internal KeyOrIndex ErrorKey { get; set; }

        internal ValueIsNullException(ValueOrSource value) :
            this("Value is null", value, null) { }

        internal ValueIsNullException(string messae, ValueOrSource value, Exception inner) : 
            base(messae, inner)
        {
            ErrorKey = value.KeyOrIndex;
            ErrorFinder = value.Finder;
            ErrorSource = value.Source;
        }

        internal override string GetDetails()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(ErrorPath))
            {
                sb.Append("ErrorPath: ");
                sb.AppendLine(ErrorPath);
            }

            var src = ErrorSource?.ToString();
            if (!string.IsNullOrWhiteSpace(src))
            {
                sb.Append("ErrorSource: ");
                sb.AppendLine(src);
            }

            return sb.ToString();
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("ErrorType: ValueIsNullException");
            sb.AppendLine(GetDetails());
            sb.Append(base.GetDetails());

            if (!string.IsNullOrWhiteSpace(Message))
            {
                sb.Append("Message: ");
                sb.AppendLine(Message);
            }

            return sb.ToString();
        }
    }
}
