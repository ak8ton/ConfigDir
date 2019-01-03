using System;
using System.Collections.Generic;
using ConfigDir.Internal;


namespace ConfigDir
{
    /// <summary>
    /// Базовый класс привязки
    /// </summary>
    public abstract partial class Config : IConfig
    {
        public string Description { get; set; } = "";

        private readonly Dictionary<string, object> cash = new Dictionary<string, object>();

        public string[] Keys { get; private set; }

        public virtual void Validate(string key, object value)
        {
        }

        public TValue GetValue<TValue>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (cash.ContainsKey(key))
            {
                if (cash[key] is TValue v)
                {
                    return v;
                }
                else
                {
                    throw new Exception();
                }
            }

            TValue value = FindValue<TValue>(key);
            cash[key] = value;

            return value;
        }

        public void SetValue(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentException(nameof(value));
            }

            cash[key] = value;
        }

        private TValue FindValue<TValue>(string key)
        {
            switch (TypeInspector.GetTypeCategory(typeof(TValue)))
            {
                case TypeCategory.None:
                    throw new ArgumentException("GetValue<TValue>(string key)");

                case TypeCategory.Primitive:
                    return FindPrimitiveValue<TValue>(key);

                case TypeCategory.Config:
                    return CreateSubConfig<TValue>(key);

                default:
                    throw new NotImplementedException("GetValue<TValue>(string key)");
            }
        }

        private TValue FindPrimitiveValue<TValue>(string key)
        {
            foreach (var value in FindAllValues(key))
            {
                if (value.Type == ValueOrSourceType.value)
                {
                    var v = (TValue)Convert.ChangeType(value.Value, typeof(TValue));
                    Validate(key, v);
                    return v;
                }
                throw new Exception();
            }

            throw new Exception();
        }

    }
}
