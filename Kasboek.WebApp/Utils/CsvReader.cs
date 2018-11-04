using System;
using System.Collections.Generic;
using System.Linq;

namespace Kasboek.WebApp.Utils
{
    /// <summary>
    /// Simpele CSV reader:
    /// Alleen ondersteuning voor alles quotes of niets quotes.
    /// Geen ondersteuning voor newlines of separator als content.
    /// Geen ondersteuning voor meer of minder waarden dan aangegeven.
    /// Lege regels worden overgeslagen.
    /// </summary>
    public class CsvReader
    {

        protected string Content { get; }
        protected char Separator { get; }
        protected char? Quote { get; }
        protected int AmountOfValues { get; }
        protected bool ContainsHeader { get; }
        protected bool Parsed { get; set; }
        protected List<List<string>> Result { get; set; }
        protected List<string> ValidationErrors { get; set; }

        public CsvReader(string content, char separator, char? quote, int amountOfValues, bool containsHeader)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Separator = separator;
            Quote = quote;
            AmountOfValues = amountOfValues;
            ContainsHeader = containsHeader;
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
            var combinedSeparator = Quote.HasValue ? $"{Quote.Value}{Separator}{Quote.Value}" : $"{Separator}";

            for (var i = ContainsHeader ? 1 : 0; i < rows.Length; i++)
            {
                var row = rows[i];
                if (row.Length == 0)
                {
                    continue;
                }

                var values = row.Split(combinedSeparator);
                if (values.Length != AmountOfValues)
                {
                    ValidationErrors.Add($"Regel {i + 1} heeft {values.Length} waarden in plaats van {AmountOfValues} zoals is verwacht.");
                    continue;
                }
                if (Quote.HasValue)
                {
                    var firstValue = values.First();
                    if (firstValue.StartsWith(Quote.Value))
                    {
                        values[0] = firstValue.Substring(1);
                    }
                    else
                    {
                        ValidationErrors.Add($"Regel {i + 1} begint niet met het teken {Quote.Value}.");
                        continue;
                    }
                    var lastValue = values.Last();
                    if (lastValue.EndsWith(Quote.Value))
                    {
                        values[values.Length - 1] = lastValue.Substring(0, lastValue.Length - 1);
                    }
                    else
                    {
                        ValidationErrors.Add($"Regel {i + 1} eindigt niet met het teken {Quote.Value}.");
                        continue;
                    }
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
