using System.Xml;
using Newtonsoft.Json;

namespace Import.Transformation.Json
{
    public class XmlToJsonConverter
    {
        public string ConvertToJson(string xml)
        {
            var doc = new XmlDocument();

            doc.LoadXml(xml);

            return JsonConvert.SerializeXmlNode(doc);
        }

        public XmlDocument ConvertToXml(string json)
        {
            return JsonConvert.DeserializeXmlNode(json);
        }
    }
}
