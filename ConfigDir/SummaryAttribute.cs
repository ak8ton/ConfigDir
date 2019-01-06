using System;


namespace ConfigDir
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SummaryAttribute : Attribute
    {
        public string Summary { get; }

        public SummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
