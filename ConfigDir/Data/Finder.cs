﻿using ConfigDir.Exceptions;
using ConfigDir.Internal;
using System;
using System.Collections.Generic;


namespace ConfigDir.Data
{
    /// <summary>
    /// Config values finder
    /// </summary>
    public partial class Finder
    {
        private readonly List<ISource> deck = new List<ISource>();
        private readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        /// <summary>
        /// Config type
        /// </summary>
        public Type ConfigType { get; }

        /// <summary>
        /// Farent Finder
        /// </summary>
        public Finder Parent { get; }

        /// <summary>
        /// Key of Finder in parent Finder
        /// </summary>
        public string Key { get; }

        internal Finder(Type configType, string key, Finder parent)
        {
            Parent = parent;
            ConfigType = configType;
            Key = key;
        }

        /// <summary>
        /// Get value by key and type
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetValue<TValue>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (cache.ContainsKey(key))
            {
                if (cache[key] is TValue v)
                {
                    return v;
                }
                else
                {
                    throw new Exception();
                }
            }

            try
            {
                TValue value = (TValue)FindValue(key, typeof(TValue), true);
                cache[key] = value;
                return value;
            }
            catch (ConfigException ex)
            {
                ex.RequestedFinder = this;
                ex.RequestedKey = key;

                var args = new ConfigErrorEventArgs
                {
                    Exception = ex
                };

                ConfigError(args);

                throw ex;
            }
        }

        /// <summary>
        /// Set value of key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                if (cache.ContainsKey(key))
                {
                    cache.Remove(key);
                }
                return;
            }

            cache[key] = value;
        }

        /// <summary>
        /// Add new and override existing values
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Finder Update(ISource source)
        {
            deck.Insert(0, source);
            return this;
        }

        /// <summary>
        /// Add new values only
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Finder Extend(ISource source)
        {
            deck.Add(source);
            return this;
        }

        private object FindValue(string key, Type type, bool valueFoundEvent)
        {
            if (TypeInspector.IsConfig(type))
            {
                return TypeBinder.CreateDynamicInstance(key, type, this);
            }

            if (TypeInspector.IsArray(type))
            {
                throw new NotImplementedException("Array value");
            }

            return FindPrimitiveValue(key, type, valueFoundEvent);
        }

        private object FindPrimitiveValue(string key, Type type, bool valueFoundEvent)
        {
            var value = FindFirstValue(key);

            if (value.IsSource)
            {
                throw new ValueTypeException("Config option has incorrect value. Subconfig insted of value", value, type);
            }

            var v = TryChangeType(value, type);

            if (valueFoundEvent)
            {
                Validate(key, v);

                var args = new ConfigEventArgs
                {
                    Key = key,
                    Value = v,
                    RawValue = value.Value,
                    Source = value.Source,
                    Finder = this
                };

                ValueFound(args);
            }

            return v;
        }

        private ValueOrSource FindFirstValue(string key)
        {
            foreach (var value in FindAllValues(key))
            {
                if (value.Value == null)
                {
                    throw new ValueIsNullException(value);
                }

                return value;
            }

            throw new ValueNotFoundException();

        }

        // TODO: make it public
        internal IEnumerable<ValueOrSource> FindAllValues(string key)
        {
            foreach (var source in deck)
            {
                foreach (var value in source.GetAllValues(key))
                {
                    if (value is ValueOrSource vos)
                    {
                        yield return vos;
                    }
                    else if (value is ISource src)
                    {
                        yield return ValueOrSource.MkSource(this, src, key);
                    }
                    else
                    {
                        yield return ValueOrSource.MkValue(this, source, value, key);
                    }
                }
            }
        }
    }
}
