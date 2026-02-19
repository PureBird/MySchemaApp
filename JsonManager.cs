using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MySchemaApp
{
    public class JsonManager<T>
    {
        private readonly string _filePath;
        public JsonManager(string filePath) => _filePath = filePath;
        public void Save(T obj)
        {
            var option = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(obj, option);
            File.WriteAllText(_filePath, json);
        }
        public T Load()
        {
            if (!File.Exists(_filePath)) return Activator.CreateInstance<T>(); //Creates a new instance of T.
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<T>(json) ?? Activator.CreateInstance<T>();
        }
    }

    public class SchemaManager
    {
        private readonly JsonManager<List<Schema>> _json = new("schema.json");
        public List<Schema> Schemas => _json.Load();
        public void Save(List<Schema> schemas) => _json.Save(schemas);
    }

    public class SettingsManager
    {
        private readonly JsonManager<Settings> _json = new("settings.json");
        public Settings Settings => _json.Load();
        public void Save(Settings settings) => _json.Save(settings);
    }
}
