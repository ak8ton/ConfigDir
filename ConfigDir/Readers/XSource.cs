using System.Collections.Generic;
using System.Xml.Linq;


namespace ConfigDir.Readers
{
    class XSource : ISource
    {
        private XElement element;
        private Dictionary<string, object> _d = null;
        private Dictionary<string, object> Dict => _d ?? (_d = Parse());

        public string Description => "XML файл " + FileName;
        public string FileName { get; private set; }

        public XSource(string fileName)
        {
            FileName = fileName;
        }

        private XSource(string fileName, XElement element)
        {
            FileName = fileName;
            this.element = element;
        }

        public IEnumerable<object> GetAllValues(string key)
        {
            if (Dict.ContainsKey(key))
            {
               yield return Dict[key];
            }
        }

        private Dictionary<string, object> Parse()
        {
            var dict = new Dictionary<string, object>();
            var nodes = new List<XElement>(); 

            //TODO Обработка ошибок IO
            foreach (var el in (element ?? XDocument.Load(FileName).Root).Elements())
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
                return new XSource(FileName, element);
            }
            else
            {
                return element.Value;
            }
        }
    }
}
