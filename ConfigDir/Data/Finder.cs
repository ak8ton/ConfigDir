using ConfigDir.Exceptions;
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
        public KeyOrIndex Key { get; }

        internal Finder(Type configType, KeyOrIndex key, Finder parent)
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

            return GetValue<TValue>(new KeyOrIndex(key));
        }

        /// <summary>
        /// Get value by key and type
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue GetValue<TValue>(int index)
        {
            if (index < 0)
            {
                throw new ArgumentException(nameof(index));
            }

            return GetValue<TValue>(new KeyOrIndex(index));
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
        /// Set value by index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValue(int index, object value)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            string key = index.ToString();
            SetValue(key, value);
        }

        /// <summary>
        /// Add new and override existing values
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Finder Update(IConfigSource source)
        {
            deck.Insert(0, source);
            return this;
        }

        /// <summary>
        /// Add new values only
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Finder Extend(IConfigSource source)
        {
            deck.Add(source);
            return this;
        }

        private TValue GetValue<TValue>(KeyOrIndex keyOrIndex)
        {
            var key = keyOrIndex.ToString();
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
                TValue value = (TValue)FindValue(keyOrIndex, typeof(TValue), true);
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

        private object FindValue(KeyOrIndex keyOrIndex, Type type, bool valueFoundEvent)
        {
            if (TypeInspector.IsConfig(type))
            {
                if (TypeInspector.IsArray(type))
                {
                    throw new NotImplementedException("Массивы не реализованы");
                }

                return TypeBinder.CreateDynamicInstance(keyOrIndex, type, this);
            }

            return FindPrimitiveValue(keyOrIndex, type, valueFoundEvent);
        }

        private object FindPrimitiveValue(KeyOrIndex keyOrIndex, Type type, bool valueFoundEvent)
        {
            var value = FindFirstValue(keyOrIndex);

            if (value.IsSource)
            {
                throw new ValueTypeException("Config option has incorrect value. Subconfig insted of value", value, type);
            }

            var v = TryChangeType(value, type);

            if (valueFoundEvent)
            {
                var key = keyOrIndex.ToString();
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

        private ValueOrSource FindFirstValue(KeyOrIndex keyOrIndex)
        {
            foreach (var value in FindAllValues(keyOrIndex))
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
        internal IEnumerable<ValueOrSource> FindAllValues(KeyOrIndex keyOrIndex)
        {
            foreach (var source in deck)
            {
                IEnumerable<object> enumerator = null;

                if (keyOrIndex.Key != null)
                {
                    if (source is IConfigSource cfg)
                    {
                        enumerator = cfg.GetAllValues(keyOrIndex.Key);
                    }
                    else
                    {
                        throw new Exception("Congig expected");
                    }
                }

                if (keyOrIndex.Index != null)
                {
                    if (source is IArraySource cfg)
                    {
                        enumerator = cfg.GetAllValues(keyOrIndex.Index.Value);
                    }
                    else
                    {
                        throw new Exception("Array expected");
                    }
                }

                if (enumerator == null)
                {
                    throw new Exception("Unexpected key type");
                }

                foreach (var value in enumerator)
                {
                    if (value is ValueOrSource vos)
                    {
                        yield return vos;
                    }
                    else if (value is ISource src)
                    {
                        yield return ValueOrSource.MkSource(this, src, keyOrIndex);
                    }
                    else
                    {
                        yield return ValueOrSource.MkValue(this, source, value, keyOrIndex);
                    }
                }
            }
        }
    }
}
