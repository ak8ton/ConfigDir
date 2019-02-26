using System;
using System.Text;


namespace ConfigDir.Exceptions
{
    /// <summary>
    /// Config value not found
    /// </summary>
    public class ValueNotFoundException : ConfigException
    {
        internal ValueNotFoundException() : base("Value not found") { }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("ErrorType: ValueNotFoundException");

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
