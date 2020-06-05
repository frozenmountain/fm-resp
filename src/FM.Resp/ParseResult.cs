using System;
using System.Collections.Generic;
using System.Linq;

namespace FM.Resp
{
    class ParseResult
    {
        public DataType[] Types { get; }

        public Element[] Elements { get { return _Elements.ToArray(); } }
        private List<Element> _Elements;

        private Dictionary<DataType, List<Element>> _ElementsByType;

        public ParseResult()
        {
            Types = Enum.GetValues(typeof(DataType)).Cast<DataType>().ToArray();

            _Elements = new List<Element>();
            _ElementsByType = new Dictionary<DataType, List<Element>>();

            foreach (var type in Types)
            {
                _ElementsByType[type] = new List<Element>();
            }
        }

        public void AddElement(Element element)
        {
            _Elements.Add(element);
            _ElementsByType[element.Type].Add(element);
        }

        public Element[] GetElements(DataType type)
        {
            return _ElementsByType[type].ToArray();
        }
    }
}
