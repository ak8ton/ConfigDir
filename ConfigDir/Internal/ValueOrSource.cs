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

        private ValueOrSource(ValueOrSourceType type, ISource source, object value)
        {
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
        static public ValueOrSource MkValue(ISource source, object value)
        {
            return new ValueOrSource(ValueOrSourceType.value, source, value);
        }
    
        /// <summary>
        /// Конструктор источника
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static public ValueOrSource MkSource(ISource source)
        {
            return new ValueOrSource(ValueOrSourceType.source, source, null);
        }
    
        /// <summary>
        /// Остановить поиск
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public ValueOrSource MkStop(ISource source, object value)
        {
            return new ValueOrSource(ValueOrSourceType.stop, source, value);
        }
    }
}
