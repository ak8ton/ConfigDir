using ConfigDir.Data;

namespace ConfigDir
{
    /// <summary>
    /// Базовый интерфейс привязки
    /// </summary>
    public interface IConfig
    {
        Finder Data { get; }
        void Validate(string key, object value);
    }
}
