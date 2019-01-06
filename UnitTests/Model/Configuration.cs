using ConfigDir;

namespace UnitTests.Model
{
    public abstract class Configuration : Config
    {
        [Summary("Настройки тестового стенда")]
        public abstract IStend Stend { get; }
        public abstract IFilename Filename { get; }
    }
}
