using System.Collections.Generic;


namespace ConfigDir
{
    /// <summary>
    /// Source value type for array representation
    /// </summary>
    public interface IArraySource
    {
        /// <summary>
        /// Array itams
        /// </summary>
        /// <returns></returns>
        IEnumerator<object> GetArrayItamsEnumerator();
    }
}
