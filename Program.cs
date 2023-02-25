using System.Globalization;
using CsvHelper.Configuration;

public class CSVProcessing
{
    public void Reading(string filename)
    {
        var configCsvHelper = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
        };
    }
}