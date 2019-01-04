using ConfigDir.Internal;
using System;
using System.Collections.Generic;


namespace ConfigDir.Data
{
    public partial class Finder
    {
        #region Instance

        internal Finder(string[] keys)
        {
            Keys = keys;
        }

        internal void SetParent(Finder parent, string key)
        {
            Parent = Parent;
            Key = key;
            deck.Add(new ParentSource(parent, key));
        }

        #endregion Instance


        public string Description { get; set; } = "";

        private readonly Dictionary<string, object> cash = new Dictionary<string, object>();

        public string[] Keys { get; }



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
                    return TypeBinder.CreateSubConfig<TValue>(this, key);

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
                        yield return ValueOrSource.MkSource(src);
                    }
                    else
                    {
                        yield return ValueOrSource.MkValue(source, value);
                    }
                }
            }
        }
    }
}
