using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MySchemaApp
{
    internal class Printer
    {
        static public string StartDateToday(string url)
        {
            //Probably not a valid url.
            if (!url.Contains("startDatum")) return url;

            //Replaces startDatum=2020-01-01 with startDatum=idag.
            string updatedUrl = Regex.Replace(url, @"(startDatum=)[^&]*", "$1idag");
            return updatedUrl;
        }
        static public string CleanText(string text)
        {
            // Removes &nbsp;, &amp;, and such.
            string cleanText = HtmlEntity.DeEntitize(text);

            // Replaces tabs and such with empty space.
            cleanText = Regex.Replace(cleanText, @"\s+", " ");

            return cleanText.Trim();
        }

        // Parses a date like 19 aug and return a DateTime object.
        static public DateTime ParseDate(string dayAndDate)
        {
            int dummyYear = DateTime.Now.Year; // Won't be displayed, just needed for parsing.

            // Jan didn't work for swedish culture (it wants jan), Maj didn't work for english culture (it wants May), so I'm using both.
            var cultures = new[]
            {
                new CultureInfo("sv-SE"), // Swedish
                new CultureInfo("en-US")  // English
            };

            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(dayAndDate + " " + dummyYear, "d MMM yyyy", culture, DateTimeStyles.None, out DateTime dt)) return dt;
            }
            throw new FormatException($"Unable to parse date: {dayAndDate}");
        }

        static public void CustomPrintTable(HtmlNodeCollection rows)
        {
            if (rows == null) return;

            string day = "";
            DateTime date = new DateTime();

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                if (cells == null) continue;
                CustomPrintTableRowData(cells, day, date);

                // Tracks current day and date in case of empty cells. Can happen
                // due to two or more consecutive lessons on the same day.
                // Example:
                // | Mån | 19 aug | 08:00 | 10:00 | Matematik | IK205G | 
                 //|     |        | 10:00 | 12:00 | Matematik | IK205G |
                if (cells.Count > 1 && !string.IsNullOrEmpty(CleanText(cells[0].InnerText))) day = CleanText(cells[0].InnerText);
                if (cells.Count > 1 && !string.IsNullOrEmpty(CleanText(cells[1].InnerText))) date = ParseDate(CleanText(cells[1].InnerText));
            }
        }

        static public void BasicPrintTable(HtmlNodeCollection rows)
        {
            if (rows == null) return;
            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                if (cells == null) continue;
                BasicPrintTableRowData(cells);
            }
        }

        // Takes the row and hardcodes what I want to have
        // displayed. I don't care much for who the day's teacher might be.
        static public void CustomPrintTableRowData(HtmlNodeCollection cells, string lastDay, DateTime lastDate)
        {
            if (cells.Count == 1) // Probably a header row.
            {
                Console.WriteLine(CleanText(cells[0].InnerText)); return;
            }

            if (string.IsNullOrEmpty(CleanText(cells[1].InnerText))) cells[1].InnerHtml = lastDay;
            if (string.IsNullOrEmpty(CleanText(cells[2].InnerText))) cells[2].InnerHtml = lastDate.ToString("d MMM"); // "1 Jan". 5 Maj -> 5 May because of culture issues. Low prio problem.
            if (!string.IsNullOrEmpty(CleanText(cells[5].InnerText))) cells[5].InnerHtml = "*GRUPP " + CleanText(cells[5].InnerText + "*"); // Calls attention to groups.
            cells.Remove(6); // Removes Teacher column.
            cells.Remove(0); // Removes mystery column that says "A" during exam days.

            foreach (var cell in cells)
            {
                string cellText = CleanText(cell.InnerText);
                if (!string.IsNullOrWhiteSpace(cellText)) Console.Write(cellText + " | ");
            }
            Console.WriteLine(" ");
        }

        static public void BasicPrintTableRowData(HtmlNodeCollection cells)
        {
            if (cells == null) return;
            foreach (var cell in cells)
            {
                string cellText = CleanText(cell.InnerText);
                if (!string.IsNullOrWhiteSpace(cellText)) Console.Write(cellText + " | ");
            }
            Console.WriteLine(" ");
        }
    }
}
