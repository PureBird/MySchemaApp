using System.Text.Json;

namespace MySchemaApp
{
    static public class SchemaManager
    {
        static public void SaveSchemas(List<Schema> schemas, string filePath)
        {
            var option = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(schemas, option);
            File.WriteAllText(filePath, json);
        }

        static public List<Schema> LoadSchemas(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Schema>();
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Schema>>(json) ?? new List<Schema>();
        }
    }
}
