using System.Collections.Generic;

namespace ConfigDir.Data
{
    /// <summary>
    /// Config event options
    /// </summary>
    public class ConfigEventArgs
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Typed value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Raw value
        /// </summary>
        public object RawValue { get; set; }
        
        /// <summary>
        /// Source of value
        /// </summary>
        public ISource Source { get; set; }

        /// <summary>
        /// Finder object
        /// </summary>
        public Finder Finder { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        public string Path
        {
            get => _path ?? (_path = Finder?.GetPath(Key));
        }
        string _path = null;

        /// <summary>
        /// Get string representation of ConfigEventArgs
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var newLine = "\n";
            var lines = new List<string>();

            if (Source != null)
            {
                lines.Add("Source: " + Source.ToString());
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

            var summary = Finder?.GetSummary(Key);
            if (!string.IsNullOrWhiteSpace(summary))
            {
                lines.Add("Summary: " + summary);
            }

            return string.Join(newLine, lines);
        }
    }
}
