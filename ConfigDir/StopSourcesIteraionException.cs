using System;

namespace ConfigDir
{
    /// <summary>
    /// Искличение используемое в ISource для реализации атрибута "override"
    /// </summary>
    public class StopSourcesIteraionException : Exception
    {
        /// <summary>
        /// Конструктор исключения StopSourcesIteraionException 
        /// </summary>
        public StopSourcesIteraionException()
        {
        }
    }
}
