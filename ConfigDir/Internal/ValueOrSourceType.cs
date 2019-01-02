namespace ConfigDir.Internal
{
    /// <summary>
    /// Тип значения
    /// </summary>
    enum ValueOrSourceType : byte
    {
        /// <summary>
        /// Источник. Вложенный конфиг
        /// </summary>
        source,

        /// <summary>
        /// Конечное значение
        /// </summary>
        value,

        /// <summary>
        /// Источник скрыт конечным значением
        /// </summary>
        stop
    }
}
