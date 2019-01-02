namespace ConfigDir
{
    /// <summary>
    /// Базовый интерфейс привязки
    /// </summary>
    public interface IConfigBase
    {
        void Validate(string key, object value);
        string[] Keys { get; }
        TValue GetValue<TValue>(string key);
        void SetValue(string key, object value);
        ConfigBase Extend(ISource source);
        ConfigBase Update(ISource source);
    }
}
