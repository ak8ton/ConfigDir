using ConfigDir.Data;

namespace ConfigDir
{
    /// <summary>
    /// Configuration base interface
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Finder object
        /// </summary>
        Finder Finder { get; }
    }
}
