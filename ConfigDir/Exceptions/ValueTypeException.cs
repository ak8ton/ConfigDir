using System;
using System.Text;
using ConfigDir.Data;
using ConfigDir.Internal;

namespace ConfigDir.Exceptions
{
    /// <summary>
    /// Represents errors that occur when config option has incorrect value
    /// </summary>
    public class ValueTypeException : ValueIsNullException
    {
        /// <summary>
        /// Error value
        /// </summary>
        public object ErrorValue { get; }

        /// <summary>
        /// Required value type
        /// </summary>
        public Type RequiredType { get; }

        internal ValueTypeException(string message, ValueOrSource value, Type requiredType) :
            this(message, value, requiredType, null)
        { }

        internal ValueTypeException(string message, ValueOrSource value, Type requiredType, Exception inner) : 
            base(message, value, inner)
        {
            ErrorValue = value.Value;
            RequiredType = requiredType;
        }

        internal override string GetDetails()
        {
            var sb = new StringBuilder();

            var val = ErrorValue?.ToString();
            if (!string.IsNullOrWhiteSpace(val))
            {
                sb.Append("ErrorValue: ");
                sb.AppendLine(val);
            }

            var type = RequiredType?.Name;
            if (!string.IsNullOrWhiteSpace(type))
            {
                sb.Append("RequiredType: ");
                sb.AppendLine(type);
            }

            return sb.ToString();
        }

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

            sb.AppendLine(GetDetails());
            sb.Append(base.GetDetails());

            return sb.ToString();
        }
    }
}
