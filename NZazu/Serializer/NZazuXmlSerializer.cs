using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NZazu.Contracts;

namespace NZazu.Serializer
{
    public class NZazuXmlSerializer
        : INZazuDataSerializer
    {
        public string Serialize(Dictionary<string, string> data)
        {
            // cf: http://stackoverflow.com/questions/12554186/how-to-serialize-deserialize-to-dictionaryint-string-from-custom-xml-not-us
            var xElem = new XElement(
                "items",
                data.Where(x => x.Value != null).Select(x => new XElement("item", new XAttribute("id", x.Key), new XAttribute("value", x.Value)))
             );

            return xElem.ToString();
        }

        public Dictionary<string, string> Deserialize(string value)
        {
            var xElem2 = XElement.Parse(value);
            var newDict = xElem2.Descendants("item")
                .ToDictionary(x => (string)x.Attribute("id"), x => (string)x.Attribute("value"));
            return newDict;
        }
    }
}
