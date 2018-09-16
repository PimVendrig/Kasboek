using System;
using System.Collections.Generic;
using System.Linq;

namespace Kasboek.WebApp.Utils
{
    /// <summary>
    /// Simpele CSV reader:
    /// Geen ondersteuning voor quotes.
    /// Geen ondersteuning daardoor voor newlines of separator als content.
    /// Geen ondersteuning voor minder waarden dan de eerste regel.
    /// Lege regels worden overgeslagen.
    /// </summary>
    public class CsvReader
    {

        protected string Content { get; set; }
        protected char Separator { get; set; }
        protected bool Parsed { get; set; }
        protected List<List<string>> Result { get; set; }
        protected List<string> ValidationErrors { get; set; }

        public CsvReader(string content, char separator)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Separator = separator;
        }

        public List<string> Validate()
        {
            if (!Parsed) Parse();
            return ValidationErrors;
        }

        public List<List<string>> ReadAll()
        {
            if (!Parsed) Parse();
            if (ValidationErrors.Any())
            {
                throw new InvalidOperationException("Het CVS-bestand bevat fouten.");
            }
            return Result;
        }

        protected void Parse()
        {
            Result = new List<List<string>>();
            ValidationErrors = new List<string>();

            var rows = Content.Split(Environment.NewLine);
            bool firstRow = true;
            int valueCount = 0;

            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                if (row.Length == 0)
                {
                    continue;
                }

                var values = row.Split(Separator);
                if (firstRow)
                {
                    valueCount = values.Length;
                    firstRow = false;
                }

                if (values.Length != valueCount)
                {
                    ValidationErrors.Add($"Regel {i + 1} heeft {values.Length} waarden in plaats van {valueCount} zoals in de eerste regel is bepaald.");
                    continue;
                }
                Result.Add(new List<string>(values));
            }

            if (Result.Count == 0)
            {
                ValidationErrors.Add("Geen gevulde regels in het CSV-bestand.");
            }

            Parsed = true;
        }

    }
}
