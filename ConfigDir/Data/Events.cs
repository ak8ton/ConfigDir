using System;

namespace ConfigDir.Data
{
    public partial class Finder
    {
        /// <summary>
        /// Value found error event
        /// </summary>
        public event ConfigEventHandler OnValueFound;

        /// <summary>
        /// Value not found error event
        /// </summary>
        public event ConfigEventHandler OnValueNotFound;

        /// <summary>
        /// Value type casting error event
        /// </summary>
        public event ConfigEventHandler OnValueTypeError;

        /// <summary>
        /// Validate value event
        /// </summary>
        public event ValidateEventHandler OnValidate;

        private void ValueFound(ConfigEventArgs args)
        {
            OnValueFound?.Invoke(args);
            Parent?.ValueFound(args);
        }

        private void ValueNotFound(ConfigEventArgs args)
        {
            OnValueNotFound?.Invoke(args);
            Parent?.ValueNotFound(args);
            throw new System.Exception("Значение не найдено\n" + args);
        }

        private void ValueTypeError(ConfigEventArgs args, Exception ex)
        {
            OnValueTypeError?.Invoke(args);
            Parent?.ValueTypeError(args, ex);
            throw new Exception("Не удалось привести значение к типу\n" + args, ex);
        }

        private void Validate(string key, object value)
        {
            OnValidate?.Invoke(key, value);
        }
    }
}
