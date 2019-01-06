using ConfigDir;

namespace UnitTests.Model
{
    public abstract class Configuration : Config
    {
        public abstract IStend Stend { get; }
        public abstract IFilename Filename { get; }

    }
}
