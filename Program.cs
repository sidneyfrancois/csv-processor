using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Diagnostics;

var mainProcessing = new CSVProcessing();
var sw = new Stopwatch();

string[] directoryFiles = Directory.GetFiles("TestInputT");

sw.Start();
await Parallel.ForEachAsync(directoryFiles, async (file, ct) =>
{
    await mainProcessing.CsvMapAndProcessing(file);
});
sw.Stop();

Console.WriteLine($"\nTime: {sw.ElapsedMilliseconds} ms");
Console.WriteLine($"Memory Used: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} mb");

public class CSVProcessing
{
    public async Task CsvMapAndProcessing(string filename)
    {
        var configCsvHelper = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
        };

        var employees = new List<Employee>();

        using (var reader = new StreamReader(filename))
        using (var csv = new CsvReader(reader, configCsvHelper))
        {
            await csv.ReadAsync();
            csv.ReadHeader();

            csv.Context.RegisterClassMap<EmployeeCsvMap>();

            while (await csv.ReadAsync())
            {
                var employee = csv.GetRecord<Employee>();
                employees.Add(employee);
            }

            Console.WriteLine("Finished file: " + filename);
        }
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double HourValue { get; set; }
    public DateTime Date { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public List<DateTime> LunchTime { get; set; }
}

public class EmployeeJson
{
    public string Name { get; set; }
    public int Id { get; set; }
    public double TotalRevenue { get; set; }
    public DateTime ExtraHours { get; set; }
    public DateTime OwedHours { get; set; }
    public int MissingDays { get; set; }
    public int ExtraDays { get; set; }
    public int TotalDaysOfWork { get; set; }
}

public class UtilsProcessing
{
    public List<DateTime> GetWeekDays(int month, int year)
    {
        return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                    .Select(day => new DateTime(year, month, day))
                    .Where(dt => dt.DayOfWeek != DayOfWeek.Sunday &&
                                 dt.DayOfWeek != DayOfWeek.Saturday)
                    .ToList();
    }

    public List<DateTime> GetWeekendsDays(int month, int year)
    {
        return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                    .Select(day => new DateTime(year, month, day))
                    .Where(dt => dt.DayOfWeek == DayOfWeek.Sunday ||
                                 dt.DayOfWeek == DayOfWeek.Saturday)
                    .ToList();
    }
    public List<List<Employee>> GetListOfEmployesById(List<Employee> employees)
    {
        var groupedEmployeeList = employees
                            .GroupBy(u => u.Id)
                            .Select(grp => grp.ToList())
                            .ToList();
        
        return groupedEmployeeList;
    }

    public int GetMissingDays(List<Employee> employeeReport)
    {
        List<DateTime> weekDaysOfMonth = GetWeekDays(employeeReport[0].Date.Month, employeeReport[0].Date.Year);
        var missingDates = weekDaysOfMonth.Select(x=>x.Date).Except(employeeReport.Select(y=>y.Date));

        return missingDates.Count();
    }

    public TimeSpan GetTotalDaysOfWork(List<Employee> employeeReport)
    {
        var total= employeeReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                            current += (it.ExitTime - it.EntryTime)
                                        );

        return total;
    }

    public TimeSpan GetTotalExtraHours(List<Employee> employeeReport)
    {
        var entryExtraDays = GetExtraHoursBeforeEntry(employeeReport);
        var exitExtraDays = GetExtraHoursAfterExit(employeeReport);

        var totalExtraHours = entryExtraDays + exitExtraDays;

        return totalExtraHours; 
    }

    public TimeSpan GetTotalWorkingHours(List<Employee> employeeReport)
    {
        var weekWorkingDaysReport = GetWeekWorkingDays(employeeReport);
        var weekendWorkingDaysReport = GetWeekendWorkingDays(employeeReport);

        var totalWeekWorkingHours = weekWorkingDaysReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                                            current += (it.ExitTime - it.EntryTime));

        var totalWeekendWorkingHours = weekendWorkingDaysReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                                            current += (it.ExitTime - it.EntryTime));

        var totalLunchHours = GetTotalLunchHours(employeeReport);

        var totalWorkingHours = totalWeekWorkingHours + totalWeekendWorkingHours - totalLunchHours;

        return totalWorkingHours;                                                            
    }

    public List<Employee> GetWeekWorkingDays(List<Employee> employeeReport)
    {
        var weekDays = GetWeekDays(employeeReport[0].Date.Month, employeeReport[0].Date.Year);

        var weekWorkingDaysReport = employeeReport
            .Where(x => weekDays.Any(y => y.Date == x.Date))
            .ToList();

        return weekWorkingDaysReport;
    }

    public List<Employee> GetWeekendWorkingDays(List<Employee> employeeReport)
    {
        var weekendDays = GetWeekendsDays(employeeReport[0].Date.Month, employeeReport[0].Date.Year);

        var weekendWorkingDaysReport = employeeReport
            .Where(x => weekendDays.Any(y => y.Date == x.Date))
            .ToList();

        return weekendWorkingDaysReport;
    }

    public TimeSpan GetOwedHoursAfterEntry(List<Employee> employeeReport)
    {
        var weekWorkingDaysReport = GetWeekWorkingDays(employeeReport);
        
        var total= weekWorkingDaysReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                        {
                                            DateTime entryTimeOfficial = new DateTime(
                                                                it.Date.Year,
                                                                it.Date.Month, 
                                                                it.Date.Day, 
                                                                8, 0, 0);

                                            if (it.EntryTime > entryTimeOfficial)
                                                return current += (it.EntryTime - entryTimeOfficial);
                                            else
                                                return current;
                                        });
        
        return total;
    }

    public TimeSpan GetOwedHoursBegoreExit(List<Employee> employeeReport)
    {
        var weekWorkingDaysReport = GetWeekWorkingDays(employeeReport);

        var total= weekWorkingDaysReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                        {
                                            DateTime exitTimeOfficial = new DateTime(
                                                                it.Date.Year,
                                                                it.Date.Month, 
                                                                it.Date.Day, 
                                                                18, 0, 0);

                                            if (it.ExitTime < exitTimeOfficial)
                                                return current += (exitTimeOfficial - it.ExitTime);
                                            else
                                                return current;
                                        });

        return total;
    }

    public TimeSpan GetExtraHoursBeforeEntry(List<Employee> employeeReport)
    {
        var weekWorkingDaysReport = GetWeekWorkingDays(employeeReport);
        
        var total= weekWorkingDaysReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                        {
                                            DateTime entryTimeOfficial = new DateTime(
                                                                it.Date.Year,
                                                                it.Date.Month, 
                                                                it.Date.Day, 
                                                                8, 0, 0);
                                            if (it.EntryTime < entryTimeOfficial)
                                                return current += (entryTimeOfficial - it.EntryTime);
                                            else
                                                return current;
                                        });
        
        return total;
    }

    public TimeSpan GetExtraHoursAfterExit(List<Employee> employeeReport)
    {
        var weekWorkingDaysReport = GetWeekWorkingDays(employeeReport);

        var total= weekWorkingDaysReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                        {
                                            DateTime exitTimeOfficial = new DateTime(
                                                                it.Date.Year,
                                                                it.Date.Month, 
                                                                it.Date.Day, 
                                                                18, 0, 0);

                                            if (it.ExitTime > exitTimeOfficial)
                                                return current += (it.ExitTime - exitTimeOfficial);
                                            else
                                                return current;
                                        });

        return total;
    }

    public TimeSpan GetExtraHoursOnWeekendDays(List<Employee> empleeReport)
    {
        var weekendWorkingDaysReport = GetWeekendWorkingDays(empleeReport);

        var total= weekendWorkingDaysReport.Aggregate(TimeSpan.Zero, (current, it) => 
                                            current += (it.ExitTime - it.EntryTime));

        return total;
    }

    public TimeSpan GetTotalLunchHours(List<Employee> employeeReport)
    {
        var totalLunchTime = employeeReport.Aggregate(TimeSpan.Zero, (current, it) =>
                                            current += (it.LunchTime[1] - it.LunchTime[0]));

        return totalLunchTime;
    }
}

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