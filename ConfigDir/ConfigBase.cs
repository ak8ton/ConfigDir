using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigDir.Internal;


namespace ConfigDir
{
    /// <summary>
    /// Базовый класс привязки
    /// </summary>
    public abstract class ConfigBase : IConfigBase
    {
        #region Reflection

        internal static readonly Type BaseConfigType;
        internal static readonly Type[] GetterArgs;
        internal static readonly Type[] SetterArgs;
        internal static readonly MethodInfo Getter;
        internal static readonly MethodInfo Setter;

        static ConfigBase()
        {
            var getterName = nameof(GetValue);
            var setterName = nameof(SetValue);

            BaseConfigType = typeof(ConfigBase);
            Getter = BaseConfigType.GetMethod(getterName);
            Setter = BaseConfigType.GetMethod(setterName);

            GetterArgs = Getter.GetParameters().Select(p => p.ParameterType).ToArray();
            SetterArgs = Setter.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        #endregion Reflection


        #region Instance

        private HashSet<string> KeysSet;
        private IFinder Finder;

        // TODO Converters

        internal static void Init(IConfigBase instance, InstancePropertyes props)
        {
            var config = (ConfigBase)instance;
            config.KeysSet = props.KeysSet;
            config.Finder = props.Finder;
        }

        #endregion Instance


        private readonly Dictionary<string, object> cash = new Dictionary<string, object>();

        public string[] Keys => KeysSet.ToArray();

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

        public ConfigBase Extend(ISource source)
        {
            Finder.Extend(source);
            return this;
        }

        public ConfigBase Update(ISource source)
        {
            Finder.Update(source);
            return this;
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
                    return TypeBinder.CreateSubConfig<TValue>(Finder.Query(key));

                default:
                    throw new NotImplementedException("GetValue<TValue>(string key)");
            }
        }

        private TValue FindPrimitiveValue<TValue>(string key)
        {
            foreach (var value in Finder.GetAllValues(key))
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
