namespace ConfigDir
{
    /// <summary>
    /// Базовый интерфейс привязки
    /// </summary>
    public interface IConfig
    {
        string Key { get; }
        string[] Keys { get; }
        string Description { get; set; }

        Config Parent { get; }
        Config Extend(ISource source);
        Config Update(ISource source);

        void Validate(string key, object value);
        TValue GetValue<TValue>(string key);
        void SetValue(string key, object value);
    }
}
