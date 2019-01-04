using System;
using System.Collections.Generic;
using ConfigDir.Internal;


namespace ConfigDir
{
    public delegate void ConfigEventHandler(ConfigEventArgs args);

    public class ConfigEventArgs
    {

    }

    public abstract partial class Config
    {
        public event ConfigEventHandler OnValueFound;
        public event ConfigEventHandler OnValueNotFound;
        public event ConfigEventHandler OnValueTypeError;

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
    }
}
