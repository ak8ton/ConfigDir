namespace UnitTests.Model
{
    public interface ITypeError
    {
        int NotIntegerValue { get; }
        IOptions NotSubConfig { get; }
        IEmptyValueHolder EmptyValueHolder { get; }
    }

    public interface IEmptyValueHolder
    {
        string EmptyValue { get; }
    }
}
