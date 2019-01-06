using System.Collections.Generic;
using System.Xml.Linq;


namespace ConfigDir.Readers
{
    class XSource : ISource
    {
        private XElement element;
        private Dictionary<string, object> _d = null;
        private Dictionary<string, object> Dict => _d ?? (_d = Parse());

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
            if (Dict.ContainsKey(key))
            {
               yield return Dict[key];
            }
        }

        public override string ToString()
        {
            return Description;
        }

        private Dictionary<string, object> Parse()
        {
            var dict = new Dictionary<string, object>();
            var nodes = new List<XElement>(); 

            //TODO Обработка ошибок IO
            foreach (var el in (element ?? XDocument.Load( System.IO.Path.Combine(BasePath, FilePath)).Root).Elements())
            {
                string key = el.Name.LocalName;
                object value = GetValue(el);
                nodes.Add(el);

                if (dict.ContainsKey(key))
                {
                    if (dict[key] is List<object> array)
                    {
                        array.Add(value);
                    }
                    else
                    {
                        dict[key] = new List<object> { dict[key], value };
                    }
                }
                else
                {
                    dict[key] = value;
                }
            }

            element = null;
            nodes.Remove();

            return dict;
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
