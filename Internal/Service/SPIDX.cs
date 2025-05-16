using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snapwatch.Internal.Core
{
    public class SPIDX
    {
        using (var fs = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
        using (var sr = new StreamReader(fs))
        using (var reader = new JsonTextReader(sr))

        using (var indexWriter = new StreamWriter(indexFilePath))
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    // Текущая позиция в потоке — начало объекта
                    long position = fs.Position;

    // Загружаем JObject страницы
    var obj = JObject.Load(reader);

    int page = obj["page"]?.Value<int>() ?? -1;
                    if (page != -1)
                    {
                        // Записываем в индекс: page \t позиция
                        indexWriter.WriteLine($"{page}\t{position}");
                    }
                }
            }
        }
    }
}
