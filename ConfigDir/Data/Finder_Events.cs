using System;
using ConfigDir.Exceptions;

namespace ConfigDir.Data
{
    public partial class Finder
    {
        /// <summary>
        /// Value found error event
        /// </summary>
        public event ConfigEventHandler OnValueFound;

        /// <summary>
        /// Any config error event
        /// </summary>
        public event ConfigErrorEventHandler OnConfigError;

        internal event ValidateEventHandler OnValidate;

        private void ValueFound(ConfigEventArgs args)
        {
            OnValueFound?.Invoke(args);
            Parent?.ValueFound(args);
        }

        private void ConfigError(ConfigErrorEventArgs args)
        {
            OnConfigError?.Invoke(args);
            Parent?.ConfigError(args);
        }

        private void Validate(string key, object value)
        {
            OnValidate?.Invoke(key, value);
        }
    }
}
