using ConfigDir;

namespace UnitTests.Model
{
    public abstract class Configuration : Config
    {
        [Summary("Настройки тестового стенда")]
        public abstract IStand Stand { get; }
        public abstract IFilename Filename { get; }
        public abstract IOptions NotExsists { get; }
        public abstract ITypeError TypeError { get; }
    }
}
