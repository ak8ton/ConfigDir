using System.Collections.Generic;

namespace ConfigDir.Data
{
    /// <summary>
    /// Config event options
    /// </summary>
    public class ConfigEventArgs
    {
        /// <summary>
        /// Path of value
        /// </summary>
        public string Path { get; internal set; } = "";

        /// <summary>
        /// Source of value
        /// </summary>
        public ISource Source { get; internal set; } = null;

        /// <summary>
        /// Typed value
        /// </summary>
        public object Value { get; internal set; } = null;

        /// <summary>
        /// Raw value
        /// </summary>
        public object RawValue { get; internal set; } = null;

        /// <summary>
        /// Expected type of value
        /// </summary>
        public System.Type ExpectedType { get; internal set; } = null;

        /// <summary>
        /// Get string representation of ConfigEventArgs
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var lines = new List<string>();

            if (Source != null)
            {
                lines.Add("Source: " + Source);
            }

            if (!string.IsNullOrWhiteSpace(Path))
            {
                lines.Add("Path: " + Path);
            }

            if (Value != null)
            {
                lines.Add("Value: " + Value + " - " + Value.GetType().FullName);
            }

            if (RawValue != null)
            {
                lines.Add("RawValue: " + RawValue + " - " + RawValue.GetType().FullName);
            }

            if (ExpectedType != null)
            {
                lines.Add("ExpectedType: " + ExpectedType.FullName);
            }

            return string.Join("\n", lines);
        }
    }
}
