using System.Collections.Generic;

namespace ConfigDir
{
    public interface ISource
    {
        string Description { get; }
        IEnumerable<object> GetAllValues(string key);
    }
}
