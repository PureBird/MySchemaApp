using HtmlAgilityPack;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MySchemaApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await NormalOptions();
            //List<Schema> schemas = SchemaManager.LoadSchemas();

            // Prints all favorite schemas first.
            //List<Schema> favorites = schemas.Where(s => s.IsFavorite).ToList();
            //if (favorites.Count > 0) foreach (var schema in favorites) await PrintSchema(schema);
            //else await ShowAllSchemas();
        }
        static public async Task PrintSchema(Schema schema)
        {
            Console.Clear();
            Console.WriteLine("Schema: " + schema.Title);

            //var url = "https://schema.oru.se/setup/jsp/Schema.jsp?startDatum=2026-01-19&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.IK205G-V2062V26-%2C";
            //var urlKrim = "https://schema.oru.se/setup/jsp/Schema.jsp?startDatum=idag&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.KR400G-V3060V26-%2C";
            using var http = new HttpClient();

            var html = await http.GetStringAsync(schema.Url);
            //var html = await http.GetStringAsync(urlKrim);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var schemaTable = doc.DocumentNode.SelectSingleNode("//table[@class='schemaTabell']");
            var schemaTableRows = schemaTable.SelectNodes("./tr");

            Helper.CustomPrintTable(schemaTableRows);

            Console.WriteLine("");
            await NormalOptions();
        }

        // Print menu options. Can't occur if there are no schemas.
        static public async Task NormalOptions()
        {
            //Console.WriteLine("Välj ett alternativ:");
            //Console.WriteLine("0. Avsluta programmet");
            //Console.WriteLine("1. Se hela schemat med alla kolumner");
            //Console.WriteLine("2. Visa schema");
            //Console.WriteLine("3. Lägg till schema");
            //Console.WriteLine("4. Ta bort schema");

            bool running = true;
            while (running)
            {
                Console.WriteLine("Välj ett alternativ:");
                Console.WriteLine("1. Lägg till schema.");
                Console.WriteLine("2. Ta bort schema.");
                Console.WriteLine("3. Visa alla scheman.");
                Console.WriteLine("4. Hantera favoritscheman");
                Console.WriteLine("Övriga val kommer att avsluta programmet.");

                string choice = Console.ReadLine().Trim();
                switch (choice)
                {
                    case "1":
                        // Lägg till schema
                        await AddSchema();
                        break;

                    case "2":
                        // Ta bort schema
                        break;

                    case "3":
                        // Visa alla scheman
                        await ShowAllSchemas();
                        break;

                    case "4":
                        // Hantera favoritscheman
                        break;

                    default:
                        Environment.Exit(0);
                        running = false; // Avsluta programmet
                        break;
                }
            }
        }
        static public async Task AddSchema()
        {
            Console.Clear();
            Console.WriteLine("Skriv länken för schemat. " +
                "\n\nLänken kan exempelvis se ut såhär: " +
                "\nhttps://schema.oru.se/setup/jsp/Schema.jsp?startDatum=2026-01-19&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.IK205G-V2062V26-%2C");
            Console.Write("\nDin länk: ");

            string url = Console.ReadLine().Trim();

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Console.Clear();
                Console.Write("Skriv en titel för schemat: ");

                string title = Console.ReadLine().Trim();
                if (!string.IsNullOrEmpty(title))
                {
                    List<Schema> schemas = SchemaManager.LoadSchemas();
                    schemas.Add(new Schema { Title = title, Url = url });
                    SchemaManager.SaveSchemas(schemas);
                    
                    Console.Clear();
                    Console.Write("Schemat har lagts till! Skickar dig till startmenyn");
                    for (int i = 0; i < 3; i++)
                    {
                        await Task.Delay(1250); 
                        Console.Write(".");
                    }
                    return;
                }
                else
                {
                    Console.WriteLine("Titeln kan inte vara tom, försök igen.");
                    await Task.Delay(2000);
                    Console.Clear();
                    return;
                }

            }
            else
            {
                //Temp
                //Maybe an else if with regex to check if the url is from schema.oru.se.
                Console.WriteLine("Ogiltig URL, försök igen.");
                await Task.Delay(2000);
                Console.Clear();
                return;
            }
        }
        static public async Task ShowAllSchemas()
        {
            Console.Clear();
            List<Schema> schemas = SchemaManager.LoadSchemas();
            if (schemas.Count == 0)
            {
                Console.Write("Inga scheman hittades, skickar dig till lägg till schema vyn.");
                for (int i = 0; i < 4; i++)
                {
                    await Task.Delay(1250); Console.Write(".");
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
        }
    }
}
