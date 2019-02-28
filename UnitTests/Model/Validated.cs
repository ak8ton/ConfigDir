using ConfigDir;

namespace UnitTests.Model
{
    public abstract class Validated : Config
    {
        public abstract string Value1 { get; }

        public override void Validate(string key, object value)
        {
            throw new StopTestException();
        }
    }
}
