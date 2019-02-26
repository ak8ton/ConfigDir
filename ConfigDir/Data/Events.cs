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

        /// <summary>
        /// Validate value event
        /// </summary>
        public event ValidateEventHandler OnValidate;

        private void ValueFound(ConfigEventArgs args)
        {
            OnValueFound?.Invoke(args);
            Parent?.ValueFound(args);
        }

        private void ConfigError(ConfigException exception)
        {
            var args = new ConfigErrorEventArgs
            {
                Exception = exception
            };

            OnConfigError?.Invoke(args);
            Parent?.OnConfigError(args);

            if (args.Exception != null)
            {
                throw args.Exception;
            }

            throw exception;
        }

        private void Validate(string key, object value)
        {
            OnValidate?.Invoke(key, value);
        }
    }
}
