using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public sealed class EmployeeCsvMap : ClassMap<Employee>
{
    public EmployeeCsvMap()
    {
        Map(e => e.Id).Name("C�digo");
        Map(e => e.Name).Name("Nome");
        Map(e => e.HourValue).Name("Valor hora").TypeConverter<CurrencyValueToDoubleConverter>();
        Map(e => e.Date).Name("Data").TypeConverter<DateToDateTimeConverter>();
        Map(e => e.EntryTime).Name("Entrada").TypeConverter<TimeToDateTimeConverter>();
        Map(e => e.ExitTime).Name("Sa�da").TypeConverter<TimeToDateTimeConverter>();
        Map(e => e.LunchTime).Name("Almo�o").TypeConverter<LunchIntervalToDateTimeConverter>();
    }
}

public class CurrencyValueToDoubleConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        text = Regex.Replace(text, " *, *", ",");
        var currencyConverted = Double.Parse(text, System.Globalization.NumberStyles.Currency);

        return currencyConverted;
    }
}

public class DateToDateTimeConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        DateTime dateConverted = DateTime.ParseExact(text, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);

        return dateConverted;
    }
}

public class TimeToDateTimeConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        DateTime timeConverted = DateTime.ParseExact(row[3] + " " + text, "dd/MM/yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);

        return timeConverted;
    }
}

public class LunchIntervalToDateTimeConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        text = Regex.Replace(text, " * - *", "-");
        string[] timeInterval = text.Split('-');

        var lunchEntryTime = DateTime.ParseExact(row[3] + " " + timeInterval[0], "dd/MM/yyyy HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);

        var lunchExitTime = DateTime.ParseExact(row[3] + " " + timeInterval[1], "dd/MM/yyyy HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);    

        return new List<DateTime> {lunchEntryTime, lunchExitTime};
    }
}