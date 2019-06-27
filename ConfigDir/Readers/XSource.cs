using System.Collections.Generic;
using System.Xml.Linq;


namespace ConfigDir.Readers
{
    class XSource : ISource, IArraySource
    {
        class ValuesCollection
        {
            public Dictionary<string, object> Dict { get; }
            public List<object> Values { get; }

            public ValuesCollection(Dictionary<string, object> dict, List<object> values)
            {
                Dict = dict;
                Values = values;
            }
        }

        private XElement element;
        private ValuesCollection _v = null;
        private ValuesCollection Values => _v ?? (_v = Parse());

        public string BasePath { get; }
        public string FilePath { get; }
        public string Description { get; }

        public XSource(string basePath, string fileName)
        {
            BasePath = basePath;
            FilePath = fileName;
            Description = "XML файл: " + FilePath;
        }

        private XSource(string basePath, string fileName, XElement element)
        {
            BasePath = basePath;
            FilePath = fileName;
            this.element = element;
        }

        public IEnumerable<object> GetAllValues(string key)
        {
            key = key.ToLower();
            if (Values.Dict.ContainsKey(key))
            {
                yield return Values.Dict[key];
            }
        }

        public IEnumerable<object> GetAllValues(int index)
        {
            if (index < 0) yield break;

            if (Values.Values.Count > index)
            {
                yield return Values.Values[index];
            }
        }

        public IEnumerable<string> GetKeys()
        {
            return Values.Dict.Keys;
        }

        public int GetCount()
        {
            return Values.Values.Count;
        }

        public override string ToString()
        {
            return Description;
        }

        private ValuesCollection Parse()
        {
            var values = new List<object>();
            var dict = new Dictionary<string, object>();
            var nodes = new List<XElement>();

            //TODO Обработка ошибок IO
            foreach (var el in (element ?? XDocument.Load(System.IO.Path.Combine(BasePath, FilePath)).Root).Elements())
            {
                nodes.Add(el);

                string key = el.Name.LocalName.ToLower();
                object value = GetValue(el);

                values.Add(value);
                if (!dict.ContainsKey(key))
                {
                    dict[key] = value;
                }
            }

            element = null;
            nodes.Remove();

            return new ValuesCollection(dict, values);
        }

        private object GetValue(XElement element)
        {
            // TODO Что делать с аттрибутами?
            if (element.HasElements || element.HasAttributes)
            {
                return new XSource(BasePath, FilePath, element);
            }
            else
            {
                return element.Value;
            }
        }
    }
}
