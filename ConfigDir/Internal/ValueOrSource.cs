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
        private readonly string[] keys;
        public string Path => finder.GetPath(keys);

        public ConfigEventArgs ToEventArgs(Type expectedType, object value = null)
        {
            return new ConfigEventArgs
            {
                Path = Path,
                Source = Source,
                Value = value,
                RawValue = Value,
                ExpectedType = expectedType
            };
        }

        private ValueOrSource(Finder finder, ValueOrSourceType type, ISource source, object value, params string[] keys)
        {
            this.finder = finder;
            this.keys = keys;

            Type = type;
            Source = source;
            Value = value;
        }

        /// <summary>
        /// Конструктор конечного значения
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public ValueOrSource MkValue(Finder finder, ISource source, object value, params string[] keys)
        {
            return new ValueOrSource(finder, ValueOrSourceType.value, source, value, keys);
        }

        /// <summary>
        /// Конструктор источника
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static public ValueOrSource MkSource(Finder finder, ISource source, params string[] keys)
        {
            return new ValueOrSource(finder, ValueOrSourceType.source, source, null, keys);
        }

        /// <summary>
        /// Остановить поиск
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public ValueOrSource MkStop(ValueOrSource value)
        {
            return new ValueOrSource(value.finder, ValueOrSourceType.stop, value.Source, value.Value, value.keys);
        }
    }
}
