namespace ConfigDir.Data
{
    public partial class Finder
    {
        public event ConfigEventHandler OnValueFound;
        public event ConfigEventHandler OnValueNotFound;
        public event ConfigEventHandler OnValueTypeError;
        public event ValidateEventHandler OnValidate;

        private void ValueFound()
        {
            OnValueFound?.Invoke(null);
            Parent?.ValueFound();
        }

        private void ValueNotFound()
        {
            OnValueNotFound?.Invoke(null);
            Parent?.ValueNotFound();
        }

        private void ValueTypeError()
        {
            OnValueTypeError?.Invoke(null);
            Parent?.ValueTypeError();
        }

        private void Validate(string key, object value)
        {
            OnValidate?.Invoke(key, value);
        }
    }
}
