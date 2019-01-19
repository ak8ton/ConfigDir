using ConfigDir.Internal;
using System;
using System.Collections.Generic;


namespace ConfigDir.Data
{
    public partial class Finder
    {
        #region Instance

        Type ConfigType { get; }

        internal Finder(Type configType, string key, string[] keys)
        {
            ConfigType = configType;
            Key = key;
            Keys = keys;
        }

        internal void SetParent(Finder parent)
        {
            Parent = parent;
            deck.Add(new ParentSource(this, parent, Key));
        }

        #endregion Instance


        public string Description { get; set; } = "";

        private readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        public string[] Keys { get; }

        internal Type GetValueType(string key)
        {
            return ConfigType?.GetProperty(key)?.PropertyType;
        }

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

            TValue value = (TValue)FindValue(key, typeof(TValue), true);
            cache[key] = value;

            return value;
        }

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

        private object FindValue(string key, Type type, bool valueFoundEvent)
        {
            switch (TypeInspector.GetTypeCategory(type))
            {
                case TypeCategory.Value:
                    return FindPrimitiveValue(key, type, valueFoundEvent);
                case TypeCategory.Config:
                    return TypeBinder.CreateSubConfig(this, key, type);

                // todo Array

                default:
                    throw new NotImplementedException($"GetValue<{type.FullName}>({key})");
            }
        }

        private object FindPrimitiveValue(string key, Type type, bool valueFoundEvent)
        {
            foreach (var value in FindAllValues(key))
            {
                if (value.Type == ValueOrSourceType.value)
                {
                    var v = TryChangeType(value, type);

                    if (valueFoundEvent)
                    {
                        Validate(key, v);
                        ValueFound(value.ToEventArgs(v));
                    }

                    return v;
                }

                if (value.Value == null)
                {
                    ValueNotFound(value.ToEventArgs(this.GetPath(key)));
                }

                ValueTypeError(value.ToEventArgs(), null);
                break;
            }

            ValueNotFound(new ConfigEventArgs { Path = this.GetPath(key) });
            throw new Exception();
        }

        private readonly List<ISource> deck = new List<ISource>();

        public Finder Parent { get; private set; }
        public string Key { get; private set; }

        public Finder Update(ISource source)
        {
            deck.Insert(0, source);
            return this;
        }

        public Finder Extend(ISource source)
        {
            deck.Add(source);
            return this;
        }

        private IEnumerable<ValueOrSource> FindAllValues(string key)
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
            yield return ValueOrSource.MkStop_NotFound(this, key);
        }
    }
}
