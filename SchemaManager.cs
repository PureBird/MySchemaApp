using System.Text.Json;

namespace MySchemaApp
{
    static public class SchemaManager
    {
        static readonly string FilePath = "schema.json";
        //static public void SaveSchemas(List<Schema> schemas, string filePath)
        //{
        //    var option = new JsonSerializerOptions { WriteIndented = true };
        //    string json = JsonSerializer.Serialize(schemas, option);
        //    File.WriteAllText(filePath, json);
        //}
        static public void SaveSchemas(List<Schema> schemas)
        {
            var option = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(schemas, option);
            File.WriteAllText(FilePath, json);
        }

        //static public List<Schema> LoadSchemas(string filePath)
        //{
        //    if (!File.Exists(filePath)) return new List<Schema>();
        //    string json = File.ReadAllText(filePath);
        //    return JsonSerializer.Deserialize<List<Schema>>(json) ?? new List<Schema>();
        //}
        static public List<Schema> LoadSchemas()
        {
            if (!File.Exists(FilePath)) return new List<Schema>();
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Schema>>(json) ?? new List<Schema>();
        }
    }
}
