using HtmlAgilityPack;

namespace MySchemaApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<Schema> schemas = SchemaManager.LoadSchemas();
            List<Schema> favoriteSchemas = schemas.Where(s => s.IsFavorite).ToList();
            if (favoriteSchemas.Count > 0)
            {
                foreach (var s in favoriteSchemas)
                {
                    await PrintSchema(s);
                }
                Console.WriteLine("\nValfri knapp: Återgår till startmeny.");
                Console.ReadKey();
            }

            await NormalOptions();
        }
        static public async Task PrintSchema(Schema schema)
        {
            Console.Clear();
            Console.WriteLine("Schema: " + schema.Title);

            using var http = new HttpClient();

            var html = await http.GetStringAsync(schema.Url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var schemaTable = doc.DocumentNode.SelectSingleNode("//table[@class='schemaTabell']");
            if (schemaTable == null)
            {
                // If this is null the user managed to input a faulty url.
                Console.WriteLine("Kunde inte hitta ett schema på sidan.");
                Console.WriteLine("Är länken korrekt?");
                await Task.Delay(1500);
                return;
            }

            var schemaTableRows = schemaTable.SelectNodes("./tr");

            Printer.CustomPrintTable(schemaTableRows);
        }

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
                Console.Clear();
                Console.WriteLine("Välj ett alternativ:");
                Console.WriteLine("1. Lägg till schema.");
                Console.WriteLine("2. Ta bort schema.");
                Console.WriteLine("3. Visa alla scheman.");
                Console.WriteLine("4. Hantera favoritscheman");
                Console.WriteLine("5. Hantera inställningar");
                Console.WriteLine("Övriga val kommer att avsluta programmet.");

                string choice = Console.ReadLine().Trim();
                switch (choice)
                {
                    case "1":
                        // Add schema
                        await AddSchema();
                        break;

                    case "2":
                        // Remove schema
                        await RemoveSchema();
                        break;

                    case "3":
                        // Show all schemas
                        await ViewSchemas();
                        break;

                    case "4":
                        // Handling FavoriteSchemas (startup schemas)
                        await FavoriteSchema();
                        break;

                    case "5":
                        // Handling settings
                        break;

                    default:
                        // Exits the app.
                        Environment.Exit(0);
                        running = false;
                        break;
                }
            }
        }
        static public async Task AddSchema()
        {
            bool schemaAdded = false;
            List<Schema> schemas = SchemaManager.LoadSchemas();

            while (!schemaAdded)
            {
                Console.Clear();
                Console.WriteLine("0. Tillbaka till startmenyn");
                Console.WriteLine("Skriv länken för schemat. " +
                    "\n\nLänken kan exempelvis se ut såhär: " +
                    "\nhttps://schema.oru.se/setup/jsp/Schema.jsp?startDatum=2026-01-19&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.IK205G-V2062V26-%2C");
                Console.Write("\nDin länk: ");

                string url = Console.ReadLine().Trim();
                if (url == "0") return; //Back to main menu.
                if (schemas.Any(s => s.Url == url)) //Controls for duplicates.
                {
                    Console.WriteLine("Det finns redan ett schema med den URL:en, försök igen.");
                    await Task.Delay(1000);
                    continue;
                }

                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
                && uri.Host == "schema.oru.se")
                {

                    bool titleChosen = false;
                    while (!titleChosen)
                    {
                        Console.Clear();
                        Console.WriteLine("0. Tillbaka till startmenyn");
                        Console.Write("Skriv en titel för schemat: ");

                        string title = Console.ReadLine().Trim();
                        if (title == "0") return;

                        if (!string.IsNullOrEmpty(title)) //Saves the Schema
                        {
                            schemas.Add(new Schema { Title = title, Url = url });
                            SchemaManager.SaveSchemas(schemas);

                            Console.Clear();
                            Console.Write("Schemat har lagts till! Skickar dig till startmenyn");
                            for (int i = 0; i < 3; i++)
                            {
                                await Task.Delay(1250);
                                Console.Write(".");
                            }
                            titleChosen = true;
                            schemaAdded = true;
                        }
                        else
                        {
                            Console.WriteLine("Titeln kan inte vara tom, försök igen.");
                            await Task.Delay(1000);
                        }
                    }

                }
                else
                {
                    Console.WriteLine("Kunde inte hitta det schemat, försök igen.");
                    await Task.Delay(1000);
                }
            }
        }

        static public async Task RemoveSchema()
        {
            Console.Clear();
            List<Schema> schemas = SchemaManager.LoadSchemas();

            // If there are no schemas, return to main menu.
            if (schemas.Count == 0)
            {
                Console.Write("Inga scheman hittades, skickar dig till startmenyn.");
                for (int i = 0; i < 2; i++)
                {
                    await Task.Delay(1250); Console.Write(".");
                }
                return;
            }

            // Prints the saved schemas' titles.
            ShowAllSchemaTitles(schemas, "Välj ett schema att ta bort:");

            string choice = Console.ReadLine();
            if (int.TryParse(choice, out int result)
                && result < schemas.Count + 1
                && result >= 0)
            {
                if (result == 0) return; //Back to main menu.

                Console.Write("Raderar schemat \"" + schemas[result - 1].Title + "\".");
                schemas.RemoveAt(result - 1);
                for (int i = 0; i < 2; i++)
                {
                    await Task.Delay(500); Console.Write(".");
                }
                SchemaManager.SaveSchemas(schemas);
                return;
            }
        }

        static public async Task ViewSchemas()
        {
            Console.Clear();
            List<Schema> schemas = SchemaManager.LoadSchemas();
            Console.WriteLine("0. Tillbaka till startmenyn");

            // Displays all schemas with numbers, and asks the user to choose one to remove.
            if (schemas.Count == 0)
            {
                Console.Write("Inga scheman hittades, skickar dig till startmenyn.");
                for (int i = 0; i < 2; i++)
                {
                    await Task.Delay(500); Console.Write(".");
                }
                return;
            }

            // Prints the saved schemas' titles.
            ShowAllSchemaTitles(schemas, "Välj ett schema att visa:");

            string choice = Console.ReadLine();
            if (int.TryParse(choice, out int result)
                && result < schemas.Count + 1
                && result >= 0)
            {
                if (result == 0) return;
                await PrintSchema(schemas[result - 1]);

                Console.WriteLine("\nValfri knapp: Återgår till huvudmeny.");
                Console.ReadKey();
                return;
            }
        }

        static public async Task FavoriteSchema()
        {
            Console.Clear();
            List<Schema> schemas = SchemaManager.LoadSchemas();
            List<Schema> normalSchemas = schemas.Where(s => !s.IsFavorite).ToList();
            List<Schema> favoriteSchemas = schemas.Where(s => s.IsFavorite).ToList();

            Console.WriteLine("Här kan du välja att \"Favorita\" ett schema.");
            Console.WriteLine("När du favoritar ett schema kommer det att visas så fort du startar applikationen.");

            Console.WriteLine("\n0. Tillbaka till startmenyn.\n");

            if (normalSchemas.Count == 0)
            {
                Console.WriteLine("Alla scheman är redan favoriter! Om du vill ta bort ett schema från favoriter, välj det under de redan favoritmarkerade scheman nedan.");
            }
            else
            {
                Console.WriteLine("Scheman som inte är favoriter ännu:");
                for (int i = 0; i < normalSchemas.Count; i++)
                {
                    Console.WriteLine(i + 1 + ". " + normalSchemas[i].Title);
                }
            }

            if (favoriteSchemas.Count > 0)
            {
                Console.WriteLine("\nOm du vill \"De-Favorita\" ett schema väljer du en av dessa scheman.");
                Console.WriteLine("Scheman som redan är favoriter:");
                for (int i = 0; i < favoriteSchemas.Count; i++)
                {
                    Console.WriteLine(i + 1 + normalSchemas.Count + ". " + favoriteSchemas[i].Title);
                }
            }

            string choice = Console.ReadLine();
            if (int.TryParse(choice, out int result)
                && result < schemas.Count + 1
                && result >= 0)
            {
                if (result == 0) return;

                if (result <= normalSchemas.Count)
                {
                    // Favorite a normal schema
                    normalSchemas[result - 1].IsFavorite = true;
                    Console.WriteLine("Favoritar schemat \"" + normalSchemas[result - 1].Title + ".");
                }

                else
                {
                    // De-Favorite a favorite schema
                    favoriteSchemas[result - 1 - normalSchemas.Count].IsFavorite = false;
                    Console.WriteLine("De-Favoritar schemat \"" + favoriteSchemas[result - 1 - normalSchemas.Count].Title + ".");
                }

                SchemaManager.SaveSchemas(schemas);

                // Buffer
                Console.Write("Skickar dig till startmenyn.");
                for (int i = 0; i < 2; i++)
                {
                    await Task.Delay(500); Console.Write(".");
                }
                return;
            }
            Console.ReadKey();
        }

        static public void ShowAllSchemaTitles(List<Schema> schemas, string headermsg)
        {
            // Displays all schemas with numbers, and asks the user to choose one to remove.
            // This only prints the schemas, the logic and choices are handled in whatever calls this method.
            Console.WriteLine(headermsg);
            for (int i = 0; i < schemas.Count; i++)
            {
                Console.WriteLine(i + 1 + ". " + schemas[i].Title);
            }
        }

        //for testing
        //var url = "https://schema.oru.se/setup/jsp/Schema.jsp?startDatum=2026-01-19&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.IK205G-V2062V26-%2C";
        //var urlKrim = "https://schema.oru.se/setup/jsp/Schema.jsp?startDatum=idag&intervallTyp=m&intervallAntal=6&sokMedAND=false&sprak=SV&resurser=k.KR400G-V3060V26-%2C";
    }
}
