using ConfigDir.Data;
using System;

namespace ConfigDir.Internal
{
    /// <summary>
    /// Результат поиска
    /// </summary>
    struct ValueOrSource
    {
        /// <summary>
        /// Тип значения
        /// </summary>
        public ValueOrSourceType Type { get; }

        /// <summary>
        /// Источник в котором было найдено значение
        /// </summary>
        public ISource Source { get; }

        /// <summary>
        /// Найденое значение
        /// </summary>
        public object Value { get; }

        private readonly Finder finder;
        private readonly string key;
        public string Path => finder.GetPath(key);

        public ConfigEventArgs ToEventArgs(Type expectedType, object value = null)
        {
            var args = GetConfigEventArgs();
            args.Value = value;
            args.ExpectedType = expectedType;
            return args;
        }

        public ConfigEventArgs ToEventArgs(string requiredPath)
        {
            var args = GetConfigEventArgs();
            args.RequiredPath = requiredPath;
            return args;
        }

        private ConfigEventArgs GetConfigEventArgs()
        {
            return new ConfigEventArgs
            {
                Path = Path,
                Source = Source,
                RawValue = Value,
            };
        }

        private ValueOrSource(Finder finder, ValueOrSourceType type, ISource source, object value, string key)
        {
            this.finder = finder;
            this.key = key;

            Type = type;
            Source = source;
            Value = value;
        }

        static public ValueOrSource MkValue(Finder finder, ISource source, object value, string key)
        {
            return new ValueOrSource(finder, ValueOrSourceType.value, source, value, key);
        }

        static public ValueOrSource MkSource(Finder finder, ISource source, string key)
        {
            return new ValueOrSource(finder, ValueOrSourceType.source, source, null, key);
        }

        static public ValueOrSource MkStop_NotFound(Finder finder, string key)
        {
            return new ValueOrSource(finder, ValueOrSourceType.stop, null, null, key);
        }

        static public ValueOrSource MkStop_TypeError(ValueOrSource value)
        {
            return new ValueOrSource(value.finder, ValueOrSourceType.stop, value.Source, value.Value, value.key);
        }
    }
}
