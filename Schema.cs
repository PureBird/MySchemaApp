namespace MySchemaApp
{
    public class Schema
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsFavorite { get; set; } = false;
        public Schema() { }
    }
}
