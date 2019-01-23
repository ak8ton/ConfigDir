using System;


namespace ConfigDir
{
    /// <summary>
    /// Configuration property description
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SummaryAttribute : Attribute
    {
        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// Configuration property description
        /// </summary>
        /// <param name="summary"></param>
        public SummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
