using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MySchemaApp
{
    internal class Program
    {
        static readonly string FilePath = "schema.json";
        static async Task Main(string[] args)
        {
            List<Schema> schemas = SchemaManager.LoadSchemas(FilePath);
            //List<Schema> schemas = new List<Schema>();

            if (schemas.Count == 0)
            {
                Console.Write("Inga scheman hittades, skickar dig till lägg till schema vyn.");
                for (int i = 0; i < 4; i++)
                {
                    await Task.Delay(1250);
                    Console.Write(".");
                }
                return;
            }

            Console.WriteLine("Välj ett schema att visa:");
            for (int i = 0; i < schemas.Count; i++)
            {
                Console.WriteLine(i + 1 + ". " + schemas[i].Title);
            }
            
            string choice = Console.ReadLine();
            if (int.TryParse(choice, out int result) 
                && result < schemas.Count + 1
                && result > 0)
            {
                await PrintSchema(schemas[result - 1]);
            }

            //await PrintSchema(schemas[0]);
        }
        static public async Task PrintSchema(Schema schema)
        {
            Console.WriteLine("Schema: " + schema.Title);

            var url = "https://schema.oru.se/setup/jsp/Schema.jsp?startDatum=2026-01-19&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.IK205G-V2062V26-%2C";
            //var urlKrim = "https://schema.oru.se/setup/jsp/Schema.jsp?startDatum=idag&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.KR400G-V3060V26-%2C";
            using var http = new HttpClient();

            var html = await http.GetStringAsync(schema.Url);
            //var html = await http.GetStringAsync(urlKrim);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var schemaTable = doc.DocumentNode.SelectSingleNode("//table[@class='schemaTabell']");
            var schemaTableRows = schemaTable.SelectNodes("./tr");

            Helper.CustomPrintTable(schemaTableRows);

            Console.ReadKey();
        }

        // Print menu options. Can't occur if there are no schemas.
        static public void PrintOptions()
        {
            Console.WriteLine("Välj ett alternativ:");
            Console.WriteLine("0. Avsluta programmet");
            Console.WriteLine("1. Se hela schemat med alla kolumner");
            Console.WriteLine("2. Visa schema");
            Console.WriteLine("3. Lägg till schema");
            Console.WriteLine("4. Ta bort schema");
        }
    }
}
