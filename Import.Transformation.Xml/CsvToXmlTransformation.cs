using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Import.Abstractions;
using Import.Abstractions.Interfaces;

namespace Import.Transformation.Xml
{
    public class CsvToXmlTransformation:ITransformationFileService
    {
        public async Task<string> TransformFile(string fileName, TransformationSettings settings)
        {
            var lines = File.ReadAllLines(fileName);

            string[] headers;

            return await Task.Run(() =>
            {
                headers = settings.HeaderOnFirstRow ? lines[0].Split(settings.Separator).Select(x => x.Trim('\"').Trim('\'')).ToArray() : lines[0].Split(settings.Separator).Select((s, i) => $"Column{i}").ToArray();

                var list = new XElement(Path.GetFileNameWithoutExtension(fileName));

                for(var index = 0; index < lines.Length; index++)
                {
                    if (settings.HeaderOnFirstRow && index == 0) continue;

                    var xElement = new XElement(Path.GetFileNameWithoutExtension(fileName));

                    var data = lines[index].Split(settings.Separator);

                    for(var x = 0; x < data.Length; x++)
                    {
                        if (string.IsNullOrEmpty(headers[x])) continue;
                        var item = data[x];
                        xElement.Add(new XElement(headers[x].Replace(" ", ""),  item.Trim('\"').Trim('\'')));
                    }

                    list.Add(xElement);
                }

                return list.ToString();
            });
        }
    }
}
