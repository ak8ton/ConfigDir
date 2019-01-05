using System.Collections.Generic;

namespace ConfigDir.Data
{
    public class ConfigEventArgs
    {
        /// <summary>
        /// Путь
        /// </summary>
        public string Path { get; internal set; } = "";

        /// <summary>
        /// Источник
        /// </summary>
        public ISource Source { get; internal set; } = null;

        /// <summary>
        /// Значение с требуемым типом
        /// </summary>
        public object Value { get; internal set; } = null;

        /// <summary>
        /// Значение с исходным типом
        /// </summary>
        public object RawValue { get; internal set; } = null;

        /// <summary>
        /// Тип значения
        /// </summary>
        public System.Type ExpectedType { get; internal set; } = null;

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
